using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class applic4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "marriedStatus",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "marriedStatus",
                table: "Applicants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
