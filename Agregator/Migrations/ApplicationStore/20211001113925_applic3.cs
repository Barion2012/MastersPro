using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class applic3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "professionId",
                table: "Applicants",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "marriedStatus",
                table: "Applicants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)");

            migrationBuilder.AlterColumn<bool>(
                name: "hostel",
                table: "Applicants",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)");

            migrationBuilder.AlterColumn<string>(
                name: "deviceNumber",
                table: "Applicants",
                type: "varchar(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_citizenshipId",
                table: "Applicants",
                column: "citizenshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_professionId",
                table: "Applicants",
                column: "professionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Countries_citizenshipId",
                table: "Applicants",
                column: "citizenshipId",
                principalTable: "Countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Positions_professionId",
                table: "Applicants",
                column: "professionId",
                principalTable: "Positions",
                principalColumn: "row_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Countries_citizenshipId",
                table: "Applicants");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Positions_professionId",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_citizenshipId",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_professionId",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "Applicants");

            migrationBuilder.AlterColumn<string>(
                name: "professionId",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "marriedStatus",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "hostel",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "deviceNumber",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldNullable: true);
        }
    }
}
