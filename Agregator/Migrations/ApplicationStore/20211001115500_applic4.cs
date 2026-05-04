using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class applic4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "agreement",
                table: "Applicants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "agreement",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
