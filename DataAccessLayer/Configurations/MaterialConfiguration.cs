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
    public class MaterialConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.HasKey(m => m.ID);
            
            builder.Property(m => m.TypeName).HasMaxLength(100);
            builder.Property(m => m.Size).HasMaxLength(50);
            builder.Property(m => m.Price).IsRequired().HasPrecision(18, 2);

            // Relationship with Factory
            builder.HasOne(m => m.Factory)
                   .WithMany()
                   .HasForeignKey(m => m.FactoryId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Many-to-Many relationship with Orders
            builder.HasMany(m => m.Orders)
                   .WithMany(o => o.Materials)
                   .UsingEntity<Dictionary<string, object>>(
                       "OrderMaterial",
                       j => j.HasOne<Order>().WithMany().HasForeignKey("OrderId"),
                       j => j.HasOne<Material>().WithMany().HasForeignKey("MaterialId"));
        }
    }
}
