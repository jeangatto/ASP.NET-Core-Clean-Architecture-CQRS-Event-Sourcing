using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.PublicApi.Migrations.EventStoreDb;

/// <inheritdoc />
public partial class EventStore : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EventStores",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Data = table.Column<string>(type: "VARCHAR(MAX)", unicode: false, nullable: false, comment: "JSON serialized event"),
                MessageType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_EventStores", x => x.Id));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "EventStores");
    }
}