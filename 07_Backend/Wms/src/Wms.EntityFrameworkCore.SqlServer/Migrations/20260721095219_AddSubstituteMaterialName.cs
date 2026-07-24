using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddSubstituteMaterialName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubstituteMaterialName",
                table: "Wms_Material_MaterialSubstituteRelation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubstituteMaterialName",
                table: "Wms_Material_MaterialSubstituteRelation");
        }
    }
}
