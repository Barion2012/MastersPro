using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class message1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    row_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "varchar(8000)", nullable: true),
                    created = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    SendFromId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SendToId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.row_id);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SendFromId",
                        column: x => x.SendFromId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SendToId",
                        column: x => x.SendToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_Messages_SendFromId",
                table: "Messages",
                column: "SendFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SendToId",
                table: "Messages",
                column: "SendToId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

        }
    }
}
