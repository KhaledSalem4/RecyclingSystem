using BussinessLogicLayer.DTOs.Order;

namespace BusinessLogicLayer.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<OrderDto>> GetOrdersByCollectorIdAsync(string collectorId);
        Task<IEnumerable<OrderDto>> GetOrdersByFactoryIdAsync(int factoryId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);

        Task AddAsync(OrderDto dto);
        Task UpdateAsync(OrderDto dto);
        Task DeleteAsync(int id);
    }
}
