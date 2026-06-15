using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CampusHub.Models;
using CampusHub.API.DTOs;
using CampusHub.Data;
using static CampusHub.Models.RoomReservation;
[Authorize]

[Route("reservations")]
public class RoomReservationsController : Controller
{
    private readonly CampusDbContext _context;


    private readonly RoomReservationApiService _service;

    public RoomReservationsController(CampusDbContext context, RoomReservationApiService service)
    {
        _context = context;
        _service = service;

    }
 [Authorize(Roles = "Student,Teacher,Admin")]

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var data = await _service.GetAll();
        return View(data);
    }
[Authorize(Roles = "Student,Teacher")]
[HttpGet("create")]
public IActionResult Create()
{
    return View();
}
[Authorize(Roles = "Student,Teacher")]
[HttpPost("create")]
[ValidateAntiForgeryToken]

public async Task<IActionResult> Create(RoomReservation model)
{
    if (!ModelState.IsValid)
        return View(model);

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    model.UserId = userId;
    model.Status = ReservationStatus.Pending;

    _context.RoomReservations.Add(model);
    await _context.SaveChangesAsync();

    return RedirectToAction("MyReservations");
}
    [Authorize(Roles = "Student,Teacher")]
    [HttpGet("my", Name = "MyReservations")]
    public async Task<IActionResult> MyReservations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var data = await _service.GetByUser(userId);

        return View(data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("approve/{id}")]
    public async Task<IActionResult> Approve(int id)
    {
        await _service.Approve(id);
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("reject/{id}")]
    public async Task<IActionResult> Reject(int id)
    {
        await _service.Reject(id);
        return RedirectToAction("Index");
    }
}