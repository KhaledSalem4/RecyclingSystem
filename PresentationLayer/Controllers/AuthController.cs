using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.AppUser;
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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var result = await _authService.RegisterAsync(dto, role: "User");

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully.");
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Email and token are required.");

            // Decode Token - VERY IMPORTANT
            token = Uri.UnescapeDataString(token);
            token = token.Replace(" ", "+");

            var result = await _authService.ConfirmEmailAsync(email, token);

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
