using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.DBContexts
{
    public class MoneyMindAuthDbContext : IdentityDbContext
    {
        public MoneyMindAuthDbContext(DbContextOptions<MoneyMindAuthDbContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var userRoleId = "04de6554-3c12-4968-a0aa-8fefb878ea4b";
            var managerRoleId = "ecac1613-163a-4aec-966d-822321d7db0b";
            var adminRoleId = "d066389c-1a19-45e3-9865-d94ff40750f9";
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId,
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id = managerRoleId,
                    ConcurrencyStamp = managerRoleId,
                    Name = "Manager",
                    NormalizedName = "Manager".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer(
        //            "YourConnectionStringHere",
        //            b => b.MigrationsAssembly("MoneyMind_DAL")); // Tên assembly chứa migrations
        //    }
        //}
    }
}
