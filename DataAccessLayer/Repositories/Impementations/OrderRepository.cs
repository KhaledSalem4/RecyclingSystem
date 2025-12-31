using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Impementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(RecyclingDbContext context) : base(context)
        {
        }

        // ✅ OVERRIDE GetAllAsync to include related entities
        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.Collector)
                .Include(o => o.Factory)
                .Include(o => o.Materials)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.Collector)
                .Include(o => o.Factory)
                .Include(o => o.Materials)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCollectorIdAsync(string collectorId)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.Collector)
                .Include(o => o.Factory)
                .Include(o => o.Materials)
                .Where(o => o.CollectorId == collectorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByFactoryIdAsync(int factoryId)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.Collector)
                .Include(o => o.Factory)
                .Include(o => o.Materials)
                .Where(o => o.FactoryId == factoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .Include(o => o.User)
                .Include(o => o.Collector)
                .Include(o => o.Factory)
                .Include(o => o.Materials)
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        // ✅ FIX: Corrected GetOrderWithDetailsAsync
        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.User)        // ✅ Fixed: Just include the entity
                .Include(o => o.Collector)   // ✅ Fixed: Just include the entity
                .Include(o => o.Factory)     // ✅ Fixed: Just include the entity
                .Include(o => o.Materials)   // ✅ Fixed: Include materials collection
                .FirstOrDefaultAsync(o => o.ID == orderId);
        }
    }
}
