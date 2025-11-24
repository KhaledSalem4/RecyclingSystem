using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.IServices
{
    public interface IFactoryService
    {
        Task<IEnumerable<FactoryDto>> GetAllAsync();
        Task<FactoryDto?> GetByIdAsync(int id);
        Task AddAsync(FactoryDto dto);
        Task UpdateAsync(FactoryDto dto);
        Task DeleteAsync(int id);
    }
}
