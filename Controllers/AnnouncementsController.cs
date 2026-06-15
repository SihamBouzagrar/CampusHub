using Microsoft.AspNetCore.Mvc;
using CampusHub.Web.Services;
using CampusHub.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CampusHub.Web.Controllers
{
    [Route("announcements")]
    public class AnnouncementsController : Controller
    {
        private readonly AnnouncementApiService _service;

        public AnnouncementsController(AnnouncementApiService service)
        {
            _service = service;
        }

        // =========================
        // LIST + SEARCH (Everyone)
        // =========================
        [HttpGet("")]
        public async Task<IActionResult> Index(string? search)
        {
            var announcements = await _service.GetAll(search);
            ViewBag.Search = search;
            return View(announcements);
        }

        // =========================
        // DETAILS (Everyone)
        // =========================
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var ann = await _service.GetById(id);
            if (ann == null) return NotFound();
            return View(ann);
        }

        // =========================
        // CREATE (Admin/Teacher)
        // =========================
        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet("create")]
        public IActionResult Create() => View();

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementCreateDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _service.Create(model, userId!);

            TempData["Success"] = "Announcement published successfully!";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE (Admin only)
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            TempData["Success"] = "Announcement deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}