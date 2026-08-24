using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaMultimedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Prestamos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prestamos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    NombrePersona = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    FechaPrestamo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FechaDevolucion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prestamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prestamos_user_items_UserItemId",
                        column: x => x.UserItemId,
                        principalTable: "user_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prestamos_UserItemId",
                table: "prestamos",
                column: "UserItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prestamos");
        }
    }
}
