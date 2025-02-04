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
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<GoalItem> GoalItems { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MonthlyGoal> MonthlyGoals { get; set; }
        public virtual DbSet<SheetTransction> SheetTransctions { get; set; }
        public virtual DbSet<SubWalletType> SubWalletTypes { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TransactionSyncLog> TransactionSyncLogs { get; set; }
        public virtual DbSet<TransactionTag> TransactionTags { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<WalletType> WalletTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Name=MoneyMindConnectionString");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WalletType>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();
            });

            var necessitiesId = "b203ae2f-3023-41c1-a25a-2b2ec238321d";
            var financialFreedomId = "654a9673-4d23-44b1-9af8-a9562341a60e";
            var educationId = "19ea7e67-8095-4a13-bba4-bda0a4a47a38";
            var leisureId = "6193fcb1-c8c4-44e9-abde-78cdb4258c4e";
            var charityId = "b79d14db-7a81-4046-b66e-1acd761123bb";
            var savingsId = "ebebc667-520d-4eac-88ed-ef9eb8e26aab";

            modelBuilder.Entity<WalletType>().HasData(
                new WalletType
                {
                    Id = Guid.Parse(necessitiesId),
                    Name = "Necessities",
                    Description = "Essential expenses for daily living, including food, housing, and utilities."
                },
                new WalletType
                {
                    Id = Guid.Parse(financialFreedomId),
                    Name = "Financial Freedom",
                    Description = "Allocations for building wealth and achieving long-term financial independence."
                },
                new WalletType
                {
                    Id = Guid.Parse(educationId),
                    Name = "Education",
                    Description = "Investments in personal growth, such as books, courses, and training programs."
                },
                new WalletType
                {
                    Id = Guid.Parse(leisureId),
                    Name = "Leisure",
                    Description = "Spending on entertainment and recreational activities for enjoyment."
                },
                new WalletType
                {
                    Id = Guid.Parse(charityId),
                    Name = "Charity",
                    Description = "Contributions to charitable causes or support for those in need."
                },
                new WalletType
                {
                    Id = Guid.Parse(savingsId),
                    Name = "Savings",
                    Description = "Funds set aside for major purchases, emergencies, or future needs."
                }
            );

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();
            });
            modelBuilder.Entity<Tag>().HasData(
                // Necessities - Chi tiêu cần thiết
                new Tag { Id = Guid.Parse("52eb2c6a-3a1e-45bd-8e55-33558f69cc3e"), Name = "Rent", Description = "Monthly rent payments.", Color = "#FF5733" },
                new Tag { Id = Guid.Parse("a01486cd-23db-4dde-b674-0292f5074549"), Name = "Utilities", Description = "Electricity, water, and internet bills.", Color = "#33FF57" },
                new Tag { Id = Guid.Parse("e26a8c01-bf3c-4df7-95cf-8f31a801f003"), Name = "Groceries", Description = "Daily groceries and household items.", Color = "#3375FF" },
                new Tag { Id = Guid.Parse("a9e231c4-aa32-4cb9-b55e-e7608b25b528"), Name = "Transportation", Description = "Fuel or public transport expenses.", Color = "#FF33B5" },
                new Tag { Id = Guid.Parse("a5c41682-e6e8-4482-a021-47a876e5d2ff"), Name = "Insurance", Description = "Health, car, or property insurance payments.", Color = "#FFAA33" },
                new Tag { Id = Guid.Parse("7289ab51-6305-4011-a00d-75f2f7bd37cf"), Name = "Healthcare", Description = "Medical bills, prescriptions, and doctor visits.", Color = "#E74C3C" },

                // Financial Freedom - Tự do tài chính
                new Tag { Id = Guid.Parse("cbc1c237-0527-403c-951f-f659536dbf89"), Name = "Investments", Description = "Investments in stocks, bonds, or mutual funds.", Color = "#F39C12" },
                new Tag { Id = Guid.Parse("b1d4bc63-215b-4e8a-aeb8-55fea7dc5ffe"), Name = "Savings", Description = "Funds saved for future use.", Color = "#16A085" },
                new Tag { Id = Guid.Parse("22534a32-cf9e-41c4-ae8a-0a155a9d9701"), Name = "Passive Income", Description = "Spending on assets that generate income.", Color = "#2ECC71" },
                new Tag { Id = Guid.Parse("19085b7c-4115-4d03-9c78-8793771b3699"), Name = "Debt Repayment", Description = "Payments towards loans or credit card debts.", Color = "#D35400" },

                // Education - Giáo dục
                new Tag { Id = Guid.Parse("c95afcd0-5684-477d-a5a0-c92d1381fdc3"), Name = "Tuition", Description = "School or university tuition fees.", Color = "#1ABC9C" },
                new Tag { Id = Guid.Parse("1e23668b-8e90-4e07-9746-c307ed9d4feb"), Name = "Books", Description = "Purchasing books for education or self-study.", Color = "#8E44AD" },
                new Tag { Id = Guid.Parse("f8c65a1f-b7f0-427e-9072-68736eb09e19"), Name = "Online Courses", Description = "E-learning platforms like Udemy or Coursera.", Color = "#2980B9" },
                new Tag { Id = Guid.Parse("ef3630b6-8e69-4e02-99a7-0df799de2e35"), Name = "Workshops", Description = "Attending workshops or skill training.", Color = "#9B59B6" },
                new Tag { Id = Guid.Parse("94237b82-5892-429c-af7d-988577f6ee33"), Name = "School Supplies", Description = "Stationery and other study materials.", Color = "#34495E" },

                // Leisure - Hưởng thụ
                new Tag { Id = Guid.Parse("e495893f-1aa6-49f3-8b9f-41b255b1ef05"), Name = "Dining Out", Description = "Meals at restaurants or cafes.", Color = "#F1C40F" },
                new Tag { Id = Guid.Parse("91482b6a-32b7-4b79-adc4-95367abfe584"), Name = "Travel", Description = "Expenses for vacations or trips.", Color = "#1F618D" },
                new Tag { Id = Guid.Parse("11c4f80b-59bf-495a-9dc9-8ea68e83478b"), Name = "Entertainment", Description = "Movies, games, or music subscriptions.", Color = "#E67E22" },
                new Tag { Id = Guid.Parse("b2a9c566-333d-486f-b6eb-3cedf781501d"), Name = "Shopping", Description = "Clothes, accessories, and other non-essential items.", Color = "#C0392B" },
                new Tag { Id = Guid.Parse("2111d251-a6b3-4e14-97ce-33098c75d140"), Name = "Hobbies", Description = "Spending on personal hobbies like photography or painting.", Color = "#7D3C98" },
                new Tag { Id = Guid.Parse("e4f334d5-4ae8-4823-8d7d-a4a14783988e"), Name = "Fitness", Description = "Gym memberships or sports equipment.", Color = "#27AE60" },

                // Charity - Từ thiện
                new Tag { Id = Guid.Parse("a2e9b2f8-d0bd-4ad6-b59f-deab605a668e"), Name = "Donations", Description = "Charitable donations to organizations or causes.", Color = "#D5A6BD" },
                new Tag { Id = Guid.Parse("a786a452-2931-440d-b4ef-840ed2c56a25"), Name = "Community Support", Description = "Helping local communities or individuals in need.", Color = "#B03A2E" },
                new Tag { Id = Guid.Parse("e6ffb782-edda-4081-bad4-4f765062afea"), Name = "Fundraising", Description = "Contributions to fundraisers or events.", Color = "#AF601A" },
                new Tag { Id = Guid.Parse("50493244-74f4-4d06-a2b6-7f32ea665804"), Name = "Relief Aid", Description = "Supporting disaster relief efforts.", Color = "#784212" },

                // Savings - Tiết kiệm
                new Tag { Id = Guid.Parse("9d0a31a1-2ff8-47e1-bd92-c77a89767854"), Name = "Emergency Fund", Description = "Savings set aside for emergencies.", Color = "#616A6B" },
                new Tag { Id = Guid.Parse("ad9a5ea0-288f-465d-8ced-ca751d913c9d"), Name = "Retirement", Description = "Funds saved for retirement.", Color = "#F8C471" },
                new Tag { Id = Guid.Parse("39480faf-a33f-4d23-a3cb-767ac288a8ef"), Name = "Future Purchases", Description = "Saving for big-ticket items like a car or home.", Color = "#5499C7" },
                new Tag { Id = Guid.Parse("3b79a615-ef8b-424d-920b-096676cf51b9"), Name = "Children's Fund", Description = "Savings for children's education or needs.", Color = "#A569BD" },
                new Tag { Id = Guid.Parse("02600672-6809-42ba-a921-c7da1dc9b0b2"), Name = "Travel Savings", Description = "Saving for future travel plans.", Color = "#1ABC9C" },

                // Miscellaneous - Chi tiêu khác
                new Tag { Id = Guid.Parse("3209b649-90cd-4094-a9b3-90fcec03179c"), Name = "Gifts", Description = "Purchases of gifts for others.", Color = "#FF5733" },
                new Tag { Id = Guid.Parse("89e11720-5151-40cc-8941-55ad35b0792c"), Name = "Special Occasions", Description = "Expenses for weddings, birthdays, or anniversaries.", Color = "#DFFF00" },
                new Tag { Id = Guid.Parse("48826d61-2571-4ec8-8da3-1f23e4766d07"), Name = "Subscriptions", Description = "Monthly or yearly subscription services.", Color = "#8E44AD" },
                new Tag { Id = Guid.Parse("b343a8d0-9f6c-4865-8910-85b8c80eb9ad"), Name = "Uncategorized", Description = "Expenses that don't fit any other category.", Color = "#BDC3C7" },
                new Tag { Id = Guid.Parse("c519c028-5b5d-48a4-b4bb-25cfc91ce18d"), Name = "Fees & Charges", Description = "Bank or credit card fees and charges.", Color = "#5D6D7E" }
            );
        }
    }
}
