using System.ComponentModel.DataAnnotations;
namespace CampusHub.API.DTOs
{

public class EventCreateDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Range(1, 1000)]
    public int MaximumParticipants { get; set; }

    public string? ImageUrl { get; set; }
}
}