using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.Impementations
{
    public class HistoryRewardRepository : GenericRepository<HistoryReward>, IHistoryRewardRepository
    {
        public HistoryRewardRepository(RecyclingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<HistoryReward>> GetUserRedemptionHistoryAsync(string userId)
        {
            return await _dbSet
                .Include(hr => hr.Reward)
                .Where(hr => hr.UserId == userId)
                .OrderByDescending(hr => hr.RedeemedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistoryReward>> GetRewardRedemptionHistoryAsync(int rewardId)
        {
            return await _dbSet
                .Include(hr => hr.User)
                .Where(hr => hr.RewardId == rewardId)
                .OrderByDescending(hr => hr.RedeemedAt)
                .ToListAsync();
        }

        public async Task<HistoryReward?> GetHistoryWithDetailsAsync(int historyId)
        {
            return await _dbSet
                .Include(hr => hr.User)
                .Include(hr => hr.Reward)
                .FirstOrDefaultAsync(hr => hr.ID == historyId);
        }
    }
}
