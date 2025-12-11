using BussinessLogicLayer.DTOs.Reward;

namespace BusinessLogicLayer.IServices
{
    public interface IRewardService
    {
        // Basic CRUD
        Task<IEnumerable<RewardDto>> GetAllAsync();
        Task<RewardDto?> GetByIdAsync(int id);
        Task<RewardDto> AddAsync(CreateRewardDto dto);
        Task<RewardDto> UpdateAsync(UpdateRewardDto dto);
        Task<bool> DeleteAsync(int id);

        // User-facing features
        Task<IEnumerable<RewardDto>> GetAvailableRewardsForUserAsync(string userId);
        Task<IEnumerable<RewardDto>> GetRewardsByCategoryAsync(string category, string? userId = null);
        Task<IEnumerable<RewardDto>> GetPopularRewardsAsync(int topCount = 10);
        Task<IEnumerable<RewardDto>> SearchRewardsAsync(string searchTerm, string? userId = null);
        Task<IEnumerable<string>> GetAllCategoriesAsync();

        // Redemption
        Task<bool> RedeemRewardAsync(string userId, RedeemRewardDto dto);

        // Admin features
        Task<IEnumerable<RewardDto>> GetLowStockRewardsAsync(int threshold = 10);
        Task<RewardWithStatsDto?> GetRewardWithStatsAsync(int rewardId);
        Task<bool> UpdateStockAsync(int rewardId, int quantityChange);
    }
}
