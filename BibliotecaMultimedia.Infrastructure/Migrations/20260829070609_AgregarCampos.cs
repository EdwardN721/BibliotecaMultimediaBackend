using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaMultimedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "creators",
                type: "character varying(1500)",
                maxLength: 1500,
                nullable: true,
                defaultValue: "Sin Descripción.",
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 1500,
                oldNullable: true,
                oldDefaultValue: "Sin Descripción.");

            migrationBuilder.CreateIndex(
                name: "IX_creators_Name",
                table: "creators",
                column: "Name",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_creators_Name",
                table: "creators");

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "creators",
                type: "text",
                maxLength: 1500,
                nullable: true,
                defaultValue: "Sin Descripción.",
                oldClrType: typeof(string),
                oldType: "character varying(1500)",
                oldMaxLength: 1500,
                oldNullable: true,
                oldDefaultValue: "Sin Descripción.");
        }
    }
}
