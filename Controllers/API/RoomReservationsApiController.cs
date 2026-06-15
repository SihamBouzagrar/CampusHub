using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusHub.Data;
using CampusHub.Models;
using System.Security.Claims;

using CampusHub.API.DTOs;
using static CampusHub.Models.RoomReservation;
using Microsoft.AspNetCore.Authorization;

namespace CampusHub.API.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class RoomReservationsApiController : ControllerBase
    {
        private readonly CampusDbContext _context;

        public RoomReservationsApiController(CampusDbContext context)
        {
            _context = context;
        }

        // GET ALL (ADMIN)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.RoomReservations.ToListAsync();
            return Ok(data);
        }

        // GET BY USER
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var data = await _context.RoomReservations
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return Ok(data);
        }

        // CREATE
[HttpPost("create")]
public async Task<IActionResult> Create(RoomReservationCreateDto dto)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    await using var transaction =
        await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

    var conflict = await _context.RoomReservations.AnyAsync(r =>
        r.RoomName == dto.RoomName &&
        r.ReservationDate.Date == dto.ReservationDate.Date &&
        r.Status != ReservationStatus.Rejected &&
        dto.StartTime < r.EndTime &&
        dto.EndTime > r.StartTime
    );

    if (conflict)
    {
        return BadRequest("Schedule conflict: room already booked.");
    }

    var reservation = new RoomReservation
    {
        RoomName = dto.RoomName,
        ReservationDate = dto.ReservationDate.Date,
        StartTime = dto.StartTime,
        EndTime = dto.EndTime,
        UserId = userId,
        Status = ReservationStatus.Pending
    };

    _context.RoomReservations.Add(reservation);
    await _context.SaveChangesAsync();

    await transaction.CommitAsync();

    return Ok(reservation);
}
        // APPROVE
    [HttpPost("approve/{id}")]
public async Task<IActionResult> Approve(int id)
{
    await using var tx =
        await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

    var reservation = await _context.RoomReservations.FindAsync(id);

    if (reservation == null)
        return NotFound();

    var conflict = await _context.RoomReservations.AnyAsync(r =>
        r.Id != id &&
        r.RoomName == reservation.RoomName &&
        r.ReservationDate.Date == reservation.ReservationDate.Date &&
        r.Status == ReservationStatus.Approved &&
        reservation.StartTime < r.EndTime &&
        reservation.EndTime > r.StartTime
    );

    if (conflict)
    {
        return BadRequest("Conflict detected: room already approved for this time.");
    }

    reservation.Status = ReservationStatus.Approved;

    await _context.SaveChangesAsync();
    await tx.CommitAsync();

    return Ok(reservation);
}

        // REJECT
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            var res = await _context.RoomReservations.FindAsync(id);
            if (res == null) return NotFound();

            res.Status = ReservationStatus.Rejected;
            await _context.SaveChangesAsync();

            return Ok(res);
        }
    }
}