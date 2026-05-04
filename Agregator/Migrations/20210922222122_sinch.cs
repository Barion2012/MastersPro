using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class sinch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "work_id",
                table: "Works",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "reestrId",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "app_type",
                table: "ContractAttachments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "building",
                table: "ContractAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "floor",
                table: "ContractAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "object_id",
                table: "ContractAttachments",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "object_name",
                table: "ContractAttachments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "path_string",
                table: "ContractAttachments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "room",
                table: "ContractAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "section",
                table: "ContractAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "work_id",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "reestrId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "app_type",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "building",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "floor",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "object_id",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "object_name",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "path_string",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "room",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "section",
                table: "ContractAttachments");
        }
    }
}
