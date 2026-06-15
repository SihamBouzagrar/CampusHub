using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusHub.Data;
using CampusHub.API.DTOs;
using CampusHub.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CampusHub.API.Controllers
{
    [ApiController]
    [Route("api/clubs")]

    public class ClubsApiController : ControllerBase
    {
        private readonly CampusDbContext _context;

        public ClubsApiController(CampusDbContext context)
        {
            _context = context;
        }

     [HttpGet]
public async Task<IActionResult> GetAll(string? search)
{
    var query = _context.Clubs.AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(c =>
            c.Name.Contains(search) ||
            c.Description.Contains(search));
    }

    var result = await query
        .Select(c => new ClubDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Category = c.Category,
            LogoUrl = c.LogoUrl
        })
        .ToListAsync();

    return Ok(result);
}     
   [HttpGet("{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            var club = await _context.Clubs
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ClubDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Category = c.Category,
                    CreatedAt = c.CreatedAt,
                    LogoUrl = c.LogoUrl
                })
                .FirstOrDefaultAsync();

            if (club == null)
                return NotFound();

            return Ok(club);
        }

        [HttpPost]
   
        public async Task<IActionResult> Create(ClubDto model)
        {
            var club = new Club
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                CreatedAt = DateTime.Now,
                LogoUrl = model.LogoUrl ?? "/images/default-club.png",
            };

            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();

            return Ok(club);
        }

        [HttpPut("{id}")]
      
        public async Task<IActionResult> Update(int id, ClubDto model)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club == null) return NotFound();

            club.Name = model.Name;
            club.Description = model.Description;
            club.Category = model.Category;
            club.LogoUrl = model.LogoUrl;

            await _context.SaveChangesAsync();
            return Ok(club);
        }

        [HttpDelete("{id}")]
      
        public async Task<IActionResult> Delete(int id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club == null) return NotFound();

            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // --- JOIN (userId transmis par le MVC, pas d'Authorize ici) ---
    
[HttpPost("{id}/join")]

public async Task<IActionResult> Join(int id, [FromQuery] string userId)
{
    if (string.IsNullOrEmpty(userId))
        return BadRequest("userId missing");

    // Vérifie que le club existe
    var clubExists = await _context.Clubs.AnyAsync(c => c.Id == id);
    if (!clubExists)
        return NotFound($"Club {id} introuvable");

    // Vérifie si déjà membre
    var alreadyMember = await _context.ClubMemberships
        .AnyAsync(m => m.ClubId == id && m.UserId == userId);
    if (alreadyMember)
        return BadRequest("Vous êtes déjà membre de ce club");

    var membership = new ClubMembership
    {
        ClubId = id,
        UserId = userId,
        JoinedAt = DateTime.Now
    };

    _context.ClubMemberships.Add(membership);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Inscription réussie", clubId = id, userId });
}
        // --- LEAVE ---
[HttpPost("{id}/leave")]

public async Task<IActionResult> Leave(int id, [FromQuery] string userId)
{
    if (string.IsNullOrEmpty(userId))
        return BadRequest("userId manquant");

    var membership = await _context.ClubMemberships
        .FirstOrDefaultAsync(m => m.ClubId == id && m.UserId == userId);

    if (membership == null)
        return BadRequest("Vous n'êtes pas membre de ce club");

    _context.ClubMemberships.Remove(membership);
    await _context.SaveChangesAsync();
    return Ok();
}
        // --- IS MEMBER ---
[HttpGet("{id}/is-member")]

public async Task<IActionResult> IsMember(int id, [FromQuery] string userId)
{
    var isMember = await _context.ClubMemberships
        .AnyAsync(m => m.ClubId == id && m.UserId == userId);

    return Ok(isMember);
}
    }
}