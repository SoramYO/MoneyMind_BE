using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMind_DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastMessageTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SheetTransctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SheetId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetTransctions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionSyncLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyncTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionSyncLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBotResponse = table.Column<bool>(type: "bit", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoalItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsedAmount = table.Column<double>(type: "float", nullable: false),
                    UsedPercentage = table.Column<double>(type: "float", nullable: false),
                    MinTargetPercentage = table.Column<double>(type: "float", nullable: true),
                    MaxTargetPercentage = table.Column<double>(type: "float", nullable: true),
                    MinAmount = table.Column<double>(type: "float", nullable: true),
                    MaxAmount = table.Column<double>(type: "float", nullable: true),
                    TargetMode = table.Column<int>(type: "int", nullable: false),
                    IsAchieved = table.Column<bool>(type: "bit", nullable: false),
                    MonthlyGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoalItems_MonthlyGoals_MonthlyGoalId",
                        column: x => x.MonthlyGoalId,
                        principalTable: "MonthlyGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalItems_WalletTypes_WalletTypeId",
                        column: x => x.WalletTypeId,
                        principalTable: "WalletTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubWalletTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubWalletTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubWalletTypes_WalletTypes_WalletTypeId",
                        column: x => x.WalletTypeId,
                        principalTable: "WalletTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubWalletTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_SubWalletTypes_SubWalletTypeId",
                        column: x => x.SubWalletTypeId,
                        principalTable: "SubWalletTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransactionTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionTags_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Color", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("02600672-6809-42ba-a921-c7da1dc9b0b2"), "#1ABC9C", "Saving for future travel plans.", "Travel Savings" },
                    { new Guid("11c4f80b-59bf-495a-9dc9-8ea68e83478b"), "#E67E22", "Movies, games, or music subscriptions.", "Entertainment" },
                    { new Guid("19085b7c-4115-4d03-9c78-8793771b3699"), "#D35400", "Payments towards loans or credit card debts.", "Debt Repayment" },
                    { new Guid("1e23668b-8e90-4e07-9746-c307ed9d4feb"), "#8E44AD", "Purchasing books for education or self-study.", "Books" },
                    { new Guid("2111d251-a6b3-4e14-97ce-33098c75d140"), "#7D3C98", "Spending on personal hobbies like photography or painting.", "Hobbies" },
                    { new Guid("22534a32-cf9e-41c4-ae8a-0a155a9d9701"), "#2ECC71", "Spending on assets that generate income.", "Passive Income" },
                    { new Guid("3209b649-90cd-4094-a9b3-90fcec03179c"), "#FF5733", "Purchases of gifts for others.", "Gifts" },
                    { new Guid("39480faf-a33f-4d23-a3cb-767ac288a8ef"), "#5499C7", "Saving for big-ticket items like a car or home.", "Future Purchases" },
                    { new Guid("3b79a615-ef8b-424d-920b-096676cf51b9"), "#A569BD", "Savings for children's education or needs.", "Children's Fund" },
                    { new Guid("48826d61-2571-4ec8-8da3-1f23e4766d07"), "#8E44AD", "Monthly or yearly subscription services.", "Subscriptions" },
                    { new Guid("50493244-74f4-4d06-a2b6-7f32ea665804"), "#784212", "Supporting disaster relief efforts.", "Relief Aid" },
                    { new Guid("52eb2c6a-3a1e-45bd-8e55-33558f69cc3e"), "#FF5733", "Monthly rent payments.", "Rent" },
                    { new Guid("7289ab51-6305-4011-a00d-75f2f7bd37cf"), "#E74C3C", "Medical bills, prescriptions, and doctor visits.", "Healthcare" },
                    { new Guid("89e11720-5151-40cc-8941-55ad35b0792c"), "#DFFF00", "Expenses for weddings, birthdays, or anniversaries.", "Special Occasions" },
                    { new Guid("91482b6a-32b7-4b79-adc4-95367abfe584"), "#1F618D", "Expenses for vacations or trips.", "Travel" },
                    { new Guid("94237b82-5892-429c-af7d-988577f6ee33"), "#34495E", "Stationery and other study materials.", "School Supplies" },
                    { new Guid("9d0a31a1-2ff8-47e1-bd92-c77a89767854"), "#616A6B", "Savings set aside for emergencies.", "Emergency Fund" },
                    { new Guid("a01486cd-23db-4dde-b674-0292f5074549"), "#33FF57", "Electricity, water, and internet bills.", "Utilities" },
                    { new Guid("a2e9b2f8-d0bd-4ad6-b59f-deab605a668e"), "#D5A6BD", "Charitable donations to organizations or causes.", "Donations" },
                    { new Guid("a5c41682-e6e8-4482-a021-47a876e5d2ff"), "#FFAA33", "Health, car, or property insurance payments.", "Insurance" },
                    { new Guid("a786a452-2931-440d-b4ef-840ed2c56a25"), "#B03A2E", "Helping local communities or individuals in need.", "Community Support" },
                    { new Guid("a9e231c4-aa32-4cb9-b55e-e7608b25b528"), "#FF33B5", "Fuel or public transport expenses.", "Transportation" },
                    { new Guid("ad9a5ea0-288f-465d-8ced-ca751d913c9d"), "#F8C471", "Funds saved for retirement.", "Retirement" },
                    { new Guid("b1d4bc63-215b-4e8a-aeb8-55fea7dc5ffe"), "#16A085", "Funds saved for future use.", "Savings" },
                    { new Guid("b2a9c566-333d-486f-b6eb-3cedf781501d"), "#C0392B", "Clothes, accessories, and other non-essential items.", "Shopping" },
                    { new Guid("b343a8d0-9f6c-4865-8910-85b8c80eb9ad"), "#BDC3C7", "Expenses that don't fit any other category.", "Uncategorized" },
                    { new Guid("c519c028-5b5d-48a4-b4bb-25cfc91ce18d"), "#5D6D7E", "Bank or credit card fees and charges.", "Fees & Charges" },
                    { new Guid("c95afcd0-5684-477d-a5a0-c92d1381fdc3"), "#1ABC9C", "School or university tuition fees.", "Tuition" },
                    { new Guid("cbc1c237-0527-403c-951f-f659536dbf89"), "#F39C12", "Investments in stocks, bonds, or mutual funds.", "Investments" },
                    { new Guid("e26a8c01-bf3c-4df7-95cf-8f31a801f003"), "#3375FF", "Daily groceries and household items.", "Groceries" },
                    { new Guid("e495893f-1aa6-49f3-8b9f-41b255b1ef05"), "#F1C40F", "Meals at restaurants or cafes.", "Dining Out" },
                    { new Guid("e4f334d5-4ae8-4823-8d7d-a4a14783988e"), "#27AE60", "Gym memberships or sports equipment.", "Fitness" },
                    { new Guid("e6ffb782-edda-4081-bad4-4f765062afea"), "#AF601A", "Contributions to fundraisers or events.", "Fundraising" },
                    { new Guid("ef3630b6-8e69-4e02-99a7-0df799de2e35"), "#9B59B6", "Attending workshops or skill training.", "Workshops" },
                    { new Guid("f8c65a1f-b7f0-427e-9072-68736eb09e19"), "#2980B9", "E-learning platforms like Udemy or Coursera.", "Online Courses" }
                });

            migrationBuilder.InsertData(
                table: "WalletTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("19ea7e67-8095-4a13-bba4-bda0a4a47a38"), "Investments in personal growth, such as books, courses, and training programs.", "Education" },
                    { new Guid("6193fcb1-c8c4-44e9-abde-78cdb4258c4e"), "Spending on entertainment and recreational activities for enjoyment.", "Leisure" },
                    { new Guid("654a9673-4d23-44b1-9af8-a9562341a60e"), "Allocations for building wealth and achieving long-term financial independence.", "Financial Freedom" },
                    { new Guid("b203ae2f-3023-41c1-a25a-2b2ec238321d"), "Essential expenses for daily living, including food, housing, and utilities.", "Necessities" },
                    { new Guid("b79d14db-7a81-4046-b66e-1acd761123bb"), "Contributions to charitable causes or support for those in need.", "Charity" },
                    { new Guid("ebebc667-520d-4eac-88ed-ef9eb8e26aab"), "Funds set aside for major purchases, emergencies, or future needs.", "Savings" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoalItems_MonthlyGoalId",
                table: "GoalItems",
                column: "MonthlyGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalItems_WalletTypeId",
                table: "GoalItems",
                column: "WalletTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_SubWalletTypes_WalletTypeId",
                table: "SubWalletTypes",
                column: "WalletTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTags_TagId",
                table: "TransactionTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTags_TransactionId",
                table: "TransactionTags",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_SubWalletTypeId",
                table: "Wallets",
                column: "SubWalletTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTypes_Name",
                table: "WalletTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoalItems");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "SheetTransctions");

            migrationBuilder.DropTable(
                name: "TransactionSyncLogs");

            migrationBuilder.DropTable(
                name: "TransactionTags");

            migrationBuilder.DropTable(
                name: "MonthlyGoals");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "SubWalletTypes");

            migrationBuilder.DropTable(
                name: "WalletTypes");
        }
    }
}
