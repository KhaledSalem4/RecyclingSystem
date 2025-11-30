using BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterUserDto dto, string role);
        Task<string> LoginAndGenerateTokenAsync(LoginUserDto dto);
        Task<IdentityResult> ConfirmEmailAsync(string email, string token);

    }
}
