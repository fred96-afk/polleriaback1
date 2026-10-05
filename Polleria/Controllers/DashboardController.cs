using IBusiness;
using Microsoft.AspNetCore.Mvc;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardBusiness dashboardBusiness) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDashboardData()
    {
        return Ok(await dashboardBusiness.GetDashboardDataAsync());
    }
}
