using CampusHub.Models;
using CampusHub.Services;
using CampusHub.Web.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CampusHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        // =========================
        // REGISTER
        // =========================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Auth()
        {
            return View("~/Views/Account/Auth.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            var role = RoleService.IsValidRole(model.Role)
                ? model.Role
                : RoleService.Student;

            await _userManager.AddToRoleAsync(user, role);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        // =========================
        // LOGIN
        // =========================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginDto model)
{
    if (!ModelState.IsValid)
        return View(model);

    var user = await _userManager.FindByEmailAsync(model.Email);

    if (user == null)
    {
        ModelState.AddModelError("", "Invalid login");
        return View(model);
    }

    var result = await _signInManager.PasswordSignInAsync(
        user.UserName,
        model.Password,
        false,
        false);

    if (!result.Succeeded)
    {
        ModelState.AddModelError("", "Invalid login");
        return View(model);
    }

    // ROLE REDIRECTION
if (await _userManager.IsInRoleAsync(user, "Admin"))
    return Redirect("/AdminDashboard");

if (await _userManager.IsInRoleAsync(user, "Teacher"))
    return Redirect("/TeacherDashboard");

    return RedirectToAction("Index", "Home");
}
        // =========================
        // LOGOUT
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // =========================
        // PROFILE (GET)
        // =========================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        // =========================
        // PROFILE UPDATE (POST)
        // =========================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            user.FullName = model.FullName;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profile updated successfully";

            return RedirectToAction("Profile");
        }

        // =========================
        // UPLOAD PROFILE IMAGE
        // =========================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select an image";
                return RedirectToAction("Profile");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] = "Invalid image format";
                return RedirectToAction("Profile");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");

            var fileName = Guid.NewGuid() + extension;

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads/profiles");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfileImageUrl = "/uploads/profiles/" + fileName;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profile image updated successfully";

            return RedirectToAction("Profile");
        }
    }
}