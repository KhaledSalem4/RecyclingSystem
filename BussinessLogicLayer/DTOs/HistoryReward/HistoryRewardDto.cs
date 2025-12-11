namespace BussinessLogicLayer.DTOs.HistoryReward
{
    public class HistoryRewardDto
    {
        public int ID { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int RewardId { get; set; }
        public string? RewardName { get; set; }
        public DateTime RedeemedAt { get; set; }
        public int PointsUsed { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
