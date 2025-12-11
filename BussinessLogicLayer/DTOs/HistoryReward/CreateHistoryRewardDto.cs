using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer.DTOs.HistoryReward
{
    public class CreateHistoryRewardDto
    {
        public string UserId { get; set; } = string.Empty;
        public int RewardId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
