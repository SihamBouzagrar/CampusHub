using CampusHub.Models;

namespace CampusHub.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalClubs { get; set; }
        public int TotalStudents { get; set; }
        public int TotalEvents { get; set; }
        public int TotalReservations { get; set; }

        public List<Event> UpcomingEvents { get; set; } = new();
        public List<ApplicationUser> LatestStudents { get; set; } = new();
        public List<EventStats> EventStats { get; set; } = new();
    }

  
}