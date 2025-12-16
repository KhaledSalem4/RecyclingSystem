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
        
        Task<OrderDto> AddAsync(CreateOrderDto dto);
        Task UpdateAsync(OrderDto dto);
        Task DeleteAsync(int id);
        
        // Points-related methods
        Task<bool> CompleteOrderAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId);
        Task<bool> UserCancelOrderAsync(int orderId, string userId);

        // Collector-related methods (only self-assignment)
        Task<bool> CollectorAcceptOrderAsync(int orderId, string collectorId);
        Task<bool> CollectorUpdateOrderStatusAsync(int orderId, string collectorId, string newStatus);
        Task<IEnumerable<OrderDto>> GetAvailableOrdersForCollectorsAsync();
        Task<IEnumerable<OrderDto>> GetMyOrdersAsCollectorAsync(string collectorId);
    }
}
