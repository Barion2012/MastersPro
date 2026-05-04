using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class applic2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "professionId",
                table: "Applicants",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "marriedStatus",
                table: "Applicants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "hostel",
                table: "Applicants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "deviceNumber",
                table: "Applicants",
                type: "varchar(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "citizenshipId",
                table: "Applicants",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<ulong>(
                name: "agreement",
                table: "Applicants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "Applicants",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Creator",
                table: "Applicants");

            migrationBuilder.AlterColumn<string>(
                name: "professionId",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "marriedStatus",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "hostel",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "deviceNumber",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "citizenshipId",
                table: "Applicants",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "agreement",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
