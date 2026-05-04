using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations.ApplicationStore
{
    public partial class attachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<decimal>(
                name: "quant",
                table: "ContractAttachments",
                type: "decimal(19,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AlterColumn<int>(
                name: "quant",
                table: "ContractAttachments",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,2)",
                oldNullable: true);
        }
    }
}
