using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class twilio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwilioLog",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sid = table.Column<string>(type: "varchar(20)", nullable: true),
                    request = table.Column<string>(type: "varchar(max)", nullable: true),
                    response = table.Column<string>(type: "varchar(max)", nullable: true),
                    error = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwilioLog", x => x.row_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwilioLog");
        }
    }
}
