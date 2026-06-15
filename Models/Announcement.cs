using System.ComponentModel.DataAnnotations;
namespace CampusHub.Models
{
    public class Announcement
    {
         public int Id { get; set; }
        [Required, MaxLength(150)]
         [Display(Name = "Announcement title")]
        public string Title  { get; set; } = string.Empty;
        [Required, MaxLength(150)]
        [Display(Name = "Message content")]
        public string Content  { get; set; } = string.Empty;
         [DataType(DataType.DateTime)]
         [Display(Name = "Publication date")]
        public DateTime CreatedAt { get; set; }
        [Required, MaxLength(150)]
         [Display(Name = "Creator")]
public string UserId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }
        
    }

}