using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFactoryMaterialRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Factories_FactoryId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_FactoryId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "FactoryId",
                table: "Materials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FactoryId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_FactoryId",
                table: "Materials",
                column: "FactoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Factories_FactoryId",
                table: "Materials",
                column: "FactoryId",
                principalTable: "Factories",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
