using Microsoft.AspNetCore.Identity;

namespace CampusHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        
    }
}