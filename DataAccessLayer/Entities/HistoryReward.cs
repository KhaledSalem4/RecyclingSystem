using RecyclingSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class HistoryReward
    {
       public int ID { get; set; }
        // Foreign key to ApplicationUser
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Foreign key to Reward
        public int RewardId { get; set; }
        public Reward Reward { get; set; }

        public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;
        public int PointsUsed { get; set; }
        public int Quantity { get; set; }
        public RedemptionStatus Status { get; set; } = RedemptionStatus.Pending;

    }
    public enum RedemptionStatus
    {
        Pending = 0,
        Approved = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4
    }
}
