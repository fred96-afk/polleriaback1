using IBusiness;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PusherServer;

namespace Business;

public class PusherService : IPusherService
{
    private readonly Pusher? _pusher;
    private readonly ILogger<PusherService> _logger;

    public PusherService(IConfiguration configuration, ILogger<PusherService> logger)
    {
        _logger = logger;
        try
        {
            var section = configuration.GetSection("Pusher");
            var appId = section["AppId"];
            var key = section["Key"];
            var secret = section["Secret"];
            var cluster = section["Cluster"] ?? "us2";

            if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(secret))
            {
                _pusher = new Pusher(
                    appId,
                    key,
                    secret,
                    new PusherOptions
                    {
                        Cluster = cluster,
                        Encrypted = true
                    });
            }
            else
            {
                _logger.LogWarning("Configuración de Pusher incompleta o ausente.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar el cliente de Pusher.");
        }
    }

    public async Task TriggerAsync(string channel, string @event, object data)
    {
        if (_pusher == null)
        {
            _logger.LogWarning("Cliente de Pusher no inicializado. Se omite notificación para el canal '{Channel}', evento '{Event}'.", channel, @event);
            return;
        }

        try
        {
            var result = await _pusher.TriggerAsync(channel, @event, data);
            _logger.LogInformation("Notificación Pusher enviada. Canal: {Channel}, Evento: {Event}, Estado: {StatusCode}", channel, @event, result.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación Pusher al canal '{Channel}', evento '{Event}'.", channel, @event);
        }
    }
}
