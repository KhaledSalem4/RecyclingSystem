using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.AppUser;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RecyclingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        // 🔍 DIAGNOSTIC ENDPOINT - Check admin status
        [HttpGet("check-admin")]
        public async Task<IActionResult> CheckAdmin()
        {
            var admin = await _userManager.FindByEmailAsync("admin@recyclingsystem.com");
            
            if (admin == null)
                return Ok(new { exists = false, message = "Admin account does not exist in database" });

            var roles = await _userManager.GetRolesAsync(admin);
            
            return Ok(new
            {
                exists = true,
                email = admin.Email,
                emailConfirmed = admin.EmailConfirmed,
                lockoutEnabled = admin.LockoutEnabled,
                accessFailedCount = admin.AccessFailedCount,
                roles = roles,
                phoneNumber = admin.PhoneNumber,
                fullName = admin.FullName
            });
        }

        // 🔧 FIX ENDPOINT - Manually fix admin account
        [HttpPost("fix-admin")]
        public async Task<IActionResult> FixAdmin()
        {
            var admin = await _userManager.FindByEmailAsync("admin@recyclingsystem.com");
            
            if (admin == null)
                return NotFound("❌ Admin account not found in database. Use create-admin endpoint first.");

            admin.EmailConfirmed = true;
            admin.LockoutEnabled = false;
            admin.AccessFailedCount = 0;
            
            var result = await _userManager.UpdateAsync(admin);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            // Ensure admin role
            if (!await _userManager.IsInRoleAsync(admin, "Admin"))
            {
                await _userManager.AddToRoleAsync(admin, "Admin");
            }

            return Ok(new
            {
                message = "✅ Admin account fixed successfully",
                emailConfirmed = admin.EmailConfirmed,
                lockoutEnabled = admin.LockoutEnabled
            });
        }

        // 🆕 CREATE ENDPOINT - Force create admin if missing
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            var existing = await _userManager.FindByEmailAsync("admin@recyclingsystem.com");
            if (existing != null)
                return BadRequest("Admin account already exists. Use fix-admin endpoint instead.");

            var adminUser = new ApplicationUser
            {
                FullName = "System Administrator",
                Email = "admin@recyclingsystem.com",
                UserName = "admin@recyclingsystem.com",
                PhoneNumber = "+1234567890",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false,
                Points = 0,
                City = "System",
                Street = "Admin",
                BuildingNo = "1",
                Apartment = "1"
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin@123");

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            await _userManager.AddToRoleAsync(adminUser, "Admin");

            return Ok(new
            {
                message = "✅ Admin account created successfully",
                email = adminUser.Email,
                password = "Admin@123",
                emailConfirmed = true
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => new
                        {
                            code = "ValidationError",
                            description = e.ErrorMessage
                        })
                });
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(new
                {
                    errors = new[]
                    {
                new { code = "PasswordMismatch", description = "Passwords do not match." }
            }
                });
            }

            var result = await _authService.RegisterAsync(dto, role: "User");

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors.Select(e => new
                    {
                        code = e.Code,
                        description = e.Description
                    })
                });
            }

            return Ok(new { message = "User registered successfully." });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] string email,
            [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Email and token are required.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Invalid email.");

            if (user.EmailConfirmed)
                return Ok("Email already confirmed.");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return BadRequest("Invalid or expired confirmation link.");

            return Ok("Email confirmed successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authService.LoginAndGenerateTokenAsync(dto);
            if (token == null)
                return Unauthorized("Invalid credentials or email not confirmed.");

            return Ok(new { token });
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // We always return 200 to avoid leaking if email exists or not
            await _authService.SendPasswordResetLinkAsync(dto);

            return Ok("If an account with that email exists, a password reset link has been sent.");
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 🔥 Decode Token - VERY IMPORTANT
            dto.Token = Uri.UnescapeDataString(dto.Token);
            dto.Token = dto.Token.Replace(" ", "+");

            var result = await _authService.ResetPasswordAsync(dto);

            if (!result.Succeeded)
                return BadRequest("Invalid or expired reset link ❌");

            return Ok("Password has been reset successfully ✔");
        }


    }
}
