using BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterUserDto dto, string role);
        Task<SignInResult> LoginAsync(LoginUserDto dto);
    }
}
