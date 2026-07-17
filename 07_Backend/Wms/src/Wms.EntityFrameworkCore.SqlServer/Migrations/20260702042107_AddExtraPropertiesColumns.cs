using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraPropertiesColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "WmsPrintTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "WmsLabelTemplates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "WmsBarcodeRules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Warehouse_WarehouseArea",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Warehouse_Warehouse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Warehouse_Location",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Transfer_TransferLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Production_MaterialRequisitionLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Outbound_OutboundOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Outbound_OutboundLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Material_UnitOfMeasure",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Material_MaterialSubstituteRelation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Material_MaterialClassification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Material_Material",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_LineSide_LineSideKanbanItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryFreezeOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryBalance",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryAlert",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryAdjustmentLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryAdjustment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Inbound_InboundOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_Inbound_InboundLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "Wms_CycleCount_CycleCountItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "WmsPrintTasks");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "WmsLabelTemplates");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "WmsBarcodeRules");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Warehouse_WarehouseArea");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Warehouse_Warehouse");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Warehouse_Location");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Transfer_TransferLines");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Production_MaterialRequisitionLines");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Outbound_OutboundOrder");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Outbound_OutboundLine");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Material_UnitOfMeasure");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Material_MaterialSubstituteRelation");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Material_MaterialClassification");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Material_Material");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_LineSide_LineSideKanbanItems");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryFreezeOrder");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryBalance");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryAlert");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryAdjustmentLine");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Inventory_InventoryAdjustment");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Inbound_InboundOrder");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_Inbound_InboundLine");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "Wms_CycleCount_CycleCountItems");
        }
    }
}
