namespace Models.Chat;

public record ChatResponse(
    Guid ConversationId,
    string Reply,
    string PusherChannel,
    string PusherEvent);
