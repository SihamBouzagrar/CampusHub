using CampusHub.Models;

namespace CampusHub.ViewModels
{
    public class TeacherDashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalReservations { get; set; }
        public int TotalDocuments { get; set; }

        public List<Event> UpcomingEvents { get; set; } = new();

        public List<RoomReservation> MyReservations { get; set; } = new();

        public List<Document> RecentDocuments { get; set; } = new();

        public List<EventStats> EventStats { get; set; } = new();
    }
}