using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class contratt4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "app_type",
                table: "ContractAttachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "building",
                table: "ContractAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "path_string",
                table: "ContractAttachments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "app_type",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "building",
                table: "ContractAttachments");

            migrationBuilder.DropColumn(
                name: "path_string",
                table: "ContractAttachments");
        }
    }
}
