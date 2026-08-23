using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaMultimedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MultiFormatosYPlataformasPorItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_formats_FormatId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_items_platforms_PlatformId",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_FormatId",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_PlatformId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "FormatId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "PlatformId",
                table: "items");

            migrationBuilder.CreateTable(
                name: "item_formats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormatId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_formats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_item_formats_formats_FormatId",
                        column: x => x.FormatId,
                        principalTable: "formats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_formats_items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_platforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_platforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_item_platforms_items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_platforms_platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_item_formats_FormatId",
                table: "item_formats",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_item_formats_ItemId_FormatId",
                table: "item_formats",
                columns: new[] { "ItemId", "FormatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_platforms_ItemId_PlatformId",
                table: "item_platforms",
                columns: new[] { "ItemId", "PlatformId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_platforms_PlatformId",
                table: "item_platforms",
                column: "PlatformId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_formats");

            migrationBuilder.DropTable(
                name: "item_platforms");

            migrationBuilder.AddColumn<Guid>(
                name: "FormatId",
                table: "items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformId",
                table: "items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_FormatId",
                table: "items",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_items_PlatformId",
                table: "items",
                column: "PlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_formats_FormatId",
                table: "items",
                column: "FormatId",
                principalTable: "formats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_items_platforms_PlatformId",
                table: "items",
                column: "PlatformId",
                principalTable: "platforms",
                principalColumn: "Id");
        }
    }
}
