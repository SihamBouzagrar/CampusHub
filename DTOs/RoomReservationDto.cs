

using static CampusHub.Models.RoomReservation;

namespace CampusHub.Web.DTOs
{
public class RoomReservationDto
{
    public int Id { get; set; }
    public string? RoomName { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public ReservationStatus Status { get; set; }
}
}