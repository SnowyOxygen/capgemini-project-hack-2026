using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSitesAndRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "facteurs_energie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeEnergie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FacteurEmission = table.Column<decimal>(type: "numeric(10,5)", nullable: true),
                    Unite = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facteurs_energie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "materiaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FacteurEmission = table.Column<decimal>(type: "numeric(10,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materiaux", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TypeSite = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AnneeConstruction = table.Column<int>(type: "integer", nullable: true),
                    SuperficieM2 = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    NombreEtages = table.Column<int>(type: "integer", nullable: true),
                    NombrePersonnes = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "energies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiteId = table.Column<int>(type: "integer", nullable: false),
                    TypeEnergie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsommationAnnuelle = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    Unite = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TypeDonnee = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_energies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_energies_sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parkings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiteId = table.Column<int>(type: "integer", nullable: false),
                    NombrePlacesTotal = table.Column<int>(type: "integer", nullable: true),
                    PlacesAeriennes = table.Column<int>(type: "integer", nullable: true),
                    PlacesSousDalle = table.Column<int>(type: "integer", nullable: true),
                    PlacesSousSol = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parkings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parkings_sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "site_materiaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiteId = table.Column<int>(type: "integer", nullable: false),
                    MateriauId = table.Column<int>(type: "integer", nullable: false),
                    Quantite = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    Unite = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_materiaux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_site_materiaux_materiaux_MateriauId",
                        column: x => x.MateriauId,
                        principalTable: "materiaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_site_materiaux_sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_energies_SiteId",
                table: "energies",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_parkings_SiteId",
                table: "parkings",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_site_materiaux_MateriauId",
                table: "site_materiaux",
                column: "MateriauId");

            migrationBuilder.CreateIndex(
                name: "IX_site_materiaux_SiteId",
                table: "site_materiaux",
                column: "SiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "energies");

            migrationBuilder.DropTable(
                name: "facteurs_energie");

            migrationBuilder.DropTable(
                name: "parkings");

            migrationBuilder.DropTable(
                name: "site_materiaux");

            migrationBuilder.DropTable(
                name: "materiaux");

            migrationBuilder.DropTable(
                name: "sites");
        }
    }
}
