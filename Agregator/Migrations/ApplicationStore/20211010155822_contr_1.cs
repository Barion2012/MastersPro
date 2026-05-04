using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class contr_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "corelationId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "operationId",
                table: "Contracts");

            migrationBuilder.AlterColumn<string>(
                name: "signtwId",
                table: "Contracts",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "inOperId",
                table: "Contracts",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "outOperId",
                table: "Contracts",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "payCorelationId",
                table: "Contracts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "toPay",
                table: "Contracts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inOperId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "outOperId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "payCorelationId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "toPay",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "signtwId",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "corelationId",
                table: "Contracts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "operationId",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
