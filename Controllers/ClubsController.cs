using Microsoft.AspNetCore.Mvc;
using CampusHub.Web.Services;
using CampusHub.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CampusHub.Data;


namespace CampusHub.Web.Controllers
{
    [Route("clubs")]
    public class ClubController : Controller
    {
        private readonly ClubApiService _service;
    

        public ClubController(ClubApiService service )
        {

            _service = service;
                   
        }


        // GET: /Club
    [HttpGet("")]

    public async Task<IActionResult> Index(string searchTerm)
    {
        var clubs = await _service.GetAll(); // ou 

        // 🔥 FILTRAGE C'EST ICI
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            clubs = clubs.Where(c => 
                c.Name.ToLower().Contains(searchTerm) ||
                (c.Category != null && c.Category.ToLower().Contains(searchTerm)) ||
                (c.Description != null && c.Description.ToLower().Contains(searchTerm))
            ).ToList();
        }
        ViewBag.SearchTerm = searchTerm;

        return View(clubs);
    }
        // GET: /Club/Details/5


[HttpGet("details/{id}")]
public async Task<IActionResult> Details(int id)
{
    var club = await _service.GetById(id);
    if (club == null) return NotFound();

    if (User.IsInRole("Student"))
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.IsMember = await _service.IsMember(id, userId!);
    }

    return View(club);
}

 // GET: /Club/Create
        [Authorize(Roles = "Admin")]
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Club/Create
  [Authorize(Roles = "Admin")]
   [HttpPost("create")]
        [ValidateAntiForgeryToken]
public async Task<IActionResult> Create(ClubDto model, IFormFile? LogoFile)
{
    if (!ModelState.IsValid)
        return View(model);
 string logoPath = "/images/default-club.png";

if (LogoFile != null && LogoFile.Length > 0)
{
    var fileName = Guid.NewGuid() + Path.GetExtension(LogoFile.FileName);

    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/clubs");

    if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

    var filePath = Path.Combine(folder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await LogoFile.CopyToAsync(stream);
    }

    logoPath = "/uploads/clubs/" + fileName;
}

    await _service.Create(new ClubDto
    {
        Name = model.Name,
        Description = model.Description,
        Category = model.Category,
        LogoUrl = logoPath
    });

    return RedirectToAction(nameof(Index));
}
[Authorize(Roles = "Admin")]
[HttpGet("edit/{id}")]
public async Task<IActionResult> Edit(int id)

        {
            var club = await _service.GetById(id);

            if (club == null)
                return NotFound();

            return View(club);
        }

        // POST: /Club/Edit/5
[Authorize(Roles = "Admin")]
[HttpPost("edit/{id}")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, ClubDto model, IFormFile? LogoFile)
{
    if (!ModelState.IsValid)
        return View(model);

    var club = await _service.GetById(id);
    if (club == null) return NotFound();

    string logoPath = club.LogoUrl;

    if (LogoFile != null && LogoFile.Length > 0)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(LogoFile.FileName);

        var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/clubs");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await LogoFile.CopyToAsync(stream);
        }

        logoPath = "/uploads/clubs/" + fileName;
    }

    // 🔥 IMPORTANT : UPDATE CORRECT OBJECT
await _service.Update(id, new ClubDto
{
    Name = model.Name,
    Description = model.Description,
    Category = model.Category,
    LogoUrl = logoPath
});

    return RedirectToAction(nameof(Index));
}
        // GET: /Club/Delete/5
  // GET: /Club/Delete/5
[Authorize(Roles = "Admin")]
[HttpGet("delete/{id}")]
public async Task<IActionResult> Delete(int id)
{
    var club = await _service.GetById(id);

    if (club == null)
        return NotFound();

    return View(club);
}

// POST: /Club/Delete/5
[Authorize(Roles = "Admin")]
[HttpPost("delete/{id}")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    if (id <= 0)
    {
        TempData["Error"] = "Invalid club id";
        return RedirectToAction(nameof(Index));
    }

   await _service.Delete(id);

return RedirectToAction(nameof(Index));
}
[Authorize(Roles = "Student")]
[HttpPost("join/{id}")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Join(int id)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var success = await _service.Join(id, userId!);

    if (success)
    {
        TempData["Success"] = "Vous avez rejoint le club avec succès.";
    }
    else
    {
        // Essaye de rejoindre pour voir le message d'erreur exact
        var debugResult = await _service.JoinDebug(id, userId!);
        TempData["Error"] = $"Erreur: {debugResult}";
    }

    return RedirectToAction(nameof(Details), new { id });
}

        [Authorize(Roles = "Student")]
        [HttpPost("leave/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _service.Leave(id, userId!);

            TempData[success ? "Success" : "Error"] = success
                ? "Vous avez quitté le club."
                : "Une erreur est survenue.";

            return RedirectToAction(nameof(Details), new { id });
        }


    }
}