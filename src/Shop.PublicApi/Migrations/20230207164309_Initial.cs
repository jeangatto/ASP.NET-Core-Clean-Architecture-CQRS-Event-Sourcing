using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.PublicApi.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FirstName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                Gender = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: false),
                Email = table.Column<string>(type: "varchar(254)", unicode: false, maxLength: 254, nullable: true),
                DateOfBirth = table.Column<DateTime>(type: "DATE", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Customers", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Customers_Email",
            table: "Customers",
            column: "Email",
            unique: true,
            filter: "[Email] IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Customers");
    }
}