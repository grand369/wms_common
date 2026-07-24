using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddInboundLineWarehouseAreaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PutawayAreaCode",
                table: "Wms_Inbound_InboundLine",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PutawayAreaId",
                table: "Wms_Inbound_InboundLine",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PutawayWarehouseCode",
                table: "Wms_Inbound_InboundLine",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PutawayWarehouseId",
                table: "Wms_Inbound_InboundLine",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PutawayAreaCode",
                table: "Wms_Inbound_InboundLine");

            migrationBuilder.DropColumn(
                name: "PutawayAreaId",
                table: "Wms_Inbound_InboundLine");

            migrationBuilder.DropColumn(
                name: "PutawayWarehouseCode",
                table: "Wms_Inbound_InboundLine");

            migrationBuilder.DropColumn(
                name: "PutawayWarehouseId",
                table: "Wms_Inbound_InboundLine");
        }
    }
}
