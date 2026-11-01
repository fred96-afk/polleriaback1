using IBusiness;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PusherServer;

namespace Business;

public class PusherService : IPusherService
{
    private readonly Pusher? _pusher;
    private readonly ILogger<PusherService> _logger;

    public PusherService(IOptions<PusherSettings> settings, ILogger<PusherService> logger)
    {
        _logger = logger;

        try
        {
            // PusherServer usa Newtonsoft.Json para serializar el payload.
            // Forzamos camelCase para que los clientes (Flutter/Angular) reciban
            // las propiedades con el mismo nombre que ya consumen.
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var pusherSettings = settings.Value;
            var appId = pusherSettings.AppId;
            var key = pusherSettings.Key;
            var secret = pusherSettings.Secret;
            var cluster = string.IsNullOrWhiteSpace(pusherSettings.Cluster) ? "us2" : pusherSettings.Cluster;

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

                _logger.LogInformation("Cliente de Pusher inicializado en el clúster '{Cluster}'.", cluster);
            }
            else
            {
                _logger.LogWarning("Configuración de Pusher incompleta o ausente. Las notificaciones en tiempo real estarán deshabilitadas.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar el cliente de Pusher.");
        }
    }

    public bool IsConfigured => _pusher != null;

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
