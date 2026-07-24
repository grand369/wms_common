using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddInboundLineUnitField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Wms_Inbound_InboundLine",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Wms_Inbound_InboundLine");
        }
    }
}
