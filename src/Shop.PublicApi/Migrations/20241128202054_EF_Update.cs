using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.PublicApi.Migrations;

/// <inheritdoc />
public partial class EF_Update : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Gender",
            table: "Customers",
            type: "nvarchar(6)",
            maxLength: 6,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(6)",
            oldUnicode: false,
            oldMaxLength: 6);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Gender",
            table: "Customers",
            type: "varchar(6)",
            unicode: false,
            maxLength: 6,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(6)",
            oldMaxLength: 6);
    }
}