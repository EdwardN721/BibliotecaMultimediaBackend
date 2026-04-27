using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BibliotecaMultimedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "creators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true, defaultValue: "Sin Descripción."),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_creators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "formats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "media_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "platforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FormatId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Rating = table.Column<short>(type: "smallint", nullable: true),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    Metadata = table.Column<JsonDocument>(type: "jsonb", nullable: true, defaultValueSql: "'{}'::jsonb"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_items_formats_FormatId",
                        column: x => x.FormatId,
                        principalTable: "formats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_items_media_types_MediaTypeId",
                        column: x => x.MediaTypeId,
                        principalTable: "media_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_items_platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "platforms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "item_creators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_creators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_item_creators_creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_creators_items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_creators_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_genres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_item_genres_genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_item_genres_items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_item_images_items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_formats_Name",
                table: "formats",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_genres_Name",
                table: "genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_creators_CreatorId",
                table: "item_creators",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_item_creators_ItemId_CreatorId_RoleId",
                table: "item_creators",
                columns: new[] { "ItemId", "CreatorId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_creators_RoleId",
                table: "item_creators",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_item_genres_GenreId",
                table: "item_genres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_item_genres_ItemId_GenreId",
                table: "item_genres",
                columns: new[] { "ItemId", "GenreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_images_ItemId",
                table: "item_images",
                column: "ItemId",
                unique: true,
                filter: "\"IsPrimary\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_items_FormatId",
                table: "items",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_items_MediaTypeId",
                table: "items",
                column: "MediaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_items_PlatformId",
                table: "items",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_items_UserId",
                table: "items",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_media_types_Name",
                table: "media_types",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_platforms_Name",
                table: "platforms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                table: "roles",
                column: "Name",
                unique: true);
            
            // =====================================================================
            // INYECCIÓN DE SCRIPT NATIVO DE POSTGRESQL (AUDITORÍA Y TRIGGERS)
            // =====================================================================
            migrationBuilder.Sql("""
                -- 1. Tabla de bitácora
                CREATE TABLE IF NOT EXISTS audit_logs (
                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    table_name VARCHAR(100) NOT NULL,
                    operation VARCHAR(10) NOT NULL CHECK (operation IN ('INSERT', 'UPDATE', 'DELETE')),
                    record_id UUID NOT NULL,
                    old_values JSONB,
                    new_values JSONB,
                    changed_by UUID,
                    created_at TIMESTAMP WITH TIME ZONE DEFAULT timezone('utc', now())
                );
                CREATE INDEX IF NOT EXISTS idx_audit_logs_table_record ON audit_logs(table_name, record_id);

                -- 2. Función de Timestamp
                CREATE OR REPLACE FUNCTION fn_update_timestamp()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW."UpdatedAt" = timezone('utc', now());
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                -- 3. Función del Trigger de Auditoría
                CREATE OR REPLACE FUNCTION fn_audit_trigger() 
                RETURNS TRIGGER AS $$
                DECLARE
                    v_user_id TEXT;
                    v_user_uuid UUID := NULL;
                BEGIN
                    BEGIN
                        v_user_id := current_setting('app.current_user_id', true);
                        IF v_user_id IS NOT NULL AND v_user_id <> '' THEN
                            v_user_uuid := v_user_id::UUID;
                        END IF;
                    EXCEPTION WHEN OTHERS THEN
                        v_user_uuid := NULL;
                    END;

                    IF TG_OP = 'INSERT' THEN
                        INSERT INTO audit_logs (table_name, operation, record_id, new_values, changed_by)
                        VALUES (TG_TABLE_NAME, TG_OP, NEW."Id", row_to_json(NEW)::JSONB, v_user_uuid);
                        RETURN NEW;
                    ELSIF TG_OP = 'UPDATE' THEN
                        IF row_to_json(OLD)::JSONB = row_to_json(NEW)::JSONB THEN RETURN NEW; END IF;
                        INSERT INTO audit_logs (table_name, operation, record_id, old_values, new_values, changed_by)
                        VALUES (TG_TABLE_NAME, TG_OP, NEW."Id", row_to_json(OLD)::JSONB, row_to_json(NEW)::JSONB, v_user_uuid);
                        RETURN NEW;
                    ELSIF TG_OP = 'DELETE' THEN
                        INSERT INTO audit_logs (table_name, operation, record_id, old_values, changed_by)
                        VALUES (TG_TABLE_NAME, TG_OP, OLD."Id", row_to_json(OLD)::JSONB, v_user_uuid);
                        RETURN OLD;
                    END IF;
                    RETURN NULL;
                END;
                $$ LANGUAGE plpgsql;

                -- 4. Asignar Triggers a las tablas
                -- Timestamp triggers
                CREATE TRIGGER trg_creators_updated_at BEFORE UPDATE ON creators FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();
                CREATE TRIGGER trg_items_updated_at BEFORE UPDATE ON items FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

                -- Audit triggers
                CREATE TRIGGER trg_audit_items AFTER INSERT OR UPDATE OR DELETE ON items FOR EACH ROW EXECUTE FUNCTION fn_audit_trigger();
                CREATE TRIGGER trg_audit_creators AFTER INSERT OR UPDATE OR DELETE ON creators FOR EACH ROW EXECUTE FUNCTION fn_audit_trigger();
                CREATE TRIGGER trg_audit_item_creators AFTER INSERT OR UPDATE OR DELETE ON item_creators FOR EACH ROW EXECUTE FUNCTION fn_audit_trigger();
            """);
        }
        
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "item_creators");

            migrationBuilder.DropTable(
                name: "item_genres");

            migrationBuilder.DropTable(
                name: "item_images");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "creators");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "formats");

            migrationBuilder.DropTable(
                name: "media_types");

            migrationBuilder.DropTable(
                name: "platforms");
            // Revertir inyección nativa de PostgreSQL
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_audit_item_creators ON item_creators;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_audit_creators ON creators;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_audit_items ON items;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_items_updated_at ON items;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_creators_updated_at ON creators;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_audit_trigger();");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_update_timestamp();");
            migrationBuilder.Sql("DROP TABLE IF EXISTS audit_logs;");
        }
    }
}
