using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusHub.Data;
using CampusHub.Models;
using CampusHub.ViewModels;
using System.Security.Claims;

public class DashboardController : Controller
{
    private readonly CampusDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        CampusDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ==========================
    // ADMIN DASHBOARD
    // ==========================
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel();

        model.TotalClubs = await _context.Clubs.CountAsync();
        model.TotalEvents = await _context.Events.CountAsync();
        model.TotalReservations = await _context.RoomReservations.CountAsync();

        model.TotalStudents =
            (await _userManager.GetUsersInRoleAsync("Student")).Count;

        model.UpcomingEvents = await _context.Events
            .Where(e => e.Date >= DateTime.Now)
            .OrderBy(e => e.Date)
            .Take(5)
            .ToListAsync();

        model.EventStats = await _context.Events
            .Select(e => new EventStats
            {
                EventId = e.EventId,
                EventTitle = e.Title,
                ParticipantsCount = e.Participants.Count()
            })
            .ToListAsync();

        var studentRoleId = await _context.Roles
            .Where(r => r.Name == "Student")
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        model.LatestStudents = await _context.Users
            .Where(u => _context.UserRoles.Any(
                ur => ur.UserId == u.Id &&
                      ur.RoleId == studentRoleId))
            .OrderByDescending(u => u.Id)
            .Take(3)
            .ToListAsync();

        return View(model);
    }

    // ==========================
    // TEACHER DASHBOARD
    // ==========================
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Teacher()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var model = new TeacherDashboardViewModel();

        model.TotalEvents =
            await _context.Events.CountAsync();

        model.TotalReservations =
            await _context.RoomReservations
                .CountAsync(r => r.UserId == userId);

      model.TotalDocuments = await _context.Documents
    .Where(d => d.UserId == userId)
    .CountAsync();

        model.UpcomingEvents = await _context.Events
            .Where(e => e.Date >= DateTime.Now)
            .OrderBy(e => e.Date)
            .Take(5)
            .ToListAsync();

        model.MyReservations = await _context.RoomReservations
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.ReservationDate)
            .Take(5)
            .ToListAsync();

        model.RecentDocuments = await _context.Documents
            .OrderByDescending(d => d.UploadedAt)
            .Take(5)
            .ToListAsync();

        model.EventStats = await _context.Events
            .Select(e => new EventStats
            {
                EventId = e.EventId,
                EventTitle = e.Title,
                ParticipantsCount = e.Participants.Count()
            })
            .ToListAsync();
return View("TeacherDashboard", model);
    }
}