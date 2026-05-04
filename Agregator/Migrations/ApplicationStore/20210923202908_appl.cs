using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class appl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(type: "varchar(100)", nullable: true),
                    lastName = table.Column<string>(type: "varchar(100)", nullable: true),
                    middleName = table.Column<string>(type: "varchar(100)", nullable: true),
                    birthDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    birthPlace = table.Column<string>(type: "varchar(200)", nullable: true),
                    citizenship = table.Column<int>(type: "int", nullable: true),
                    created = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.row_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applicants");
        }
    }
}
