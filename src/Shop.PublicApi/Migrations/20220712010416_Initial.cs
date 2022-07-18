using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.PublicApi.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CatalogBrands",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Brand = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_CatalogBrands", x => x.Id));

        migrationBuilder.CreateTable(
            name: "CatalogTypes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_CatalogTypes", x => x.Id));

        migrationBuilder.CreateTable(
            name: "CatalogItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CatalogTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CatalogBrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                PictureUri = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CatalogItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_CatalogItems_CatalogBrands_CatalogBrandId",
                    column: x => x.CatalogBrandId,
                    principalTable: "CatalogBrands",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_CatalogItems_CatalogTypes_CatalogTypeId",
                    column: x => x.CatalogTypeId,
                    principalTable: "CatalogTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CatalogItems_CatalogBrandId",
            table: "CatalogItems",
            column: "CatalogBrandId");

        migrationBuilder.CreateIndex(
            name: "IX_CatalogItems_CatalogTypeId",
            table: "CatalogItems",
            column: "CatalogTypeId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CatalogItems");
        migrationBuilder.DropTable(name: "CatalogBrands");
        migrationBuilder.DropTable(name: "CatalogTypes");
    }
}