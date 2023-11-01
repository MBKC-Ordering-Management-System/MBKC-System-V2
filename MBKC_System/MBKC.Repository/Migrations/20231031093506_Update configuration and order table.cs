using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBKC.Repository.Migrations
{
    public partial class Updateconfigurationandordertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Tax",
                table: "Orders",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,2)");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ScrawlingMoneyExchangeToKitchenCenter",
                table: "Configurations",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ScrawlingMoneyExchangeToStore",
                table: "Configurations",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.UpdateData(
                table: "Configurations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ScrawlingMoneyExchangeToKitchenCenter", "ScrawlingMoneyExchangeToStore" },
                values: new object[] { new TimeSpan(0, 22, 0, 0, 0), new TimeSpan(0, 23, 0, 0, 0) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Tax",
                table: "Orders",
                type: "decimal(9,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
