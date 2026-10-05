using IBusiness;
using Microsoft.Extensions.Configuration;
using PusherServer;

namespace Business;

public class PusherService : IPusherService
{
    private readonly Pusher _pusher;

    public PusherService(IConfiguration configuration)
    {
        var section = configuration.GetSection("Pusher");
        _pusher = new Pusher(
            section["AppId"],
            section["Key"],
            section["Secret"],
            new PusherOptions
            {
                Cluster = section["Cluster"],
                Encrypted = true
            });
    }

    public async Task TriggerAsync(string channel, string @event, object data)
    {
        await _pusher.TriggerAsync(channel, @event, data);
    }
}
