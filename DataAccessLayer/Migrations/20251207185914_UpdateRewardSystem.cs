using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRewardSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoryRewards_AspNetUsers_UserId",
                table: "HistoryRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoryRewards_Rewards_RewardId",
                table: "HistoryRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoryRewards",
                table: "HistoryRewards");

            migrationBuilder.DropColumn(
                name: "RewardType",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "ClaimedAt",
                table: "HistoryRewards");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Rewards",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Rewards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rewards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Rewards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Rewards",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Rewards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "HistoryRewards",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "HistoryRewards",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "RedeemedAt",
                table: "HistoryRewards",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "HistoryRewards",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoryRewards",
                table: "HistoryRewards",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRewards_Status",
                table: "HistoryRewards",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRewards_UserId",
                table: "HistoryRewards",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryRewards_AspNetUsers_UserId",
                table: "HistoryRewards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryRewards_Rewards_RewardId",
                table: "HistoryRewards",
                column: "RewardId",
                principalTable: "Rewards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoryRewards_AspNetUsers_UserId",
                table: "HistoryRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoryRewards_Rewards_RewardId",
                table: "HistoryRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoryRewards",
                table: "HistoryRewards");

            migrationBuilder.DropIndex(
                name: "IX_HistoryRewards_Status",
                table: "HistoryRewards");

            migrationBuilder.DropIndex(
                name: "IX_HistoryRewards_UserId",
                table: "HistoryRewards");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "HistoryRewards");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "HistoryRewards");

            migrationBuilder.DropColumn(
                name: "RedeemedAt",
                table: "HistoryRewards");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "HistoryRewards");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Rewards",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "RewardType",
                table: "Rewards",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimedAt",
                table: "HistoryRewards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoryRewards",
                table: "HistoryRewards",
                columns: new[] { "UserId", "RewardId" });

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryRewards_AspNetUsers_UserId",
                table: "HistoryRewards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryRewards_Rewards_RewardId",
                table: "HistoryRewards",
                column: "RewardId",
                principalTable: "Rewards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
