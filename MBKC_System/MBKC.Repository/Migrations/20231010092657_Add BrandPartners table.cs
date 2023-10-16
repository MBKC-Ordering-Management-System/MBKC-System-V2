using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBKC.Repository.Migrations
{
    public partial class AddBrandPartnerstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrandPartner",
                columns: table => new
                {
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandPartner", x => new { x.BrandId, x.PartnerId, x.CreatedDate });
                    table.ForeignKey(
                        name: "FK_BrandPartner_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandPartner_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandPartner_PartnerId",
                table: "BrandPartner",
                column: "PartnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandPartner");
        }
    }
}
