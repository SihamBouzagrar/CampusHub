

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusHub.Data;
using CampusHub.API.DTOs;
using CampusHub.Models;
using Microsoft.AspNetCore.Authorization;

namespace CampusHub.API.Controllers
{
    [ApiController]
    [Route("api/events")]

    public class EventsApiController : ControllerBase
    {
        private readonly CampusDbContext _context;

        public EventsApiController(CampusDbContext context)
        {
            _context = context;
        }

       [HttpGet]
public async Task<IActionResult> GetAll(
    string? search,
    DateTime? date,
    string? category)
{
    var query = _context.Events.AsQueryable();

    if (!string.IsNullOrEmpty(search))
        query = query.Where(e =>
            e.Title.Contains(search) ||
            e.Description.Contains(search));

    if (date.HasValue)
        query = query.Where(e => e.Date.Date == date.Value.Date);

    

    var result = await query
        .Select(e => new EventReadDto
        {
            EventId = e.EventId,
            Title = e.Title,
            Description = e.Description,
            EventDate = e.Date,
            Location = e.Location,
          
            ImageUrl = e.ImageUrl
        })
        .ToListAsync();

    return Ok(result);
}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _context.Events
                .AsNoTracking()
                .Include(e => e.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound();

            return Ok(new EventReadDto
            {
                EventId = ev.EventId,
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                EventDate = ev.Date,
                MaximumParticipants = ev.MaximumParticipants,
                ImageUrl = ev.ImageUrl,
                CurrentParticipants = ev.Participants.Count,
                Participants = ev.Participants.Select(p => new ParticipantDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    FullName = p.User?.FullName,
                    Email = p.User?.Email,
                    RegisteredAt = p.RegisteredAt
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateDto dto)
        {
            var ev = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Date = dto.Date,
                MaximumParticipants = dto.MaximumParticipants,
                ImageUrl = dto.ImageUrl ?? "/images/default-event.png"
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return Ok(ev);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EventCreateDto dto)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            ev.Title = dto.Title;
            ev.Description = dto.Description;
            ev.Location = dto.Location;
            ev.Date = dto.Date;
            ev.MaximumParticipants = dto.MaximumParticipants;

            await _context.SaveChangesAsync();

            return Ok(ev);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return Ok();
        }
        // --- JOIN (register) ---
        [HttpPost("{id}/join")]
        public async Task<IActionResult> Join(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("userId missing");

            var ev = await _context.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.EventId == id);
            
            if (ev == null) return NotFound("Event not found");
            if (ev.Participants.Count >= ev.MaximumParticipants)
                return BadRequest("Event is full");



            // Check duplicate
            if (ev.Participants.Any(p => p.UserId == userId))
                return BadRequest("Already registered");

            _context.EventParticipations.Add(new EventParticipation
            {
                EventId = id,
                UserId = userId,
                RegisteredAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Registered successfully" });
        }
        // --- LEAVE (cancel) ---
        [HttpPost("{id}/leave")]
        public async Task<IActionResult> Leave(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("userId missing");

            var participation = await _context.EventParticipations
                .FirstOrDefaultAsync(p => p.EventId == id && p.UserId == userId);

            if (participation == null)
                return BadRequest("Not registered");

            _context.EventParticipations.Remove(participation);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cancelled successfully" });
        }
        [HttpGet("{id}/is-registered")]
        public async Task<IActionResult> IsRegistered(int id, [FromQuery] string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Ok(false);

            var isRegistered = await _context.EventParticipations
                .AnyAsync(p => p.EventId == id && p.UserId == userId);

            return Ok(isRegistered);
        }

        [HttpPost("{eventId}/remove-participant/{participationId}")]
        public async Task<IActionResult> RemoveParticipant(int eventId, int participationId)
        {
            var participation = await _context.EventParticipations
                .FirstOrDefaultAsync(p => p.Id == participationId && p.EventId == eventId);

            if (participation == null) return NotFound();

            _context.EventParticipations.Remove(participation);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}