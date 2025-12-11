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
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // 🔗 رابط Angular بدل API
            var frontEndUrl = _config["AppSettings:FrontEndUrl"] ?? "http://localhost:4200";
            var confirmationLink =
                $"{frontEndUrl}/confirm-email?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Confirm Your Email</title>
</head>
<body style='font-family: Arial, sans-serif; background-color:#f4f4f4; margin:0; padding:0;'>

    <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width:600px; background:white; border-radius:10px; margin-top:40px; box-shadow:0 4px 8px rgba(0,0,0,0.1);'>
        <tr>
            <td style='padding:30px 25px; text-align:center;'>
                <h2 style='color:#0bb56b; margin-bottom:10px;'>Confirm Your Email</h2>
                <p style='font-size:15px; color:#555; margin:0;'>Hi {user.FullName},</p>
                <p style='font-size:15px; color:#555; margin-top:10px; line-height:1.6;'>
                    Thanks for joining <b>Recycling System</b>.<br/>
                    Please confirm your email address to activate your account.
                </p>

                <a href='{confirmationLink}' 
                   style='display:inline-block; margin-top:25px; background:#0bb56b; color:white;
                          padding:12px 24px; border-radius:5px; text-decoration:none; font-size:16px;'>
                    Confirm Email
                </a>

                <p style='font-size:13px; color:#777; margin-top:30px;'>
                    If the button doesn't work, copy and paste this link:<br/>
                    <span style='color:#0bb56b;'>{confirmationLink}</span>
                </p>
            </td>
        </tr>

        <tr>
            <td style='text-align:center; padding:15px 0; font-size:13px; color:#aaa; background:#fafafa; border-radius:0 0 10px 10px;'>
                © {DateTime.UtcNow.Year} Recycling System. All rights reserved.
            </td>
        </tr>
    </table>

</body>
</html>
";

            // Send email
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm Your Email",
                emailBody);
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
                return false;

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 👇 استخدم رابط Angular بدلاً من API
            var frontEndUrl = _config["AppSettings:FrontEndUrl"] ?? "http://localhost:4200";
            var resetLink =
                $"{frontEndUrl}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
            var emailBody = $@"
<body style='margin:0; padding:0; background:#f6f6f6; font-family:Arial, sans-serif;'>

  <div style='max-width:600px; margin:auto; padding:20px;'>

    <div style='background:#0bb56b; color:white; padding:18px; text-align:center; border-radius:8px 8px 0 0;'>
      <h2 style='margin:0;'>🔐 Reset Your Password</h2>
    </div>

    <div style='background:white; padding:25px; border-radius:0 0 8px 8px;'>

      <p style='font-size:15px; color:#555; margin:0;'>Hi {user.FullName},</p>
      <p>You requested to reset your password for your <b>Recycling System</b> account.</p>
      <p>Click the button below to create a new password:</p>

      <p style='text-align:center;'>
        <a href='{resetLink}'
          style='background:#0bb56b; color:white; text-decoration:none; padding:12px 22px;
                 display:inline-block; font-size:16px; border-radius:6px;'>
          Reset Password
        </a>
      </p>

      <p style='margin-top:25px;'>If you did not request this, please ignore this email.</p>

      <hr style='border:none; border-top:1px solid #ddd; margin-top:30px;' />

      <p style='font-size:13px; color:#888; text-align:center;'>
        © {DateTime.UtcNow.Year} Recycling System — All rights reserved.
      </p>
    </div>
  </div>

</body>";


            await _emailService.SendEmailAsync(
                user.Email,
                "Reset your password",
                emailBody);

            return true;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        }

    }
}
