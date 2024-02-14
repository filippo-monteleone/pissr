using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AziendeAgricole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Indirizzo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AziendeAgricole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AziendeIdriche",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Indirizzo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AziendeIdriche", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Ettari = table.Column<int>(type: "integer", nullable: false),
                    Altitudine = table.Column<int>(type: "integer", nullable: false),
                    Attivo = table.Column<bool>(type: "boolean", nullable: false),
                    Adattivo = table.Column<bool>(type: "boolean", nullable: false),
                    TipoSistemaIrrigazione = table.Column<string>(type: "text", nullable: false),
                    Ae = table.Column<int>(type: "integer", nullable: false),
                    Fr = table.Column<float>(type: "real", nullable: false),
                    S1 = table.Column<float>(type: "real", nullable: false),
                    S2 = table.Column<float>(type: "real", nullable: false),
                    Aws = table.Column<int>(type: "integer", nullable: false),
                    Rd = table.Column<double>(type: "double precision", nullable: false),
                    Ac = table.Column<float>(type: "real", nullable: false),
                    Hum = table.Column<double>(type: "double precision", nullable: false),
                    Kc = table.Column<double>(type: "double precision", nullable: false),
                    SoilMoisture = table.Column<double>(type: "double precision", nullable: true),
                    AziendaAgricolaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campi_AziendeAgricole_AziendaAgricolaId",
                        column: x => x.AziendaAgricolaId,
                        principalTable: "AziendeAgricole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contratti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tempo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Acqua = table.Column<float>(type: "real", nullable: false),
                    Fine = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Stato = table.Column<int>(type: "integer", nullable: false),
                    AziendaAgricolaId = table.Column<int>(type: "integer", nullable: false),
                    AziendaIdricaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contratti_AziendeAgricole_AziendaAgricolaId",
                        column: x => x.AziendaAgricolaId,
                        principalTable: "AziendeAgricole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contratti_AziendeIdriche_AziendaIdricaId",
                        column: x => x.AziendaIdricaId,
                        principalTable: "AziendeIdriche",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dispositivi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    CampoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispositivi_Campi_CampoId",
                        column: x => x.CampoId,
                        principalTable: "Campi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Eventi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Tempo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Valore = table.Column<string>(type: "text", nullable: true),
                    DispositivoId = table.Column<int>(type: "integer", nullable: false),
                    CampoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventi_Campi_CampoId",
                        column: x => x.CampoId,
                        principalTable: "Campi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Eventi_Dispositivi_DispositivoId",
                        column: x => x.DispositivoId,
                        principalTable: "Dispositivi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campi_AziendaAgricolaId",
                table: "Campi",
                column: "AziendaAgricolaId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratti_AziendaAgricolaId",
                table: "Contratti",
                column: "AziendaAgricolaId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratti_AziendaIdricaId",
                table: "Contratti",
                column: "AziendaIdricaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivi_CampoId",
                table: "Dispositivi",
                column: "CampoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventi_CampoId",
                table: "Eventi",
                column: "CampoId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventi_DispositivoId",
                table: "Eventi",
                column: "DispositivoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contratti");

            migrationBuilder.DropTable(
                name: "Eventi");

            migrationBuilder.DropTable(
                name: "AziendeIdriche");

            migrationBuilder.DropTable(
                name: "Dispositivi");

            migrationBuilder.DropTable(
                name: "Campi");

            migrationBuilder.DropTable(
                name: "AziendeAgricole");
        }
    }
}
