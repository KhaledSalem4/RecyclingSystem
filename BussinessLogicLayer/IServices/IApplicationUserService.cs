using BussinessLogicLayer.DTOs.AppUser;

namespace BusinessLogicLayer.IServices
{
    public interface IApplicationUserService
    {
        // Existing methods
        Task<IEnumerable<ApplicationUserDto>> GetAllAsync();
        Task<ApplicationUserDto?> GetByIdAsync(string id);
        Task UpdateAsync(ApplicationUserDto dto);

        // New methods for user profile management
        Task<UpdateUserProfileDto?> GetUserProfileAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, UpdateUserDto dto);
    }
}

