using Microsoft.EntityFrameworkCore;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.DBContexts
{
    public class MoneyMindDbContext : DbContext
    {
        public MoneyMindDbContext()
        {
        }

        public MoneyMindDbContext(DbContextOptions<MoneyMindDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Jar> Jars { get; set; }
        public virtual DbSet<MonthlyGoal> MonthlyGoals { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public DbSet<AccountBank> AccountBanks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Name=MoneyMindConnectionString");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();
            });

            var necessitiesId = "b203ae2f-3023-41c1-a25a-2b2ec238321d";
            var financialFreedomId = "654a9673-4d23-44b1-9af8-a9562341a60e";
            var educationId = "19ea7e67-8095-4a13-bba4-bda0a4a47a38";
            var leisureId = "6193fcb1-c8c4-44e9-abde-78cdb4258c4e";
            var charityId = "b79d14db-7a81-4046-b66e-1acd761123bb";
            var savingsId = "ebebc667-520d-4eac-88ed-ef9eb8e26aab";

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = Guid.Parse(necessitiesId),
                    Name = "Necessities",
                    Description = "Essential expenses for daily living, including food, housing, and utilities."
                },
                new Category
                {
                    Id = Guid.Parse(financialFreedomId),
                    Name = "Financial Freedom",
                    Description = "Allocations for building wealth and achieving long-term financial independence."
                },
                new Category
                {
                    Id = Guid.Parse(educationId),
                    Name = "Education",
                    Description = "Investments in personal growth, such as books, courses, and training programs."
                },
                new Category
                {
                    Id = Guid.Parse(leisureId),
                    Name = "Leisure",
                    Description = "Spending on entertainment and recreational activities for enjoyment."
                },
                new Category
                {
                    Id = Guid.Parse(charityId),
                    Name = "Charity",
                    Description = "Contributions to charitable causes or support for those in need."
                },
                new Category
                {
                    Id = Guid.Parse(savingsId),
                    Name = "Savings",
                    Description = "Funds set aside for major purchases, emergencies, or future needs."
                }
            );
        }
    }
}
