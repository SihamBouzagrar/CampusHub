using System.ComponentModel.DataAnnotations;
namespace CampusHub.Models
{
public class RoomReservation
{
    public int Id { get; set; }

    public string RoomName { get; set; } = string.Empty;

    public DateTime ReservationDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public string UserId { get; set; } = string.Empty;
     public ApplicationUser? User { get; set; }
    public enum ReservationStatus { Pending, Approved, Rejected }
}
}
