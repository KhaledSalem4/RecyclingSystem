using BussinessLogicLayer.DTOs.Reward;

namespace BusinessLogicLayer.IServices
{
    public interface IRewardService
    {
        Task<IEnumerable<RewardDto>> GetAllAsync();
        Task<RewardDto?> GetByIdAsync(int id);
        Task AddAsync(RewardDto dto);
        Task UpdateAsync(RewardDto dto);
        Task DeleteAsync(int id);
    }
}
