using System.ComponentModel.DataAnnotations;

namespace Models.Chat;

public class ChatRequest
{
    public Guid ConversationId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(10)]
    public List<ChatHistoryMessage> History { get; set; } = [];
}

public class ChatHistoryMessage
{
    [Required]
    [RegularExpression("^(user|model)$")]
    public string Role { get; set; } = string.Empty;

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}
