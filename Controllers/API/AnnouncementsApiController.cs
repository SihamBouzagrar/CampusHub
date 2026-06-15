using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusHub.Data;
using CampusHub.API.DTOs;
using CampusHub.Models;

namespace CampusHub.API.Controllers
{
    [ApiController]
    [Route("api/announcements")]
    public class AnnouncementsApiController : ControllerBase
    {
        private readonly CampusDbContext _context;

        public AnnouncementsApiController(CampusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var query = _context.Announcements
                .AsNoTracking()
                .Include(a => a.Author)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(a =>
                    a.Title.Contains(search) ||
                    a.Content.Contains(search));
            }

            var announcements = await query
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementReadDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                   AuthorName = a.Author != null
    ? a.Author.UserName
    : "Unknown",
                    AuthorEmail = a.Author != null ? a.Author.Email : null
                })
                .ToListAsync();

            return Ok(announcements);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ann = await _context.Announcements
                .AsNoTracking()
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ann == null) return NotFound();

            return Ok(new AnnouncementReadDto
            {
                Id = ann.Id,
                Title = ann.Title,
                Content = ann.Content,
                CreatedAt = ann.CreatedAt,
                AuthorName = ann.Author != null ? ann.Author.FullName ?? ann.Author.UserName ?? "Unknown" : "Unknown",
                AuthorEmail = ann.Author?.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(
          [FromBody] AnnouncementCreateDto dto,
          [FromQuery] string userId)
        {
            try
            {
                var ann = new Announcement
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    CreatedAt = DateTime.Now,
                    UserId = userId
                };

                _context.Announcements.Add(ann);
                await _context.SaveChangesAsync();

                return Ok(ann);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ann = await _context.Announcements.FindAsync(id);
            if (ann == null) return NotFound();

            _context.Announcements.Remove(ann);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}