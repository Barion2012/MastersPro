using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class contr_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "corelationId",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "operationId",
                table: "Contracts",
                newName: "outOperId");

            migrationBuilder.AddColumn<decimal>(
                name: "ToPay",
                table: "Contracts",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "inOperId",
                table: "Contracts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "payCorelationId",
                table: "Contracts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToPay",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "inOperId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "payCorelationId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "outOperId",
                table: "Contracts",
                newName: "operationId");

            migrationBuilder.AddColumn<Guid>(
                name: "corelationId",
                table: "Contracts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }
    }
}
