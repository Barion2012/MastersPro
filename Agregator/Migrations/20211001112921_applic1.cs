using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class applic1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "citizenship",
                table: "Applicants");

            migrationBuilder.AddColumn<ulong>(
                name: "rostScope",
                table: "Positions",
                type: "bit",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AlterColumn<DateTime>(
                name: "paidDate",
                table: "Contracts",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<string>(
                name: "actualPlace",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "agreement",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "citizenshipId",
                table: "Applicants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "confirmation",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "deviceNumber",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "education",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "hostel",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<ulong>(
                name: "male",
                table: "Applicants",
                type: "bit",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<string>(
                name: "marriedStatus",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "professionId",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "skils",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "tools",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rostScope",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "actualPlace",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "agreement",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "citizenshipId",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "confirmation",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "deviceNumber",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "education",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "hostel",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "male",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "marriedStatus",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "professionId",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "skils",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "tools",
                table: "Applicants");

            migrationBuilder.AlterColumn<DateTime>(
                name: "paidDate",
                table: "Contracts",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "citizenship",
                table: "Applicants",
                type: "int",
                nullable: true);
        }
    }
}
