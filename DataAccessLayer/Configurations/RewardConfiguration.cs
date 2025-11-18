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

            builder.Property(r => r.Title)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.Description)
                   .HasMaxLength(300);

            builder.Property(r => r.RequiredPoints)
                   .IsRequired();
        }
    }
}

