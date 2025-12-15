using Microsoft.AspNetCore.Identity;
using RecyclingSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public String FullName
        {
            get; set;

        }
        public int Points
        {
            get; set;
        }
        
        // Address Information
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNo { get; set; }
        public string? Apartment { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public ICollection<HistoryReward> HistoryRewards { get; set; } = new HashSet<HistoryReward>();
        }
}
