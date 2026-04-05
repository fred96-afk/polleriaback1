using System.ComponentModel.DataAnnotations;

namespace DbModel.Tables;

public class Banner
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public string? LinkUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int Order { get; set; } = 0;
}