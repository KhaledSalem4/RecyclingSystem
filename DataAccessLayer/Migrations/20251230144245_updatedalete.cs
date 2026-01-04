using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class updatedalete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoryRewards_Rewards_RewardId",
                table: "HistoryRewards");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryRewards_Rewards_RewardId",
                table: "HistoryRewards",
                column: "RewardId",
                principalTable: "Rewards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoryRewards_Rewards_RewardId",
                table: "HistoryRewards");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoryRewards_Rewards_RewardId",
                table: "HistoryRewards",
                column: "RewardId",
                principalTable: "Rewards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
