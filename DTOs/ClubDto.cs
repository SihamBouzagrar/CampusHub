namespace CampusHub.API.DTOs
{
    public class ClubDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
      
        public DateTime CreatedAt { get; set; }
  public string? LogoUrl { get; set; }

    }
}