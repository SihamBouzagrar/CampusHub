using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace CampusHub.Models
{
    public class Club
    {
        public int Id { get; set; }
        [Required, MaxLength(150)]
        [Display(Name = "Club name")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Club description")]
        public string? Description { get; set; }
        [Required]
        [Display(Name = "Club category")]
        public string? Category { get; set; }
      
        [Display(Name = "Club image/logo")]
        public string? LogoUrl { get; set; }
        [DataType(DataType.DateTime)]
        
        [Display(Name = "Creation date")]
        public DateTime CreatedAt { get; set; }
public ICollection<ClubMembership> Members { get; set; } = new List<ClubMembership>();
    }
}