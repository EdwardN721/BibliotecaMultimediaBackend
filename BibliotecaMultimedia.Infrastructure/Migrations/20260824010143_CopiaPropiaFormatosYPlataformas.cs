using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaMultimedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CopiaPropiaFormatosYPlataformas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_item_formats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormatId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_item_formats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_item_formats_formats_FormatId",
                        column: x => x.FormatId,
                        principalTable: "formats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_item_formats_user_items_UserItemId",
                        column: x => x.UserItemId,
                        principalTable: "user_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_item_platforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_item_platforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_item_platforms_platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_item_platforms_user_items_UserItemId",
                        column: x => x.UserItemId,
                        principalTable: "user_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_item_formats_FormatId",
                table: "user_item_formats",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_user_item_formats_UserItemId_FormatId",
                table: "user_item_formats",
                columns: new[] { "UserItemId", "FormatId" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_item_platforms_PlatformId",
                table: "user_item_platforms",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_user_item_platforms_UserItemId_PlatformId",
                table: "user_item_platforms",
                columns: new[] { "UserItemId", "PlatformId" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_item_formats");

            migrationBuilder.DropTable(
                name: "user_item_platforms");
        }
    }
}
