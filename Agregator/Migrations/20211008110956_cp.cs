using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class cp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "corelationId",
                table: "Contracts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "operationId",
                table: "Contracts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "signtwId",
                table: "Contracts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "corelationId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "operationId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "signtwId",
                table: "Contracts");
        }
    }
}
