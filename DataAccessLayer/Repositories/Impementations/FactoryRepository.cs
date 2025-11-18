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
    public class FactoryRepository : GenericRepository<Factory>, IFactoryRepository
    {
        public FactoryRepository(RecyclingDbContext context) : base(context)
        {
        }

        public async Task<Factory?> GetFactoryWithOrdersAsync(int factoryId)
        {
            return await _dbSet
                .Include(f => f.Orders)
                    .ThenInclude(o => o.User)
                .Include(f => f.Orders)
                    .ThenInclude(o => o.Collector)
                .Include(f => f.Orders)
                    .ThenInclude(o => o.Materials)
                .FirstOrDefaultAsync(f => f.ID == factoryId);
        }

        public async Task<IEnumerable<Factory>> GetFactoriesByLocationAsync(string location)
        {
            return await _dbSet
                .Where(f => f.Location.Contains(location))
                .ToListAsync();
        }
    }
}
