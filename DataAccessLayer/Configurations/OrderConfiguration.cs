using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.ID);
            builder.Property(o => o.OrderDate).IsRequired();
            
            // Configure Status as enum stored as string in database
            builder.Property(o => o.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            // User who placed the order
            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Collector who handles the order
            builder.HasOne(o => o.Collector)
                   .WithMany()
                   .HasForeignKey(o => o.CollectorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Factory where materials are delivered
            builder.HasOne(o => o.Factory)
                   .WithMany(f => f.Orders)
                   .HasForeignKey(o => o.FactoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many relationship with Materials
            builder.HasMany(o => o.Materials)
                   .WithMany(m => m.Orders)
                   .UsingEntity<Dictionary<string, object>>(
                        "OrderMaterial",
                        j => j.HasOne<Material>().WithMany().HasForeignKey("MaterialId"),
                        j => j.HasOne<Order>().WithMany().HasForeignKey("OrderId"));
        }
    }
}
