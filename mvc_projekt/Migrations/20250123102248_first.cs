using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc_projekt.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zadanie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tytul = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KategoriaId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ZadanieNadrzedneId = table.Column<int>(type: "int", nullable: true),
                    ZadaniaPodrzedneId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zadanie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zadanie_Kategoria_KategoriaId",
                        column: x => x.KategoriaId,
                        principalTable: "Kategoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zadanie_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zadanie_Zadanie_ZadaniaPodrzedneId",
                        column: x => x.ZadaniaPodrzedneId,
                        principalTable: "Zadanie",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Zadanie_Zadanie_ZadanieNadrzedneId",
                        column: x => x.ZadanieNadrzedneId,
                        principalTable: "Zadanie",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zadanie_KategoriaId",
                table: "Zadanie",
                column: "KategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadanie_StatusId",
                table: "Zadanie",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadanie_ZadaniaPodrzedneId",
                table: "Zadanie",
                column: "ZadaniaPodrzedneId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadanie_ZadanieNadrzedneId",
                table: "Zadanie",
                column: "ZadanieNadrzedneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zadanie");

            migrationBuilder.DropTable(
                name: "Kategoria");

            migrationBuilder.DropTable(
                name: "Status");
        }
    }
}
