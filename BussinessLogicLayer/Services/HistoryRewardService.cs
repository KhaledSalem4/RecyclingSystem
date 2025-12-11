using BussinessLogicLayer.DTOs.HistoryReward;
using BussinessLogicLayer.IServices;
using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BussinessLogicLayer.Services
{
    public class HistoryRewardService : IHistoryRewardService
    {
        private readonly RecyclingDbContext _context;

        public HistoryRewardService(RecyclingDbContext context)
        {
            _context = context;
        }

        // Read
        public async Task<IEnumerable<HistoryRewardDto>> GetAllAsync()
        {
            return await _context.HistoryRewards
                .AsNoTracking()
                .Include(hr => hr.Reward)
                .Select(hr => new HistoryRewardDto
                {
                    ID = hr.ID,
                    UserId = hr.UserId,
                    RewardId = hr.RewardId,
                    RewardName = hr.Reward != null ? hr.Reward.Name : null,
                    RedeemedAt = hr.RedeemedAt,
                    PointsUsed = hr.PointsUsed,
                    Quantity = hr.Quantity,
                    Status = hr.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<PagedResult<HistoryRewardDto>> GetPagedAsync(HistoryRewardQueryParams query)
        {
            var q = _context.HistoryRewards
                .AsNoTracking()
                .Include(hr => hr.Reward)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.UserId))
                q = q.Where(x => x.UserId == query.UserId);

            if (query.RewardId.HasValue)
                q = q.Where(x => x.RewardId == query.RewardId.Value);

            if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<RedemptionStatus>(query.Status, true, out var s))
                q = q.Where(x => x.Status == s);

            if (query.From.HasValue)
                q = q.Where(x => x.RedeemedAt >= query.From.Value);

            if (query.To.HasValue)
                q = q.Where(x => x.RedeemedAt <= query.To.Value);

            // Sorting (simple)
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("RedeemedAt", StringComparison.OrdinalIgnoreCase))
                    q = query.SortDescending ? q.OrderByDescending(x => x.RedeemedAt) : q.OrderBy(x => x.RedeemedAt);
                else if (query.SortBy.Equals("PointsUsed", StringComparison.OrdinalIgnoreCase))
                    q = query.SortDescending ? q.OrderByDescending(x => x.PointsUsed) : q.OrderBy(x => x.PointsUsed);
                else
                    q = q.OrderByDescending(x => x.RedeemedAt);
            }
            else
            {
                q = q.OrderByDescending(x => x.RedeemedAt);
            }

            var total = await q.CountAsync();
            var page = Math.Max(1, query.Page);
            var pageSize = Math.Max(1, query.PageSize);
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(hr => new HistoryRewardDto
                {
                    ID = hr.ID,
                    UserId = hr.UserId,
                    RewardId = hr.RewardId,
                    RewardName = hr.Reward != null ? hr.Reward.Name : null,
                    RedeemedAt = hr.RedeemedAt,
                    PointsUsed = hr.PointsUsed,
                    Quantity = hr.Quantity,
                    Status = hr.Status.ToString()
                })
                .ToListAsync();

            return new PagedResult<HistoryRewardDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<HistoryRewardDto?> GetByIdAsync(int id)
        {
            var hr = await _context.HistoryRewards
                .AsNoTracking()
                .Include(x => x.Reward)
                .FirstOrDefaultAsync(x => x.ID == id);

            if (hr == null) return null;

            return new HistoryRewardDto
            {
                ID = hr.ID,
                UserId = hr.UserId,
                RewardId = hr.RewardId,
                RewardName = hr.Reward?.Name,
                RedeemedAt = hr.RedeemedAt,
                PointsUsed = hr.PointsUsed,
                Quantity = hr.Quantity,
                Status = hr.Status.ToString()
            };
        }

        public async Task<IEnumerable<HistoryRewardDto>> GetByUserAsync(string userId)
        {
            return await _context.HistoryRewards
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Reward)
                .OrderByDescending(x => x.RedeemedAt)
                .Select(hr => new HistoryRewardDto
                {
                    ID = hr.ID,
                    UserId = hr.UserId,
                    RewardId = hr.RewardId,
                    RewardName = hr.Reward != null ? hr.Reward.Name : null,
                    RedeemedAt = hr.RedeemedAt,
                    PointsUsed = hr.PointsUsed,
                    Quantity = hr.Quantity,
                    Status = hr.Status.ToString()
                })
                .ToListAsync();
        }

        // Create / Redeem
        public async Task<HistoryRewardDto> CreateAsync(CreateHistoryRewardDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var reward = await _context.Rewards.FirstOrDefaultAsync(r => r.ID == dto.RewardId);
            if (reward == null) throw new InvalidOperationException("Reward not found.");

            var history = new HistoryReward
            {
                UserId = dto.UserId,
                RewardId = dto.RewardId,
                Quantity = Math.Max(1, dto.Quantity),
                PointsUsed = reward.RequiredPoints * Math.Max(1, dto.Quantity),
                RedeemedAt = DateTime.UtcNow,
                Status = RedemptionStatus.Approved
            };

            _context.HistoryRewards.Add(history);
            await _context.SaveChangesAsync();

            return new HistoryRewardDto
            {
                ID = history.ID,
                UserId = history.UserId,
                RewardId = history.RewardId,
                RewardName = reward.Name,
                RedeemedAt = history.RedeemedAt,
                PointsUsed = history.PointsUsed,
                Quantity = history.Quantity,
                Status = history.Status.ToString()
            };
        }

        public async Task<HistoryRewardDto> RedeemAsync(RedeemHistoryRewardDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be at least 1.", nameof(dto));

            // Validate prereqs
            var validation = await ValidateRedeemAsync(dto);
            if (!validation)
                throw new InvalidOperationException("Redeem validation failed (insufficient points, stock, or reward unavailable).");

            var user = await _context.ApplicationUsers.FirstAsync(u => u.Id == dto.UserId);
            var reward = await _context.Rewards.FirstAsync(r => r.ID == dto.RewardId);
            var totalPoints = reward.RequiredPoints * dto.Quantity;

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Deduct user points
                user.Points -= totalPoints;
                _context.ApplicationUsers.Update(user);

                // Decrement stock
                reward.StockQuantity -= dto.Quantity;
                if (reward.StockQuantity <= 0)
                {
                    reward.StockQuantity = Math.Max(0, reward.StockQuantity);
                    reward.IsAvailable = false;
                }
                _context.Rewards.Update(reward);

                // Create history record with Pending status
                var history = new HistoryReward
                {
                    UserId = dto.UserId,
                    RewardId = dto.RewardId,
                    Quantity = dto.Quantity,
                    PointsUsed = totalPoints,
                    RedeemedAt = DateTime.UtcNow,
                    Status = RedemptionStatus.Pending
                };

                _context.HistoryRewards.Add(history);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return new HistoryRewardDto
                {
                    ID = history.ID,
                    UserId = history.UserId,
                    RewardId = history.RewardId,
                    RewardName = reward.Name,
                    RedeemedAt = history.RedeemedAt,
                    PointsUsed = history.PointsUsed,
                    Quantity = history.Quantity,
                    Status = history.Status.ToString()
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // Update / Status
        public async Task<bool> UpdateStatusAsync(int id, string newStatus)
        {
            var hr = await _context.HistoryRewards.FirstOrDefaultAsync(x => x.ID == id);
            if (hr == null) return false;

            if (!Enum.TryParse<RedemptionStatus>(newStatus, true, out var parsed))
                return false;

            // Basic transition validation could be added here
            hr.Status = parsed;
            _context.HistoryRewards.Update(hr);
            await _context.SaveChangesAsync();
            return true;
        }

        // Bulk / Delete
        public async Task BulkAddAsync(IEnumerable<CreateHistoryRewardDto> dtos)
        {
            if (dtos == null) return;

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var list = new List<HistoryReward>();
                foreach (var dto in dtos)
                {
                    var reward = await _context.Rewards.FirstOrDefaultAsync(r => r.ID == dto.RewardId);
                    var points = reward != null ? reward.RequiredPoints * Math.Max(1, dto.Quantity) : 0;
                    list.Add(new HistoryReward
                    {
                        UserId = dto.UserId,
                        RewardId = dto.RewardId,
                        Quantity = Math.Max(1, dto.Quantity),
                        PointsUsed = points,
                        RedeemedAt = DateTime.UtcNow,
                        Status = RedemptionStatus.Approved
                    });
                }

                _context.HistoryRewards.AddRange(list);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var hr = await _context.HistoryRewards.FirstOrDefaultAsync(x => x.ID == id);
            if (hr == null) return false;

            // No soft-delete flag present on entity; perform logical cancelation by setting status.
            hr.Status = RedemptionStatus.Cancelled;
            _context.HistoryRewards.Update(hr);
            await _context.SaveChangesAsync();
            return true;
        }

        // Summaries / Validation
        public async Task<HistoryRewardSummaryDto> GetSummaryAsync(string userId)
        {
            var q = _context.HistoryRewards.AsNoTracking().Where(x => x.UserId == userId);
            var totalCount = await q.CountAsync();
            var totalPoints = await q.SumAsync(x => (int?)x.PointsUsed) ?? 0;

            return new HistoryRewardSummaryDto
            {
                UserId = userId,
                TotalRedemptions = totalCount,
                TotalPointsUsed = totalPoints
            };
        }

        public async Task<bool> ValidateRedeemAsync(RedeemHistoryRewardDto dto)
        {
            if (dto == null) return false;
            if (dto.Quantity <= 0) return false;

            var user = await _context.ApplicationUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null) return false;

            var reward = await _context.Rewards.AsNoTracking().FirstOrDefaultAsync(r => r.ID == dto.RewardId);
            if (reward == null) return false;
            if (!reward.IsAvailable) return false;
            if (reward.StockQuantity < dto.Quantity) return false;

            var required = reward.RequiredPoints * dto.Quantity;
            if (user.Points < required) return false;

            return true;
        }
    }
}