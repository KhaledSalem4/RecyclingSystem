using AutoMapper;
using BusinessLogicLayer.IServices;
using BussinessLogicLayer.DTOs.Reward;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using RecyclingSystem.DataAccess.Entities;

namespace BusinessLogicLayer.Services
{
    public class RewardService : IRewardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RewardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Basic CRUD Operations
        public async Task<IEnumerable<RewardDto>> GetAllAsync()
        {
            var rewards = await _unitOfWork.Rewards.GetAllAsync();
            return _mapper.Map<IEnumerable<RewardDto>>(rewards);
        }

        public async Task<RewardDto?> GetByIdAsync(int id)
        {
            var reward = await _unitOfWork.Rewards.GetByIdAsync(id);
            return reward == null ? null : _mapper.Map<RewardDto>(reward);
        }

        public async Task<RewardDto> AddAsync(CreateRewardDto dto)
        {
            // Check for duplicate name
            if (await _unitOfWork.Rewards.RewardExistsByNameAsync(dto.Name))
            {
                throw new InvalidOperationException($"Reward with name '{dto.Name}' already exists.");
            }

            var reward = _mapper.Map<Reward>(dto);
            reward.IsAvailable = true;

            await _unitOfWork.Rewards.AddRewardAsync(reward);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RewardDto>(reward);
        }

        public async Task<RewardDto> UpdateAsync(UpdateRewardDto dto)
        {
            // Check for duplicate name (excluding current reward)
            if (await _unitOfWork.Rewards.RewardExistsByNameAsync(dto.Name, dto.ID))
            {
                throw new InvalidOperationException($"Another reward with name '{dto.Name}' already exists.");
            }

            var reward = _mapper.Map<Reward>(dto);
            await _unitOfWork.Rewards.UpdateAsync(reward);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RewardDto>(reward);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _unitOfWork.Rewards.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // User-facing Features
        public async Task<IEnumerable<RewardDto>> GetAvailableRewardsForUserAsync(string userId)
        {
            var user = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.UserId == userId);
            var userPoints = user?.User?.Points ?? 0;

            var rewards = await _unitOfWork.Rewards.GetAvailableRewardsForUserAsync(userPoints);
            return _mapper.Map<IEnumerable<RewardDto>>(rewards);
        }

        public async Task<IEnumerable<RewardDto>> GetRewardsByCategoryAsync(string category, string? userId = null)
        {
            int? userPoints = null;

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.UserId == userId);
                userPoints = user?.User?.Points ?? 0;
            }

            var rewards = await _unitOfWork.Rewards.GetRewardsByCategoryAsync(category, userPoints);
            return _mapper.Map<IEnumerable<RewardDto>>(rewards);
        }

        public async Task<IEnumerable<RewardDto>> GetPopularRewardsAsync(int topCount = 10)
        {
            var rewards = await _unitOfWork.Rewards.GetPopularRewardsAsync(topCount);
            return _mapper.Map<IEnumerable<RewardDto>>(rewards);
        }

        public async Task<IEnumerable<RewardDto>> SearchRewardsAsync(string searchTerm, string? userId = null)
        {
            int? userPoints = null;

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.UserId == userId);
                userPoints = user?.User?.Points ?? 0;
            }

            var rewards = await _unitOfWork.Rewards.SearchRewardsAsync(searchTerm, userPoints);
            return _mapper.Map<IEnumerable<RewardDto>>(rewards);
        }

        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Rewards.GetAllCategoriesAsync();
        }

        // Redemption Logic
        public async Task<bool> RedeemRewardAsync(string userId, RedeemRewardDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get user with points
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException("User not found");

                // Get reward
                var reward = await _unitOfWork.Rewards.GetRewardForRedemptionAsync(dto.RewardId);
                if (reward == null)
                    throw new InvalidOperationException("Reward is not available");

                // Validate stock
                if (reward.StockQuantity < dto.Quantity)
                    throw new InvalidOperationException("Insufficient stock");

                // Calculate points
                int totalPointsNeeded = reward.RequiredPoints * dto.Quantity;

                // Validate user points
                if (user.Points < totalPointsNeeded)
                    throw new InvalidOperationException("Insufficient points");

                // ✅ Deduct points from user
                user.Points -= totalPointsNeeded;
                _unitOfWork.Users.Update(user);

                // Deduct stock
                await _unitOfWork.Rewards.UpdateStockQuantityAsync(dto.RewardId, -dto.Quantity);

                // Create redemption history
                var historyReward = new HistoryReward
                {
                    UserId = userId,
                    RewardId = dto.RewardId,
                    PointsUsed = totalPointsNeeded,
                    Quantity = dto.Quantity,
                    RedeemedAt = DateTime.UtcNow,
                    Status = RedemptionStatus.Pending
                };

                await _unitOfWork.HistoryRewards.AddAsync(historyReward);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        // Admin Features
        public async Task<IEnumerable<RewardDto>> GetLowStockRewardsAsync(int threshold = 10)
        {
            var rewards = await _unitOfWork.Rewards.GetLowStockRewardsAsync(threshold);
            return _mapper.Map<IEnumerable<RewardDto>>(rewards);
        }

        public async Task<RewardWithStatsDto?> GetRewardWithStatsAsync(int rewardId)
        {
            var reward = await _unitOfWork.Rewards.GetRewardWithHistoryAsync(rewardId);
            if (reward == null)
                return null;

            var dto = _mapper.Map<RewardWithStatsDto>(reward);
            dto.TotalRedemptions = reward.HistoryRewards.Count;
            dto.PendingRedemptions = reward.HistoryRewards.Count(hr => hr.Status == RedemptionStatus.Pending);

            return dto;
        }

        public async Task<bool> UpdateStockAsync(int rewardId, int quantityChange)
        {
            var result = await _unitOfWork.Rewards.UpdateStockQuantityAsync(rewardId, quantityChange);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return result;
        }
    }
}
