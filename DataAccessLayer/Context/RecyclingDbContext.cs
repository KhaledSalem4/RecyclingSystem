using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecyclingSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Context
{
    public class RecyclingDbContext : IdentityDbContext<ApplicationUser>
    {
        public RecyclingDbContext(DbContextOptions<RecyclingDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<HistoryReward> HistoryRewards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
