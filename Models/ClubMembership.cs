using System.ComponentModel.DataAnnotations;

namespace CampusHub.Models{
public class ClubMembership
{
    public int Id { get; set; }
[Required]
public int ClubId { get; set; }
        public Club? Club { get; set; }
    [Required]
   public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.Now;
}
}