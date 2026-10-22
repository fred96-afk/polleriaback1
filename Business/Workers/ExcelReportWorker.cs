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

public class ExcelReportWorker : BackgroundService
{
    private readonly IRabbitMQConnectionManager _connectionManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<ExcelReportWorker> _logger;

    public ExcelReportWorker(
        IRabbitMQConnectionManager connectionManager,
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMQSettings> settings,
        ILogger<ExcelReportWorker> logger)
    {
        _connectionManager = connectionManager;
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExcelReportWorker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            IModel? channel = null;

            try
            {
                if (!_connectionManager.IsConnected && !_connectionManager.TryConnect())
                {
                    _logger.LogWarning("ExcelReportWorker: RabbitMQ no disponible. Reintentando en 10 segundos...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                channel = _connectionManager.CreateModel();
                if (channel == null)
                {
                    _logger.LogWarning("ExcelReportWorker: No se pudo obtener canal de RabbitMQ. Reintentando en 10 segundos...");
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
                    queue: _settings.ExcelQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueBind(
                    queue: _settings.ExcelQueueName,
                    exchange: _settings.ExchangeName,
                    routingKey: _settings.RoutingKeyExcel);

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
                        _logger.LogError(ex, "ExcelReportWorker: Error deserializando mensaje: {Json}", json);
                        channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    if (message != null)
                    {
                        await ProcessExcelMessageAsync(message);
                    }

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(
                    queue: _settings.ExcelQueueName,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation("ExcelReportWorker suscrito a la cola '{Queue}' con éxito.", _settings.ExcelQueueName);

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
                _logger.LogError(ex, "ExcelReportWorker: Error en el ciclo de ejecución. Reintentando en 10 segundos...");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            finally
            {
                try { channel?.Dispose(); } catch { /* Ignore cleanup errors */ }
            }
        }

        _logger.LogInformation("ExcelReportWorker detenido.");
    }

    private async Task ProcessExcelMessageAsync(ReportQueueMessage message)
    {
        var channelName = string.IsNullOrWhiteSpace(message.Channel) ? "reports" : message.Channel;
        _logger.LogInformation("ExcelReportWorker procesando reporte JobId: {JobId}", message.JobId);

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
                ReportType = "Excel",
                Status = "Processing",
                Message = $"Generando reporte Excel de ventas ({message.StartDate:dd/MM/yyyy} - {message.EndDate:dd/MM/yyyy})...",
                Timestamp = DateTime.UtcNow
            });

            // Generar el archivo Excel
            var excelBytes = await reportBusiness.GenerateSalesExcelAsync(new SalesReportRequest(message.StartDate, message.EndDate));

            // Guardar en almacenamiento
            var fileName = $"ReporteVentas_{message.JobId}_{message.StartDate:yyyyMMdd}_{message.EndDate:yyyyMMdd}.xlsx";
            await storageService.SaveReportAsync(fileName, excelBytes);
            var downloadUrl = storageService.GetDownloadUrl(fileName);

            _logger.LogInformation("ExcelReportWorker: Reporte Excel generado y guardado: {FileName} ({Bytes} bytes)", fileName, excelBytes.Length);

            var completedNotification = new ReportNotificationEvent
            {
                JobId = message.JobId,
                ReportType = "Excel",
                Status = "Completed",
                Message = "Reporte Excel de ventas generado exitosamente.",
                FileName = fileName,
                DownloadUrl = downloadUrl,
                FileSize = excelBytes.Length,
                Timestamp = DateTime.UtcNow
            };

            // Notificar que se completó
            await pusherService.TriggerAsync(channelName, "report-completed", completedNotification);
            await pusherService.TriggerAsync(channelName, "report-ready", completedNotification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ExcelReportWorker: Error al generar reporte Excel para JobId: {JobId}", message.JobId);

            await pusherService.TriggerAsync(channelName, "report-failed", new ReportNotificationEvent
            {
                JobId = message.JobId,
                ReportType = "Excel",
                Status = "Failed",
                Message = "Error al generar el reporte Excel de ventas.",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
