using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class contratt3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "work_id",
                table: "Works",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "floor",
                table: "ContractAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "object_id",
                table: "ContractAttachments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "object_name",
                table: "ContractAttachments",
                type: "nvarchar(max)",
                nullable: true);

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
                name: "floor",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "object_id",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "object_name",
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
