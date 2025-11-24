using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.IServices
{
    public interface IHistoryRewardService
    {
        Task<IEnumerable<HistoryRewardDto>> GetAllAsync();
        Task AddAsync(HistoryRewardDto dto);
    }
}
