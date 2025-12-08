using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecyclingSystem.DataAccess.Entities;

namespace RecyclingSystem.DataAccess.Configurations
{
    public class RewardConfiguration : IEntityTypeConfiguration<Reward>
    {
        public void Configure(EntityTypeBuilder<Reward> builder)
        {
            builder.HasKey(r => r.ID);

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.Description)
                   .HasMaxLength(500);

            builder.Property(r => r.Category)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(r => r.RequiredPoints)
                   .IsRequired();

            builder.Property(r => r.StockQuantity)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(r => r.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(r => r.ImageUrl)
                   .HasMaxLength(500);

            // Relationship: Reward → HistoryReward (one-to-many)
            builder.HasMany(r => r.HistoryRewards)
                   .WithOne(hr => hr.Reward)
                   .HasForeignKey(hr => hr.RewardId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

