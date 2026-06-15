namespace CampusHub.API.DTOs
{
public class EventReadDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
         public int CurrentParticipants { get; set; }
        public string Location { get; set; } = string.Empty;
        public int MaximumParticipants { get; set; }
        public string? ImageUrl { get; set; }
            public List<ParticipantDto> Participants { get; set; } = new();
    }
    public class ParticipantDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public DateTime RegisteredAt { get; set; }
}
}
