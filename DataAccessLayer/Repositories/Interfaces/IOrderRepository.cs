using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetOrdersByCollectorIdAsync(string collectorId);
        Task<IEnumerable<Order>> GetOrdersByFactoryIdAsync(int factoryId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
    }
}
