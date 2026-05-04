using Microsoft.EntityFrameworkCore.Migrations;

namespace Agregator.Migrations
{
    public partial class qq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "recipientId",
                table: "RecipientsView",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "quant",
                table: "ContractAttachments",
                type: "decimal(10,4)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "marriedStatus",
                table: "Applicants",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "male",
                table: "Applicants",
                type: "bit",
                nullable: true,
                defaultValueSql: "1",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldDefaultValueSql: "1");

            migrationBuilder.AlterColumn<ulong>(
                name: "hostel",
                table: "Applicants",
                type: "bit",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bit");

            migrationBuilder.AlterColumn<ulong>(
                name: "agreement",
                table: "Applicants",
                type: "bit",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "recipientId",
                table: "RecipientsView",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "quant",
                table: "ContractAttachments",
                type: "int(11)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "marriedStatus",
                table: "Applicants",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "male",
                table: "Applicants",
                type: "bit",
                nullable: false,
                defaultValueSql: "1",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "1");

            migrationBuilder.AlterColumn<ulong>(
                name: "hostel",
                table: "Applicants",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "agreement",
                table: "Applicants",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
