using System.ComponentModel.DataAnnotations;

namespace Models.Email;

public class EmailTemplateTestRequest
{
    [Required]
    [StringLength(200)]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ScheduledDate { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string EstimatedDuration { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Details { get; set; } = string.Empty;

    [StringLength(100)]
    public string? SupportContact { get; set; }
}
