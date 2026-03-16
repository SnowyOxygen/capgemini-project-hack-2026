using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmissionTrackingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodeDebut",
                table: "energies",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodeFin",
                table: "energies",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "emission_snapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiteId = table.Column<int>(type: "integer", nullable: false),
                    PeriodeDebut = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodeFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmissionConstruction = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionEnergieElectrique = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionEnergieGaz = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionTotale = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionAnnuelle = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionParM2 = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    EmissionParPersonne = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emission_snapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emission_snapshots_sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "emissions_mensuelles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiteId = table.Column<int>(type: "integer", nullable: false),
                    Annee = table.Column<int>(type: "integer", nullable: false),
                    Mois = table.Column<int>(type: "integer", nullable: false),
                    EmissionConstruction = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionEnergieElectrique = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionEnergieGaz = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionTotale = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    EmissionParM2 = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    EmissionParPersonne = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emissions_mensuelles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emissions_mensuelles_sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_emission_snapshots_SiteId_PeriodeDebut_PeriodeFin",
                table: "emission_snapshots",
                columns: new[] { "SiteId", "PeriodeDebut", "PeriodeFin" });

            migrationBuilder.CreateIndex(
                name: "IX_emissions_mensuelles_SiteId_Annee_Mois",
                table: "emissions_mensuelles",
                columns: new[] { "SiteId", "Annee", "Mois" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emission_snapshots");

            migrationBuilder.DropTable(
                name: "emissions_mensuelles");

            migrationBuilder.DropColumn(
                name: "PeriodeDebut",
                table: "energies");

            migrationBuilder.DropColumn(
                name: "PeriodeFin",
                table: "energies");
        }
    }
}
