using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Chat;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatBusiness chatBusiness, ILogger<ChatController> logger) : ControllerBase
{
    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await chatBusiness.SendMessageAsync(request, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "El chat no está configurado.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Falló la solicitud de chat a Gemini.");
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { error = ex.Message });
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(
                StatusCodes.Status504GatewayTimeout,
                new { error = "Gemini tardó demasiado en responder. Inténtalo nuevamente." });
        }
    }
}
