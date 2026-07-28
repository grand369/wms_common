using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddFreezeOrderMaterialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaterialId",
                table: "Wms_Inventory_InventoryFreezeOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialCode",
                table: "Wms_Inventory_InventoryFreezeOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FreezeQuantity",
                table: "Wms_Inventory_InventoryFreezeOrder",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "Wms_Inventory_InventoryFreezeOrder");

            migrationBuilder.DropColumn(
                name: "MaterialCode",
                table: "Wms_Inventory_InventoryFreezeOrder");

            migrationBuilder.DropColumn(
                name: "FreezeQuantity",
                table: "Wms_Inventory_InventoryFreezeOrder");
        }
    }
}
