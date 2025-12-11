using DataAccessLayer.Entities;
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
        // Query methods
        Task<IEnumerable<Reward>> GetAvailableRewardsForUserAsync(int userPoints);
        Task<Reward?> GetRewardWithHistoryAsync(int rewardId);
        Task<Reward?> GetRewardForRedemptionAsync(int rewardId);
        Task<IEnumerable<Reward>> GetRewardsByCategoryAsync(string category, int? userPoints = null);
        Task<IEnumerable<Reward>> GetPopularRewardsAsync(int topCount = 10);
        Task<IEnumerable<Reward>> GetLowStockRewardsAsync(int threshold = 10);
        Task<IEnumerable<Reward>> SearchRewardsAsync(string searchTerm, int? userPoints = null);
        Task<IEnumerable<Reward>> GetRewardsInPointRangeAsync(int minPoints, int maxPoints);
        Task<IEnumerable<HistoryReward>> GetUserRedemptionHistoryAsync(string userId);
        Task<IEnumerable<Reward>> GetAlmostAffordableRewardsAsync(int userPoints, int pointsGap = 50);
        Task<IEnumerable<string>> GetAllCategoriesAsync();

        // Command methods
        Task AddRewardAsync(Reward reward);
        Task UpdateAsync(Reward reward);
        Task DeleteAsync(int id);
        Task<bool> UpdateStockQuantityAsync(int rewardId, int quantityChange);

        Task<bool> RewardExistsByNameAsync(string name, int? excludeId = null);
    }
}
