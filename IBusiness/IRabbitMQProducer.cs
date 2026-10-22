using Models.Reports;

namespace IBusiness;

public interface IRabbitMQProducer
{
    Task<bool> PublishReportRequestAsync(ReportQueueMessage message);
}
