using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusHub.Data;
using CampusHub.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CampusHub.API.DTOs;
using CampusHub.Web.Services;

namespace CampusHub.Controllers
{
 
    [Route("events")]
    public class EventsController : Controller
    {
      private readonly CampusDbContext _context;
    private readonly IWebHostEnvironment _env;
 private readonly EventApiService _service;


    // ✅ UN SEUL CONSTRUCTEUR avec tout
    public EventsController(
        EventApiService service, 
        CampusDbContext context, 
        IWebHostEnvironment env)
    {
        _service = service;
        _context = context;
        _env = env;
    }

        // =========================
        // LIST + SEARCH + FILTER
        // =========================
[HttpGet("")]
public async Task<IActionResult> Index(string? search, string? category, DateTime? date)
{
    var events = await _service.GetAll(search, date, category);

    ViewBag.CurrentSearch = search;
    ViewBag.CurrentCategory = category;
    ViewBag.CurrentDate = date;

    return View(events);
}

        // =========================
        // CREATE
                [Authorize(Roles = "Admin,Teacher")]
        [HttpGet("create")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateDto model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid) return View(model);

            string imagePath = "/images/default-event.png";
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/events");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await ImageFile.CopyToAsync(stream);

                imagePath = "/uploads/events/" + fileName;
            }

            await _service.Create(new EventCreateDto
            {
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                Date = model.Date,
                MaximumParticipants = model.MaximumParticipants,
                ImageUrl = imagePath
            });

            return RedirectToAction(nameof(Index));
        }
            // =========================
            // DETAILS

        // ✅ CORRIGÉ
[HttpGet("details/{id}")]
public async Task<IActionResult> Details(int id)
{
    var ev = await _service.GetById(id);  // ← _service, pas _context
    if (ev == null) return NotFound();

    if (User.IsInRole("Student"))
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.IsRegistered = await _service.IsRegistered(id, userId!);
    }

    return View(ev);  // ← Passe EventReadDto (DTO) à la vue
}
   
            // EDIT
                   [Authorize(Roles = "Admin,Teacher")]
            [HttpGet("edit/{id}")]
     
public async Task<IActionResult> Edit(int id)
        {
            var ev = await _service.GetById(id);
            if (ev == null) return NotFound();

            return View(new EventCreateDto
            {
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                Date = ev.EventDate,
                MaximumParticipants = ev.MaximumParticipants,
                ImageUrl = ev.ImageUrl
            });
        }
            [Authorize(Roles = "Admin,Teacher")]
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventCreateDto model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid) return View(model);

            var ev = await _service.GetById(id);
            if (ev == null) return NotFound();

            string imagePath = ev.ImageUrl ?? "/images/default-event.png";
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/events");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await ImageFile.CopyToAsync(stream);

                imagePath = "/uploads/events/" + fileName;
            }

           await _service.Update(id, new EventCreateDto
            {
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                Date = model.Date,
                MaximumParticipants = model.MaximumParticipants,
                ImageUrl = imagePath
            });

            
                return RedirectToAction(nameof(Index));
            }

            // =========================
            // DELETE
            // =========================
             [Authorize(Roles = "Admin,Teacher")]
        [HttpPost("delete/{id}")]
           
            [ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
{
    var success = await _service.Delete(id);

    if (!success)
        TempData["Error"] = "Event already deleted or not found";

    return RedirectToAction(nameof(Index));
}
            // =========================
            // JOIN EVENT
            // =========================
           [Authorize(Roles = "Student")]
        [HttpPost("join/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _service.Join(id, userId!);

            TempData[success ? "Success" : "Error"] = success
                ? "Registered successfully!"
                : "Could not register (full or already registered).";

            return RedirectToAction(nameof(Details), new { id });
        }

            // =========================
            // LEAVE EVENT
            // =========================
           [Authorize(Roles = "Student")]
        [HttpPost("leave/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _service.Leave(id, userId!);

            TempData[success ? "Success" : "Error"] = success
                ? "Participation cancelled."
                : "Could not cancel participation.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // --- REMOVE PARTICIPANT (Admin/Teacher) ---
   [Authorize(Roles = "Admin,Teacher")]
[HttpPost("remove-participant/{participationId}")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> RemoveParticipant(int participationId, [FromQuery] int eventId)
{
    await _service.RemoveParticipant(eventId, participationId);
    return RedirectToAction(nameof(Details), new { id = eventId });
}

        }
    }
