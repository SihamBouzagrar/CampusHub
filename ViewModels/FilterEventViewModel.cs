using CampusHub.Models;

public class EventFilterViewModel
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public DateTime? Date { get; set; }

    public List<Event> Events { get; set; } = new();
}