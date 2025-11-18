using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class FactoryConfiguration : IEntityTypeConfiguration<Factory>
    {
        public void Configure(EntityTypeBuilder<Factory> builder)
        {
            builder.HasKey(f => f.ID);
            builder.Property(f => f.Name).IsRequired().HasMaxLength(100);
            builder.Property(f => f.Location).HasMaxLength(200);
            
            builder.HasMany(f => f.Orders)
                   .WithOne(o => o.Factory)
                   .HasForeignKey(o => o.FactoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
