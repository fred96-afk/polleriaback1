using Models.Reports;

namespace IBusiness;

public interface IReportBusiness
{
    Task<byte[]> GenerateSalesPdfAsync(SalesReportRequest request);
    Task<byte[]> GenerateSalesExcelAsync(SalesReportRequest request);
}
