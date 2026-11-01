namespace IBusiness;

public interface IPusherService
{
    /// <summary>
    /// Indica si las credenciales de Pusher están configuradas correctamente.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Publica un evento en un canal de Pusher.
    /// </summary>
    Task TriggerAsync(string channel, string @event, object data);
}
