using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeknikServisApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMarkaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MarkaId",
                table: "Urunler",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId",
                table: "Urunler",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Markalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    Sira = table.Column<int>(type: "integer", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Markalar_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modeller",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MarkaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: true),
                    Sira = table.Column<int>(type: "integer", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modeller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modeller_Markalar_MarkaId",
                        column: x => x.MarkaId,
                        principalTable: "Markalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Modeller_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_MarkaId",
                table: "Urunler",
                column: "MarkaId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_ModelId",
                table: "Urunler",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Markalar_Ad",
                table: "Markalar",
                column: "Ad");

            migrationBuilder.CreateIndex(
                name: "IX_Markalar_TenantId",
                table: "Markalar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Modeller_Ad",
                table: "Modeller",
                column: "Ad");

            migrationBuilder.CreateIndex(
                name: "IX_Modeller_MarkaId",
                table: "Modeller",
                column: "MarkaId");

            migrationBuilder.CreateIndex(
                name: "IX_Modeller_TenantId",
                table: "Modeller",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Urunler_Markalar_MarkaId",
                table: "Urunler",
                column: "MarkaId",
                principalTable: "Markalar",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Urunler_Modeller_ModelId",
                table: "Urunler",
                column: "ModelId",
                principalTable: "Modeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Urunler_Markalar_MarkaId",
                table: "Urunler");

            migrationBuilder.DropForeignKey(
                name: "FK_Urunler_Modeller_ModelId",
                table: "Urunler");

            migrationBuilder.DropTable(
                name: "Modeller");

            migrationBuilder.DropTable(
                name: "Markalar");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_MarkaId",
                table: "Urunler");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_ModelId",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "MarkaId",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Urunler");
        }
    }
}
