using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Business.RabbitMQ;

public class RabbitMQConnectionManager : IRabbitMQConnectionManager
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQConnectionManager> _logger;
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private readonly object _lock = new();
    private bool _disposed;

    public RabbitMQConnectionManager(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQConnectionManager> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        _factory = new ConnectionFactory
        {
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            RequestedHeartbeat = TimeSpan.FromSeconds(30),
            ClientProvidedName = "Polleria.API"
        };

        if (!string.IsNullOrWhiteSpace(_settings.Uri) && TryApplyUriSettings(_settings.Uri))
        {
            return;
        }

        ApplyHostSettings();
    }

    /// <summary>
    /// Configura la conexión a partir de una URI AMQP/AMQPS (p. ej. CloudAMQP).
    /// </summary>
    private bool TryApplyUriSettings(string uriValue)
    {
        if (!Uri.TryCreate(uriValue, UriKind.Absolute, out var uri))
        {
            _logger.LogError("URI de RabbitMQ inválida: {Uri}. Utilizando configuración por host.", uriValue);
            return false;
        }

        var isTls = uri.Scheme.Equals("amqps", StringComparison.OrdinalIgnoreCase);
        if (!isTls && !uri.Scheme.Equals("amqp", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError("Esquema de URI de RabbitMQ no soportado: {Scheme}. Se esperaba 'amqp' o 'amqps'.", uri.Scheme);
            return false;
        }

        var userInfo = uri.UserInfo.Split(':', 2);

        _factory.HostName = uri.Host;
        _factory.Port = uri.Port > 0 ? uri.Port : (isTls ? 5671 : 5672);
        _factory.UserName = userInfo.Length > 0 && !string.IsNullOrEmpty(userInfo[0])
            ? Uri.UnescapeDataString(userInfo[0])
            : "guest";
        _factory.Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "guest";

        var vhost = uri.AbsolutePath.Trim('/');
        _factory.VirtualHost = string.IsNullOrEmpty(vhost) ? "/" : Uri.UnescapeDataString(vhost);

        _factory.Ssl.Enabled = isTls;
        if (isTls)
        {
            _factory.Ssl.ServerName = uri.Host;
            _factory.Ssl.Version = System.Security.Authentication.SslProtocols.Tls12;
        }

        _logger.LogInformation(
            "RabbitMQ configurado mediante URI ({Scheme}) para host: {Host}:{Port}, VHost: {VHost}, TLS: {Tls}",
            uri.Scheme, _factory.HostName, _factory.Port, _factory.VirtualHost, isTls);

        return true;
    }

    private void ApplyHostSettings()
    {
        _factory.HostName = string.IsNullOrWhiteSpace(_settings.HostName) ? "localhost" : _settings.HostName;
        _factory.Port = _settings.Port > 0 ? _settings.Port : 5672;
        _factory.UserName = string.IsNullOrWhiteSpace(_settings.UserName) ? "guest" : _settings.UserName;
        _factory.Password = string.IsNullOrWhiteSpace(_settings.Password) ? "guest" : _settings.Password;
        _factory.VirtualHost = string.IsNullOrWhiteSpace(_settings.VirtualHost) ? "/" : _settings.VirtualHost;

        // Si el puerto es 5671 se asume TLS (CloudAMQP expone AMQPS ahí).
        if (_factory.Port == 5671)
        {
            _factory.Ssl.Enabled = true;
            _factory.Ssl.ServerName = _factory.HostName;
            _factory.Ssl.Version = System.Security.Authentication.SslProtocols.Tls12;
        }

        _logger.LogInformation("RabbitMQ configurado para host: {Host}:{Port}, VHost: {VHost}, TLS: {Tls}",
            _factory.HostName, _factory.Port, _factory.VirtualHost, _factory.Ssl.Enabled);
    }

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public bool TryConnect()
    {
        lock (_lock)
        {
            if (IsConnected) return true;

            try
            {
                _logger.LogInformation("Intentando conectar con RabbitMQ...");
                _connection = _factory.CreateConnection();

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += (sender, ea) =>
                    {
                        _logger.LogWarning("Conexión con RabbitMQ perdida. Causa: {ReplyText}", ea.ReplyText);
                    };

                    _logger.LogInformation("Conexión con RabbitMQ establecida exitosamente.");
                    return true;
                }

                _logger.LogWarning("No se pudo establecer conexión con RabbitMQ.");
                return false;
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogWarning(ex, "Servidor RabbitMQ no alcanzable ({Host}:{Port}).", _factory.HostName, _factory.Port);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al conectar con RabbitMQ.");
                return false;
            }
        }
    }

    public IModel? CreateModel()
    {
        if (!IsConnected && !TryConnect())
        {
            _logger.LogWarning("No se puede crear canal/modelo: RabbitMQ no está conectado.");
            return null;
        }

        try
        {
            return _connection?.CreateModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear canal de RabbitMQ.");
            return null;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al liberar la conexión de RabbitMQ.");
        }
    }
}
