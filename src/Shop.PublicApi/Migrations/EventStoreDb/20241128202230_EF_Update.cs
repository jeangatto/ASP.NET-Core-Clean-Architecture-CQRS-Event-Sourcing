using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.PublicApi.Migrations.EventStoreDb;

/// <inheritdoc />
public partial class EF_Update : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Data",
            table: "EventStores",
            type: "varchar(255)",
            unicode: false,
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "VARCHAR(MAX)",
            oldUnicode: false,
            comment: "JSON serialized event",
            oldComment: "JSON serialized event");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Data",
            table: "EventStores",
            type: "VARCHAR(MAX)",
            unicode: false,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(255)",
            oldUnicode: false,
            oldMaxLength: 255,
            comment: "JSON serialized event",
            oldComment: "JSON serialized event");
    }
}