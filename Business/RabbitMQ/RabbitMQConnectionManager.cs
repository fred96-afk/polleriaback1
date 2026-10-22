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
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        if (!string.IsNullOrWhiteSpace(_settings.Uri))
        {
            try
            {
                _factory.Uri = new Uri(_settings.Uri);
                _logger.LogInformation("RabbitMQ configurado mediante URI amqp.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "URI de RabbitMQ inválida: {Uri}. Utilizando host por defecto.", _settings.Uri);
                ApplyHostSettings();
            }
        }
        else
        {
            ApplyHostSettings();
        }
    }

    private void ApplyHostSettings()
    {
        _factory.HostName = string.IsNullOrWhiteSpace(_settings.HostName) ? "localhost" : _settings.HostName;
        _factory.Port = _settings.Port > 0 ? _settings.Port : 5672;
        _factory.UserName = string.IsNullOrWhiteSpace(_settings.UserName) ? "guest" : _settings.UserName;
        _factory.Password = string.IsNullOrWhiteSpace(_settings.Password) ? "guest" : _settings.Password;
        _factory.VirtualHost = string.IsNullOrWhiteSpace(_settings.VirtualHost) ? "/" : _settings.VirtualHost;

        _logger.LogInformation("RabbitMQ configurado para host: {Host}:{Port}, VHost: {VHost}",
            _factory.HostName, _factory.Port, _factory.VirtualHost);
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
