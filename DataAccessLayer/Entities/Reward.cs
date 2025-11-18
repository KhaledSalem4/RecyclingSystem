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

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int RequiredPoints { get; set; }

        public ICollection<HistoryReward> HistoryReward { get; set; } = new HashSet<HistoryReward>();
    }
}
