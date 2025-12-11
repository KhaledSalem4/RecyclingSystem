using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer.DTOs.Reward
{
    public class RewardWithStatsDto : RewardDto
    {
        public int TotalRedemptions { get; set; }
        public int PendingRedemptions { get; set; }
    }
}
