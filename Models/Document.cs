using System.ComponentModel.DataAnnotations;

namespace CampusHub.Models
{
public class Document
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string FileName { get; set; }
    public string FilePath { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public string UserId { get; set; }

    public string UserRole { get; set; } // Student / Teacher / Admin
}
}