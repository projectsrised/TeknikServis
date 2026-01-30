using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeknikServisApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class KategoriGlobalMusteriTedarikci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kategoriler_Tenants_TenantId",
                table: "Kategoriler");

            migrationBuilder.DropIndex(
                name: "IX_Kategoriler_TenantId",
                table: "Kategoriler");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Kategoriler");

            // Önce boş email'leri placeholder ile doldur
            migrationBuilder.Sql("UPDATE \"Musteriler\" SET \"Email\" = 'musteri_' || \"Id\" || '@temp.com' WHERE \"Email\" IS NULL OR \"Email\" = ''");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Musteriler",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Tedarikci",
                table: "Musteriler",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tedarikci",
                table: "Musteriler");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Musteriler",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Kategoriler",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_TenantId",
                table: "Kategoriler",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kategoriler_Tenants_TenantId",
                table: "Kategoriler",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
