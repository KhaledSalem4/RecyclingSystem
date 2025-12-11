using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.Impementations
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly RecyclingDbContext _context;
        private readonly DbSet<ApplicationUser> _dbSet;

        public ApplicationUserRepository(RecyclingDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<ApplicationUser>();
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public void Update(ApplicationUser user)
        {
            _dbSet.Update(user);
        }

        public async Task<bool> AnyAsync(string id)
        {
            return await _dbSet.AnyAsync(u => u.Id == id);
        }
    }
}
