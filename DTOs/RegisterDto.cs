using System.ComponentModel.DataAnnotations;

namespace CampusHub.Web.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
          public string? Role { get; set; }
    }
}