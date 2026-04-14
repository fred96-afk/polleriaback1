using Models.Dashboard;

namespace IBusiness;

public interface IDashboardBusiness
{
    Task<DashboardResponse> GetDashboardDataAsync();
}
