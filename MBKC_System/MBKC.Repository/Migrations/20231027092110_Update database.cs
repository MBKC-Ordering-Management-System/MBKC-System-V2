using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBKC.Repository.Migrations
{
    public partial class Updatedatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashierMoneyExchanges_Cashiers_AccountId",
                table: "CashierMoneyExchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipperPayments_BankingAccounts_BankingAccountId",
                table: "ShipperPayments");

            migrationBuilder.DropIndex(
                name: "IX_ShipperPayments_BankingAccountId",
                table: "ShipperPayments");

            migrationBuilder.DropIndex(
                name: "IX_CashierMoneyExchanges_AccountId",
                table: "CashierMoneyExchanges");

            migrationBuilder.DropColumn(
                name: "BankingAccountId",
                table: "ShipperPayments");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "CashierMoneyExchanges");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Commission",
                table: "StorePartners",
                type: "decimal(9,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ShipperPayments",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "MoneyExchanges",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "ExchangeImage",
                table: "MoneyExchanges",
                type: "varchar(max)",
                unicode: false,
                maxLength: 2147483647,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipperPayments_KCBankingAccountId",
                table: "ShipperPayments",
                column: "KCBankingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CashierMoneyExchanges_CashierId",
                table: "CashierMoneyExchanges",
                column: "CashierId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashierMoneyExchanges_Cashiers_CashierId",
                table: "CashierMoneyExchanges",
                column: "CashierId",
                principalTable: "Cashiers",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipperPayments_BankingAccounts_KCBankingAccountId",
                table: "ShipperPayments",
                column: "KCBankingAccountId",
                principalTable: "BankingAccounts",
                principalColumn: "BankingAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashierMoneyExchanges_Cashiers_CashierId",
                table: "CashierMoneyExchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipperPayments_BankingAccounts_KCBankingAccountId",
                table: "ShipperPayments");

            migrationBuilder.DropIndex(
                name: "IX_ShipperPayments_KCBankingAccountId",
                table: "ShipperPayments");

            migrationBuilder.DropIndex(
                name: "IX_CashierMoneyExchanges_CashierId",
                table: "CashierMoneyExchanges");

            migrationBuilder.DropColumn(
                name: "ExchangeImage",
                table: "MoneyExchanges");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Commission",
                table: "StorePartners",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ShipperPayments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<int>(
                name: "BankingAccountId",
                table: "ShipperPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "MoneyExchanges",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "CashierMoneyExchanges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShipperPayments_BankingAccountId",
                table: "ShipperPayments",
                column: "BankingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CashierMoneyExchanges_AccountId",
                table: "CashierMoneyExchanges",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashierMoneyExchanges_Cashiers_AccountId",
                table: "CashierMoneyExchanges",
                column: "AccountId",
                principalTable: "Cashiers",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipperPayments_BankingAccounts_BankingAccountId",
                table: "ShipperPayments",
                column: "BankingAccountId",
                principalTable: "BankingAccounts",
                principalColumn: "BankingAccountId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
