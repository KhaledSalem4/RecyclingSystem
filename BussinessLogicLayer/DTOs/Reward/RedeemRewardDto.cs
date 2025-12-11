using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer.DTOs.Reward
{
    public class RedeemRewardDto
    {
        public int RewardId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
