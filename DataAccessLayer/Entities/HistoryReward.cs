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

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int RewardId { get; set; }
        public Reward Reward { get; set; }

        public DateTime ClaimedAt { get; set; } = DateTime.UtcNow;

        public int PointsUsed { get; set; }
    }
}
