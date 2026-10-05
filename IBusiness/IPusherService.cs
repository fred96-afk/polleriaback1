namespace IBusiness;

public interface IPusherService
{
    Task TriggerAsync(string channel, string @event, object data);
}
