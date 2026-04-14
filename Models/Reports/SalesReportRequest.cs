namespace Models.Reports;

public record SalesReportRequest(
    DateTime StartDate,
    DateTime EndDate
);
