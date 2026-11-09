using System.Net.Mail;
using DbModel.Tables;
using IBusiness;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Email;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class MaintenanceController(
    IClientRepository clientRepository,
    IUserRepository userRepository,
    IEmailService emailService,
    ILogger<MaintenanceController> logger) : ControllerBase
{
    [HttpPost("notify")]
    public async Task<IActionResult> SendNotice([FromBody] MaintenanceNoticeRequest request)
    {
        var recipients = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var client in await clientRepository.GetAllAsync())
        {
            AddRecipient(recipients, client.Email, client.Name);
        }

        foreach (var user in await userRepository.GetAllAsync())
        {
            if (user.IsVerified)
            {
                AddRecipient(recipients, user.Email, user.Name);
            }
        }

        if (recipients.Count == 0)
        {
            return Ok(new { total = 0, sent = 0, failed = 0 });
        }

        var sent = 0;
        var failed = 0;

        foreach (var recipient in recipients)
        {
            try
            {
                await emailService.SendMaintenanceNoticeEmailAsync(
                    recipient.Key,
                    recipient.Value,
                    request.ScheduledDate,
                    request.EstimatedDuration,
                    request.Details,
                    request.SupportContact);
                sent++;
            }
            catch (Exception ex)
            {
                failed++;
                logger.LogError(ex, "No se pudo enviar el aviso de mantenimiento a {Email}.", recipient.Key);
            }
        }

        var result = new { total = recipients.Count, sent, failed };
        return failed == recipients.Count
            ? StatusCode(StatusCodes.Status502BadGateway, result)
            : Ok(result);
    }

    private static void AddRecipient(
        IDictionary<string, string> recipients,
        string? email,
        string? name)
    {
        if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out var address))
        {
            return;
        }

        recipients.TryAdd(address.Address, string.IsNullOrWhiteSpace(name) ? "Cliente" : name.Trim());
    }
}
