using BussinessLogicLayer.DTOs.HistoryReward;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLogicLayer.IServices
{
    public interface IHistoryRewardService
    {
        // Read
        Task<IEnumerable<HistoryRewardDto>> GetAllAsync();
        Task<PagedResult<HistoryRewardDto>> GetPagedAsync(HistoryRewardQueryParams query);
        Task<HistoryRewardDto?> GetByIdAsync(int id);
        Task<IEnumerable<HistoryRewardDto>> GetByUserAsync(string userId);

        // Create / Redeem
        Task<HistoryRewardDto> CreateAsync(CreateHistoryRewardDto dto);
        Task<HistoryRewardDto> RedeemAsync(RedeemHistoryRewardDto dto);

        // Update / Status
        Task<bool> UpdateStatusAsync(int id, string newStatus);

        // Bulk / Delete
        Task BulkAddAsync(IEnumerable<CreateHistoryRewardDto> dtos);
        Task<bool> SoftDeleteAsync(int id);

        // Summaries / Validation
        Task<HistoryRewardSummaryDto> GetSummaryAsync(string userId);
        Task<bool> ValidateRedeemAsync(RedeemHistoryRewardDto dto);
    }
}