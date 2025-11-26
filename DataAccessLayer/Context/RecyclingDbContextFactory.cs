using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Context
{
    public class RecyclingDbContextFactory : IDesignTimeDbContextFactory<RecyclingDbContext>
    {
        public RecyclingDbContext CreateDbContext(string[] args)
        {
            // Build configuration to read from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../PresentationLayer"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Create DbContextOptionsBuilder
            var optionsBuilder = new DbContextOptionsBuilder<RecyclingDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new RecyclingDbContext(optionsBuilder.Options);
        }
    }
}
