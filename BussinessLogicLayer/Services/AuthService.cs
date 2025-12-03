using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.AppUser;
using BussinessLogicLayer.IServices;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;


namespace BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _emailService = emailService;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterUserDto dto, string role)
        {
            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                Points = 0
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return result;

            if (!string.IsNullOrEmpty(role))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);
            }

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); // [5]

            // Build confirmation link to your API
            var apiBaseUrl = _config["AppSettings:ApiBaseUrl"] ?? "https://localhost:44375";
            var confirmationLink =
                $"{apiBaseUrl}/api/Auth/confirm-email?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

            // Send email
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"Please confirm your email by clicking this link: {confirmationLink}");

            return result;
        }
        public async Task<IdentityResult> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            return await _userManager.ConfirmEmailAsync(user, token);
        }
        public async Task<string> LoginAndGenerateTokenAsync(LoginUserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
            if (!result.Succeeded)
                return null;

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60")),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<bool> SendPasswordResetLinkAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return false; // don’t reveal that user doesn’t exist

            // Optionally ensure email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user); //[4][5]

            var apiBaseUrl = _config["AppSettings:ApiBaseUrl"] ?? "https://localhost:44375";
            var resetLink =
                $"{apiBaseUrl}/api/Auth/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset your password",
                $"You can reset your password by clicking this link: {resetLink}");

            return true;
        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                return await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword); //[6][4]
        }
    }
}
