using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialUnitFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InventoryUnitCode",
                table: "Wms_Material_Material",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InventoryUnitName",
                table: "Wms_Material_Material",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseUnitCode",
                table: "Wms_Material_Material",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseUnitName",
                table: "Wms_Material_Material",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesUnitCode",
                table: "Wms_Material_Material",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesUnitName",
                table: "Wms_Material_Material",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InventoryUnitCode",
                table: "Wms_Material_Material");

            migrationBuilder.DropColumn(
                name: "InventoryUnitName",
                table: "Wms_Material_Material");

            migrationBuilder.DropColumn(
                name: "PurchaseUnitCode",
                table: "Wms_Material_Material");

            migrationBuilder.DropColumn(
                name: "PurchaseUnitName",
                table: "Wms_Material_Material");

            migrationBuilder.DropColumn(
                name: "SalesUnitCode",
                table: "Wms_Material_Material");

            migrationBuilder.DropColumn(
                name: "SalesUnitName",
                table: "Wms_Material_Material");
        }
    }
}
