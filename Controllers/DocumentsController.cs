using CampusHub.Data;
using CampusHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CampusHub.Controllers
{
[Authorize(Roles = "Student,Teacher,Admin")]

    public class DocumentsController : Controller
    {
        private readonly CampusDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(
            CampusDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // =========================
        // LIST DOCUMENTS
        // =========================
      public async Task<IActionResult> Index()
{
    var documents = await _context.Documents
        .OrderByDescending(d => d.UploadedAt)
        .ToListAsync();

    return View(documents);
}

        // =========================
        // UPLOAD PAGE
        // =========================
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // =========================
        // UPLOAD POST
        // =========================
      [HttpPost]
public async Task<IActionResult> Upload(string title, IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest("File required");

    var allowedExtensions = new[] { ".pdf", ".docx", ".pptx", ".xlsx" };
    var extension = Path.GetExtension(file.FileName).ToLower();

    if (!allowedExtensions.Contains(extension))
        return BadRequest("Invalid file type");

    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
    if (!Directory.Exists(uploadsFolder))
        Directory.CreateDirectory(uploadsFolder);

    var uniqueFileName = Guid.NewGuid() + extension;
    var path = Path.Combine(uploadsFolder, uniqueFileName);

    using (var stream = new FileStream(path, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var role = User.FindFirst(ClaimTypes.Role)?.Value;

    var doc = new Document
    {
        Title = title,
        FileName = file.FileName,
        FilePath = "/uploads/" + uniqueFileName,
        UserId = userId,
        UserRole = role,
        UploadedAt = DateTime.Now
    };

    _context.Documents.Add(doc);
    await _context.SaveChangesAsync();

    return RedirectToAction("Index");
}
        // =========================
        // DOWNLOAD
        // =========================
       [Authorize(Roles = "Student,Teacher,Admin")]
public async Task<IActionResult> Download(int id)
{
    var document = await _context.Documents.FindAsync(id);

    if (document == null)
        return NotFound();

    var path = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));

    var memory = new MemoryStream();
    using (var stream = new FileStream(path, FileMode.Open))
    {
        await stream.CopyToAsync(memory);
    }

    memory.Position = 0;

    return File(memory, "application/octet-stream", document.FileName);
}
        // =========================
        // DELETE
        // =========================
        [HttpPost]
public async Task<IActionResult> Delete(int id)
{
    var document = await _context.Documents.FindAsync(id);

    if (document == null)
        return NotFound();

    // Supprimer le fichier physique
    var path = Path.Combine(
        _environment.WebRootPath,
        document.FilePath.TrimStart('/'));

    if (System.IO.File.Exists(path))
    {
        System.IO.File.Delete(path);
    }

    // Supprimer l'enregistrement de la base
    _context.Documents.Remove(document);

    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}
    }
}