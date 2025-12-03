using BussinessLogicLayer.DTOs.Factory;

namespace BusinessLogicLayer.IServices
{
    public interface IFactoryService
    {
        // Basic CRUD
        Task<IEnumerable<FactoryDto>> GetAllFactoriesAsync();
        Task<FactoryDto?> GetFactoryByIdAsync(int id);
        Task<FactoryDto> CreateFactoryAsync(CreateFactoryDto dto);
        Task<FactoryDto> UpdateFactoryAsync(UpdateFactoryDto dto);
        Task<bool> DeleteFactoryAsync(int id);

        // Factory with orders
        Task<FactoryDetailsDto?> GetFactoryWithOrdersAsync(int id);
    }
}
