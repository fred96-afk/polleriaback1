using System.Text;
using System.Text.Json;
using Business.RabbitMQ;
using IBusiness;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.RabbitMQ;
using Models.Reports;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Business.Workers;

public class PdfReportWorker : BackgroundService
{
    private readonly IRabbitMQConnectionManager _connectionManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<PdfReportWorker> _logger;

    public PdfReportWorker(
        IRabbitMQConnectionManager connectionManager,
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMQSettings> settings,
        ILogger<PdfReportWorker> logger)
    {
        _connectionManager = connectionManager;
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PdfReportWorker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            IModel? channel = null;

            try
            {
                if (!_connectionManager.IsConnected && !_connectionManager.TryConnect())
                {
                    _logger.LogWarning("PdfReportWorker: RabbitMQ no disponible. Reintentando en 10 segundos...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                channel = _connectionManager.CreateModel();
                if (channel == null)
                {
                    _logger.LogWarning("PdfReportWorker: No se pudo obtener canal de RabbitMQ. Reintentando en 10 segundos...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                // Declarar exchange y cola
                channel.ExchangeDeclare(
                    exchange: _settings.ExchangeName,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false,
                    arguments: null);

                channel.QueueDeclare(
                    queue: _settings.PdfQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueBind(
                    queue: _settings.PdfQueueName,
                    exchange: _settings.ExchangeName,
                    routingKey: _settings.RoutingKeyPdf);

                // Procesar 1 mensaje a la vez
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    ReportQueueMessage? message = null;
                    try
                    {
                        message = JsonSerializer.Deserialize<ReportQueueMessage>(json);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "PdfReportWorker: Error deserializando mensaje: {Json}", json);
                        channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    if (message != null)
                    {
                        await ProcessPdfMessageAsync(message);
                    }

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(
                    queue: _settings.PdfQueueName,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation("PdfReportWorker suscrito a la cola '{Queue}' con éxito.", _settings.PdfQueueName);

                // Mantener el worker en espera mientras el canal y la conexión sigan vivos
                while (!stoppingToken.IsCancellationRequested && channel.IsOpen && _connectionManager.IsConnected)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PdfReportWorker: Error en el ciclo de ejecución. Reintentando en 10 segundos...");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            finally
            {
                try { channel?.Dispose(); } catch { /* Ignore cleanup errors */ }
            }
        }

        _logger.LogInformation("PdfReportWorker detenido.");
    }

    private async Task ProcessPdfMessageAsync(ReportQueueMessage message)
    {
        var channelName = string.IsNullOrWhiteSpace(message.Channel) ? "reports" : message.Channel;
        _logger.LogInformation("PdfReportWorker procesando reporte JobId: {JobId}", message.JobId);

        using var scope = _scopeFactory.CreateScope();
        var pusherService = scope.ServiceProvider.GetRequiredService<IPusherService>();
        var reportBusiness = scope.ServiceProvider.GetRequiredService<IReportBusiness>();
        var storageService = scope.ServiceProvider.GetRequiredService<IReportStorageService>();

        try
        {
            // Notificar que se está procesando
            await pusherService.TriggerAsync(channelName, "report-processing", new ReportNotificationEvent
            {
                JobId = message.JobId,
                ReportType = "Pdf",
                Status = "Processing",
                Message = $"Generando reporte PDF de ventas ({message.StartDate:dd/MM/yyyy} - {message.EndDate:dd/MM/yyyy})...",
                Timestamp = DateTime.UtcNow
            });

            // Generar el archivo PDF
            var pdfBytes = await reportBusiness.GenerateSalesPdfAsync(new SalesReportRequest(message.StartDate, message.EndDate));

            // Guardar en almacenamiento
            var fileName = $"ReporteVentas_{message.JobId}_{message.StartDate:yyyyMMdd}_{message.EndDate:yyyyMMdd}.pdf";
            await storageService.SaveReportAsync(fileName, pdfBytes);
            var downloadUrl = storageService.GetDownloadUrl(fileName);

            _logger.LogInformation("PdfReportWorker: Reporte PDF generado y guardado: {FileName} ({Bytes} bytes)", fileName, pdfBytes.Length);

            var completedNotification = new ReportNotificationEvent
            {
                JobId = message.JobId,
                ReportType = "Pdf",
                Status = "Completed",
                Message = "Reporte PDF de ventas generado exitosamente.",
                FileName = fileName,
                DownloadUrl = downloadUrl,
                FileSize = pdfBytes.Length,
                Timestamp = DateTime.UtcNow
            };

            // Notificar que se completó
            await pusherService.TriggerAsync(channelName, "report-completed", completedNotification);
            await pusherService.TriggerAsync(channelName, "report-ready", completedNotification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PdfReportWorker: Error al generar reporte PDF para JobId: {JobId}", message.JobId);

            await pusherService.TriggerAsync(channelName, "report-failed", new ReportNotificationEvent
            {
                JobId = message.JobId,
                ReportType = "Pdf",
                Status = "Failed",
                Message = "Error al generar el reporte PDF de ventas.",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
