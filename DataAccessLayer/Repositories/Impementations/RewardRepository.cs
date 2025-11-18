using DataAccessLayer.Context;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using RecyclingSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Impementations
{
    public class RewardRepository : GenericRepository<Reward>, IRewardRepository
    {
        public RewardRepository(RecyclingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Reward>> GetAvailableRewardsForUserAsync(int userPoints)
        {
            return await _dbSet
                .Where(r => r.RequiredPoints <= userPoints)
                .OrderBy(r => r.RequiredPoints)
                .ToListAsync();
        }

        public async Task<Reward?> GetRewardWithHistoryAsync(int rewardId)
        {
            return await _dbSet
                .Include(r => r.HistoryReward)
                    .ThenInclude(hr => hr.User)
                .FirstOrDefaultAsync(r => r.ID == rewardId);
        }
    }
}
