using RecyclingSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IRewardRepository : IGenericRepository<Reward>
    {
        Task<IEnumerable<Reward>> GetAvailableRewardsForUserAsync(int userPoints);
        Task<Reward?> GetRewardWithHistoryAsync(int rewardId);
    }
}
