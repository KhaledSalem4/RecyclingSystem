using DataAccessLayer.Entities;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IHistoryRewardRepository : IGenericRepository<HistoryReward>
    {
        Task<IEnumerable<HistoryReward>> GetUserRedemptionHistoryAsync(string userId);
        Task<IEnumerable<HistoryReward>> GetRewardRedemptionHistoryAsync(int rewardId);
        Task<HistoryReward?> GetHistoryWithDetailsAsync(int historyId);
    }
}
