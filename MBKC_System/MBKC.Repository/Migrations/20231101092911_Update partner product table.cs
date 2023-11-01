using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBKC.Repository.Migrations
{
    public partial class Updatepartnerproducttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerProducts",
                table: "PartnerProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerProducts",
                table: "PartnerProducts",
                columns: new[] { "ProductId", "PartnerId", "StoreId", "CreatedDate", "MappedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerProducts",
                table: "PartnerProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerProducts",
                table: "PartnerProducts",
                columns: new[] { "ProductId", "PartnerId", "StoreId", "CreatedDate" });
        }
    }
}
