using CampusHub.API.DTOs;

namespace CampusHub.API.DTOs
{
  public class RoomReservationCreateDto
{
    public string? RoomName { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
}