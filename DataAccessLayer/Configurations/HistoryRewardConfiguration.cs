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

            builder.HasKey(ur => ur.ID);

            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.HistoryRewards)
                   .HasForeignKey(ur => ur.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.Reward)
                   .WithMany(r => r.HistoryReward)
                   .HasForeignKey(ur => ur.RewardId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ur => ur.PointsUsed)
                   .IsRequired();
        }
    }
}
