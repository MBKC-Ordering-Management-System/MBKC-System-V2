using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBKC.Repository.Migrations
{
    public partial class Updatepartnerproducttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MappingProducts");

            migrationBuilder.AlterColumn<int>(
                name: "ExchangeId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<float>(
                name: "Commission",
                table: "StorePartners",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<int>(
                name: "KCBankingAccountId",
                table: "ShipperPayments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "PartnerProducts",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerProducts", x => new { x.ProductId, x.PartnerId, x.StoreId, x.CreatedDate });
                    table.ForeignKey(
                        name: "FK_PartnerProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerProducts_StorePartners_StoreId_PartnerId_CreatedDate",
                        columns: x => new { x.StoreId, x.PartnerId, x.CreatedDate },
                        principalTable: "StorePartners",
                        principalColumns: new[] { "StoreId", "PartnerId", "CreatedDate" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerProducts_StoreId_PartnerId_CreatedDate",
                table: "PartnerProducts",
                columns: new[] { "StoreId", "PartnerId", "CreatedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerProducts");

            migrationBuilder.DropColumn(
                name: "Commission",
                table: "StorePartners");

            migrationBuilder.AlterColumn<int>(
                name: "ExchangeId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "KCBankingAccountId",
                table: "ShipperPayments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MappingProducts",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingProducts", x => new { x.ProductId, x.PartnerId, x.StoreId, x.CreatedDate });
                    table.ForeignKey(
                        name: "FK_MappingProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MappingProducts_StorePartners_StoreId_PartnerId_CreatedDate",
                        columns: x => new { x.StoreId, x.PartnerId, x.CreatedDate },
                        principalTable: "StorePartners",
                        principalColumns: new[] { "StoreId", "PartnerId", "CreatedDate" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MappingProducts_StoreId_PartnerId_CreatedDate",
                table: "MappingProducts",
                columns: new[] { "StoreId", "PartnerId", "CreatedDate" });
        }
    }
}
