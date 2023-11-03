using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBKC.Repository.Migrations
{
    public partial class UpdatecommissionRowinOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Wallets_WalletId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_WalletId",
                table: "Stores");

            migrationBuilder.AlterColumn<int>(
                name: "WalletId",
                table: "Stores",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "Commission",
                table: "Orders",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,2)");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_WalletId",
                table: "Stores",
                column: "WalletId",
                unique: true,
                filter: "[WalletId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Wallets_WalletId",
                table: "Stores",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Wallets_WalletId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_WalletId",
                table: "Stores");

            migrationBuilder.AlterColumn<int>(
                name: "WalletId",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Commission",
                table: "Orders",
                type: "decimal(9,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_WalletId",
                table: "Stores",
                column: "WalletId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Wallets_WalletId",
                table: "Stores",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
