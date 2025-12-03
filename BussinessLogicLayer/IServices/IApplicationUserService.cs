using BussinessLogicLayer.DTOs.AppUser;
namespace BusinessLogicLayer.IServices;

    public interface IApplicationUserService
    {
        Task<IEnumerable<ApplicationUserDto>> GetAllAsync();
        Task<ApplicationUserDto?> GetByIdAsync(string id);
        Task UpdateAsync(ApplicationUserDto dto);
    }

