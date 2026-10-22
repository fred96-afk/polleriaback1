using RabbitMQ.Client;

namespace Business.RabbitMQ;

public interface IRabbitMQConnectionManager : IDisposable
{
    bool IsConnected { get; }
    bool TryConnect();
    IModel? CreateModel();
}
