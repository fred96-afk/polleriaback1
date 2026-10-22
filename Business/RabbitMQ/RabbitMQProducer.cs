using System.Text;
using System.Text.Json;
using IBusiness;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.RabbitMQ;
using Models.Reports;
using RabbitMQ.Client;

namespace Business.RabbitMQ;

public class RabbitMQProducer : IRabbitMQProducer
{
    private readonly IRabbitMQConnectionManager _connectionManager;
    private readonly RabbitMQSettings _settings;
    private readonly IPusherService _pusherService;
    private readonly ILogger<RabbitMQProducer> _logger;

    public RabbitMQProducer(
        IRabbitMQConnectionManager connectionManager,
        IOptions<RabbitMQSettings> settings,
        IPusherService pusherService,
        ILogger<RabbitMQProducer> logger)
    {
        _connectionManager = connectionManager;
        _settings = settings.Value;
        _pusherService = pusherService;
        _logger = logger;
    }

    public async Task<bool> PublishReportRequestAsync(ReportQueueMessage message)
    {
        try
        {
            using var channel = _connectionManager.CreateModel();
            if (channel == null)
            {
                _logger.LogError("No se pudo obtener un canal de RabbitMQ para publicar el reporte JobId: {JobId}", message.JobId);
                return false;
            }

            // Asegurar Exchange
            channel.ExchangeDeclare(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null);

            // Asegurar colas
            channel.QueueDeclare(
                queue: _settings.ExcelQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueDeclare(
                queue: _settings.PdfQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Asegurar enlaces
            channel.QueueBind(
                queue: _settings.ExcelQueueName,
                exchange: _settings.ExchangeName,
                routingKey: _settings.RoutingKeyExcel);

            channel.QueueBind(
                queue: _settings.PdfQueueName,
                exchange: _settings.ExchangeName,
                routingKey: _settings.RoutingKeyPdf);

            // Determinar routing key según tipo
            var isExcel = message.ReportType.Equals("Excel", StringComparison.OrdinalIgnoreCase);
            var routingKey = isExcel ? _settings.RoutingKeyExcel : _settings.RoutingKeyPdf;

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.CorrelationId = message.JobId;

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Mensaje publicado en RabbitMQ con éxito. JobId: {JobId}, Tipo: {ReportType}, RoutingKey: {RoutingKey}",
                message.JobId, message.ReportType, routingKey);

            // Notificación Push inmediata informando que el reporte está encolado
            var channelName = string.IsNullOrWhiteSpace(message.Channel) ? "reports" : message.Channel;
            await _pusherService.TriggerAsync(channelName, "report-queued", new ReportNotificationEvent
            {
                JobId = message.JobId,
                ReportType = message.ReportType,
                Status = "Queued",
                Message = $"Solicitud de reporte {message.ReportType} encolada exitosamente.",
                Timestamp = DateTime.UtcNow
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al publicar mensaje en RabbitMQ para JobId: {JobId}", message.JobId);
            return false;
        }
    }
}
