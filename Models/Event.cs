using System.ComponentModel.DataAnnotations;
namespace CampusHub.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Event details")]
        public string? Description { get; set; }
        [Required]
        [Display(Name = "Event Location")]
        public string? Location { get; set; }
        [Required]

        [Display(Name = "Capacity")]
        public int MaximumParticipants { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Event date")]
        public DateTime Date { get; set; }
        [Display(Name = "Event Image")]
        public string? ImageUrl { get; set; }

       public ICollection<EventParticipation> Participants { get; set; } = new List<EventParticipation>();

    }
}