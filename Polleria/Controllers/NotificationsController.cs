using IBusiness;
using Microsoft.AspNetCore.Mvc;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(IPusherService pusherService) : ControllerBase
{
    /// <summary>
    /// Permite verificar que el backend tiene configurado Pusher y qué canales/eventos emite.
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            pusherConfigured = pusherService.IsConfigured,
            channels = new[]
            {
                new
                {
                    name = "orders",
                    events = new[] { "new-order", "order-created", "status-updated", "payment-updated", "order-accepted" }
                },
                new
                {
                    name = "tables",
                    events = new[] { "call-waiter", "request-bill", "table-alert" }
                },
                new
                {
                    name = "reports",
                    events = new[] { "report-queued", "report-processing", "report-completed", "report-ready", "report-failed" }
                }
            }
        });
    }
}
