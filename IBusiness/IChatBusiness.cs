using Models.Chat;

namespace IBusiness;

public interface IChatBusiness
{
    Task<ChatResponse> SendMessageAsync(ChatRequest request, CancellationToken cancellationToken);
}
