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
            // Configure primary key
            builder.HasKey(hr => hr.ID);

            // Relationship: ApplicationUser → HistoryReward (one-to-many)
            builder.HasOne(hr => hr.User)
                   .WithMany(u => u.HistoryRewards)
                   .HasForeignKey(hr => hr.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Reward → HistoryReward (one-to-many)
            builder.HasOne(hr => hr.Reward)
                   .WithMany(r => r.HistoryRewards)
                   .HasForeignKey(hr => hr.RewardId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(hr => hr.RedeemedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(hr => hr.PointsUsed)
                   .IsRequired();

            builder.Property(hr => hr.Quantity)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(hr => hr.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .HasDefaultValue(RedemptionStatus.Pending);

            // Create index for faster queries
            builder.HasIndex(hr => hr.UserId);
            builder.HasIndex(hr => hr.RewardId);
            builder.HasIndex(hr => hr.Status);
        }
    }
}
