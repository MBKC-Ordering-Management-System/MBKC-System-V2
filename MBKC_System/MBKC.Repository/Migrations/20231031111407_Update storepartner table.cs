using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBKC.Repository.Migrations
{
    public partial class Updatestorepartnertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Commission",
                table: "StorePartners",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Commission",
                table: "StorePartners",
                type: "decimal(9,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
