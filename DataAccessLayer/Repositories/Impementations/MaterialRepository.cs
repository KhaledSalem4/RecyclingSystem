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
    public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(RecyclingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Material>> GetMaterialsByFactoryIdAsync(int factoryId)
        {
            return await _dbSet
                .Include(m => m.Factory)
                .Where(m => m.FactoryId == factoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Material>> GetMaterialsByTypeAsync(string typeName)
        {
            return await _dbSet
                .Include(m => m.Factory)
                .Where(m => m.TypeName == typeName)
                .ToListAsync();
        }

        public async Task<Material?> GetMaterialWithOrdersAsync(int materialId)
        {
            return await _dbSet
                .Include(m => m.Factory)
                .Include(m => m.Orders)
                .FirstOrDefaultAsync(m => m.ID == materialId);
        }

    }
}
