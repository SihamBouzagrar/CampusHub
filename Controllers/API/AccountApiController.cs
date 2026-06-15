using CampusHub.Models;
using CampusHub.Services;
using CampusHub.Web.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.ApiControllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountApiController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // =========================
        // REGISTER API
        // =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "Email already exists"
                });
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var role = RoleService.IsValidRole(model.Role)
                ? model.Role
                : RoleService.Student;

            await _userManager.AddToRoleAsync(user, role);

            return Ok(new
            {
                message = "User registered successfully",
                role
            });
        }

        // =========================
        // LOGIN API
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                false);

            if (!result.Succeeded)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                message = "Login successful",
                user = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.ProfileImageUrl,
                    roles
                }
            });
        }

        // =========================
        // LOGOUT API
        // =========================
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new
            {
                message = "Logged out successfully"
            });
        }

        // =========================
        // PROFILE API
        // =========================
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.ProfileImageUrl,
                roles
            });
        }
        // Ajoute cet endpoint dans ta classe AccountApiController

[Authorize(Roles = "Admin")]   // Seul l'admin peut voir la liste
[HttpGet("users")]
public async Task<IActionResult> GetAllUsers()
{
    var users = await _userManager.Users
        .Select(u => new
        {
            u.Id,
            u.Email,
            u.FullName,
            u.ProfileImageUrl,
            u.UserName,
            Roles = _userManager.GetRolesAsync(u).Result  // Sync pour simplifier
        })
        .ToListAsync();

    return Ok(new
    {
        total = users.Count,
        users
    });
}
    }
}