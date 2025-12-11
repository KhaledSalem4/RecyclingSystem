using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer.DTOs.HistoryReward
{
    public class HistoryRewardSummaryDto
    {
        public string UserId { get; set; } = string.Empty;
        public int TotalRedemptions { get; set; }
        public int TotalPointsUsed { get; set; }
    }
}
