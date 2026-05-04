using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class contr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaidDate",
                table: "Contracts",
                newName: "paidDate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "paidDate",
                table: "Contracts",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "paidDate",
                table: "Contracts",
                newName: "PaidDate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidDate",
                table: "Contracts",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }
    }
}
