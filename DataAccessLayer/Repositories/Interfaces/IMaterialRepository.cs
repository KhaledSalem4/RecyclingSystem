using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IMaterialRepository : IGenericRepository<Material>
    {
        Task<IEnumerable<Material>> GetMaterialsByTypeAsync(string typeName);
        Task<Material?> GetMaterialWithOrdersAsync(int materialId);
    }
}
