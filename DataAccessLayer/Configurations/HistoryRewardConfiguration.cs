using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecyclingSystem.DataAccess.Entities;

namespace RecyclingSystem.DataAccess.Configurations
{
    public class HistoryRewardConfiguration : IEntityTypeConfiguration<HistoryReward>
    {
        public void Configure(EntityTypeBuilder<HistoryReward> builder)
        {
            // Configure composite primary key
            builder.HasKey(hr => new { hr.UserId, hr.RewardId });

            builder.HasOne(hr => hr.User)
                   .WithMany(u => u.HistoryRewards)
                   .HasForeignKey(hr => hr.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(hr => hr.Reward)
                   .WithMany(r => r.HistoryReward)
                   .HasForeignKey(hr => hr.RewardId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(hr => hr.ClaimedAt)
                   .IsRequired();

            builder.Property(hr => hr.PointsUsed)
                   .IsRequired();
        }
    }
}
