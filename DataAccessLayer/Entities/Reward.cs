using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecyclingSystem.DataAccess.Entities
{
    public class Reward
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category {get; set; }
        public int RequiredPoints { get; set; }
        public int StockQuantity { get; set; }

        public bool IsAvailable { get; set; } = true;
        public string? ImageUrl { get; set; }



        public ICollection<HistoryReward> HistoryRewards { get; set; } = new HashSet<HistoryReward>();
    }
}
