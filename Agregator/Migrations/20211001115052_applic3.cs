using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class applic3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
