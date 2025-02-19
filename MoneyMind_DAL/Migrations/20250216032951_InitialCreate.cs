using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMind_DAL.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:MoneyMind_DAL/Migrations/20250219125742_InitialMoneyMindMigration.cs
    public partial class InitialMoneyMindMigration : Migration
========
    public partial class InitialCreate : Migration
>>>>>>>> 6ef27173440f15b603ebb8158da9ed8af8febaa8:MoneyMind_DAL/Migrations/20250216032951_InitialCreate.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastMessageTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyGoal",
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
                    table.PrimaryKey("PK_MonthlyGoal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SheetTransction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SheetId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetTransction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionSyncLog",
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
                    table.PrimaryKey("PK_TransactionSyncLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFcmTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FcmToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFcmTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false),
                    IsBotResponse = table.Column<bool>(type: "bit", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoalItem",
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
                    table.PrimaryKey("PK_GoalItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoalItem_MonthlyGoal_MonthlyGoalId",
                        column: x => x.MonthlyGoalId,
                        principalTable: "MonthlyGoal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalItem_WalletType_WalletTypeId",
                        column: x => x.WalletTypeId,
                        principalTable: "WalletType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletCategory",
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
                    table.PrimaryKey("PK_WalletCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletCategory_WalletType_WalletTypeId",
                        column: x => x.WalletTypeId,
                        principalTable: "WalletType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    WalletCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_WalletCategory_WalletCategoryId",
                        column: x => x.WalletCategoryId,
                        principalTable: "WalletCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallet_WalletCategory_WalletCategoryId",
                        column: x => x.WalletCategoryId,
                        principalTable: "WalletCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
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
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransactionActivity",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionActivity", x => new { x.TransactionId, x.ActivityId });
                    table.ForeignKey(
                        name: "FK_TransactionActivity_Activity_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionActivity_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTag",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTag", x => new { x.TransactionId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TransactionTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionTag_Transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tag",
                columns: new[] { "Id", "Color", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("02600672-6809-42ba-a921-c7da1dc9b0b2"), "#1ABC9C", "Saving for future travel plans.", false, "Travel Savings" },
                    { new Guid("11c4f80b-59bf-495a-9dc9-8ea68e83478b"), "#E67E22", "Movies, games, or music subscriptions.", false, "Entertainment" },
                    { new Guid("19085b7c-4115-4d03-9c78-8793771b3699"), "#D35400", "Payments towards loans or credit card debts.", false, "Debt Repayment" },
                    { new Guid("1e23668b-8e90-4e07-9746-c307ed9d4feb"), "#8E44AD", "Purchasing books for education or self-study.", false, "Books" },
                    { new Guid("2111d251-a6b3-4e14-97ce-33098c75d140"), "#7D3C98", "Spending on personal hobbies like photography or painting.", false, "Hobbies" },
                    { new Guid("22534a32-cf9e-41c4-ae8a-0a155a9d9701"), "#2ECC71", "Spending on assets that generate income.", false, "Passive Income" },
                    { new Guid("3209b649-90cd-4094-a9b3-90fcec03179c"), "#FF5733", "Purchases of gifts for others.", false, "Gifts" },
                    { new Guid("39480faf-a33f-4d23-a3cb-767ac288a8ef"), "#5499C7", "Saving for big-ticket items like a car or home.", false, "Future Purchases" },
                    { new Guid("3b79a615-ef8b-424d-920b-096676cf51b9"), "#A569BD", "Savings for children's education or needs.", false, "Children's Fund" },
                    { new Guid("48826d61-2571-4ec8-8da3-1f23e4766d07"), "#8E44AD", "Monthly or yearly subscription services.", false, "Subscriptions" },
                    { new Guid("50493244-74f4-4d06-a2b6-7f32ea665804"), "#784212", "Supporting disaster relief efforts.", false, "Relief Aid" },
                    { new Guid("52eb2c6a-3a1e-45bd-8e55-33558f69cc3e"), "#FF5733", "Monthly rent payments.", false, "Rent" },
                    { new Guid("7289ab51-6305-4011-a00d-75f2f7bd37cf"), "#E74C3C", "Medical bills, prescriptions, and doctor visits.", false, "Healthcare" },
                    { new Guid("89e11720-5151-40cc-8941-55ad35b0792c"), "#DFFF00", "Expenses for weddings, birthdays, or anniversaries.", false, "Special Occasions" },
                    { new Guid("91482b6a-32b7-4b79-adc4-95367abfe584"), "#1F618D", "Expenses for vacations or trips.", false, "Travel" },
                    { new Guid("94237b82-5892-429c-af7d-988577f6ee33"), "#34495E", "Stationery and other study materials.", false, "School Supplies" },
                    { new Guid("9d0a31a1-2ff8-47e1-bd92-c77a89767854"), "#616A6B", "Savings set aside for emergencies.", false, "Emergency Fund" },
                    { new Guid("a01486cd-23db-4dde-b674-0292f5074549"), "#33FF57", "Electricity, water, and internet bills.", false, "Utilities" },
                    { new Guid("a2e9b2f8-d0bd-4ad6-b59f-deab605a668e"), "#D5A6BD", "Charitable donations to organizations or causes.", false, "Donations" },
                    { new Guid("a5c41682-e6e8-4482-a021-47a876e5d2ff"), "#FFAA33", "Health, car, or property insurance payments.", false, "Insurance" },
                    { new Guid("a786a452-2931-440d-b4ef-840ed2c56a25"), "#B03A2E", "Helping local communities or individuals in need.", false, "Community Support" },
                    { new Guid("a9e231c4-aa32-4cb9-b55e-e7608b25b528"), "#FF33B5", "Fuel or public transport expenses.", false, "Transportation" },
                    { new Guid("ad9a5ea0-288f-465d-8ced-ca751d913c9d"), "#F8C471", "Funds saved for retirement.", false, "Retirement" },
                    { new Guid("b1d4bc63-215b-4e8a-aeb8-55fea7dc5ffe"), "#16A085", "Funds saved for future use.", false, "Savings" },
                    { new Guid("b2a9c566-333d-486f-b6eb-3cedf781501d"), "#C0392B", "Clothes, accessories, and other non-essential items.", false, "Shopping" },
                    { new Guid("b343a8d0-9f6c-4865-8910-85b8c80eb9ad"), "#BDC3C7", "Expenses that don't fit any other category.", false, "Uncategorized" },
                    { new Guid("c519c028-5b5d-48a4-b4bb-25cfc91ce18d"), "#5D6D7E", "Bank or credit card fees and charges.", false, "Fees & Charges" },
                    { new Guid("c95afcd0-5684-477d-a5a0-c92d1381fdc3"), "#1ABC9C", "School or university tuition fees.", false, "Tuition" },
                    { new Guid("cbc1c237-0527-403c-951f-f659536dbf89"), "#F39C12", "Investments in stocks, bonds, or mutual funds.", false, "Investments" },
                    { new Guid("e26a8c01-bf3c-4df7-95cf-8f31a801f003"), "#3375FF", "Daily groceries and household items.", false, "Groceries" },
                    { new Guid("e495893f-1aa6-49f3-8b9f-41b255b1ef05"), "#F1C40F", "Meals at restaurants or cafes.", false, "Dining Out" },
                    { new Guid("e4f334d5-4ae8-4823-8d7d-a4a14783988e"), "#27AE60", "Gym memberships or sports equipment.", false, "Fitness" },
                    { new Guid("e6ffb782-edda-4081-bad4-4f765062afea"), "#AF601A", "Contributions to fundraisers or events.", false, "Fundraising" },
                    { new Guid("ef3630b6-8e69-4e02-99a7-0df799de2e35"), "#9B59B6", "Attending workshops or skill training.", false, "Workshops" },
                    { new Guid("f8c65a1f-b7f0-427e-9072-68736eb09e19"), "#2980B9", "E-learning platforms like Udemy or Coursera.", false, "Online Courses" }
                });

            migrationBuilder.InsertData(
                table: "WalletType",
                columns: new[] { "Id", "Description", "IsDisabled", "Name" },
                values: new object[,]
                {
                    { new Guid("19ea7e67-8095-4a13-bba4-bda0a4a47a38"), "Investments in personal growth, such as books, courses, and training programs.", false, "Education" },
                    { new Guid("6193fcb1-c8c4-44e9-abde-78cdb4258c4e"), "Spending on entertainment and recreational activities for enjoyment.", false, "Leisure" },
                    { new Guid("654a9673-4d23-44b1-9af8-a9562341a60e"), "Allocations for building wealth and achieving long-term financial independence.", false, "Financial Freedom" },
                    { new Guid("b203ae2f-3023-41c1-a25a-2b2ec238321d"), "Essential expenses for daily living, including food, housing, and utilities.", false, "Necessities" },
                    { new Guid("b79d14db-7a81-4046-b66e-1acd761123bb"), "Contributions to charitable causes or support for those in need.", false, "Charity" },
                    { new Guid("ebebc667-520d-4eac-88ed-ef9eb8e26aab"), "Funds set aside for major purchases, emergencies, or future needs.", false, "Savings" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_WalletCategoryId",
                table: "Activity",
                column: "WalletCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalItem_MonthlyGoalId",
                table: "GoalItem",
                column: "MonthlyGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalItem_WalletTypeId",
                table: "GoalItem",
                column: "WalletTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatId",
                table: "Message",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Name",
                table: "Tag",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_WalletId",
                table: "Transaction",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionActivity_ActivityId",
                table: "TransactionActivity",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTag_TagId",
                table: "TransactionTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_WalletCategoryId",
                table: "Wallet",
                column: "WalletCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletCategory_WalletTypeId",
                table: "WalletCategory",
                column: "WalletTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletType_Name",
                table: "WalletType",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoalItem");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "SheetTransction");

            migrationBuilder.DropTable(
                name: "TransactionActivity");

            migrationBuilder.DropTable(
                name: "TransactionSyncLog");

            migrationBuilder.DropTable(
                name: "TransactionTag");

            migrationBuilder.DropTable(
                name: "UserFcmTokens");

            migrationBuilder.DropTable(
                name: "MonthlyGoal");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "WalletCategory");

            migrationBuilder.DropTable(
                name: "WalletType");
        }
    }
}
