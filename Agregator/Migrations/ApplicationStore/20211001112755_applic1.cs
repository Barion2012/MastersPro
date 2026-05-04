using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class applic1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "citizenship",
                table: "Applicants");

            migrationBuilder.AddColumn<bool>(
                name: "rostScope",
                table: "Positions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "actualPlace",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "agreement",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "");

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
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deviceNumber",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "education",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hostel",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
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
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "professionId",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "skils",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tools",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true);
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

            migrationBuilder.AddColumn<int>(
                name: "citizenship",
                table: "Applicants",
                type: "int",
                nullable: true);
        }
    }
}
