using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeknikServisApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: true),
                    VergiNo = table.Column<string>(type: "text", nullable: true),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    LisansBitisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bayiler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: true),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    Il = table.Column<string>(type: "text", nullable: true),
                    Ilce = table.Column<string>(type: "text", nullable: true),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    YetkiliAd = table.Column<string>(type: "text", nullable: true),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    MerkezMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bayiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bayiler_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    Tip = table.Column<int>(type: "integer", nullable: false),
                    UstKategoriId = table.Column<Guid>(type: "uuid", nullable: true),
                    Sira = table.Column<int>(type: "integer", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kategoriler_Kategoriler_UstKategoriId",
                        column: x => x.UstKategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Kategoriler_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Depolar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: true),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    MerkezDepoMu = table.Column<bool>(type: "boolean", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depolar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Depolar_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Depolar_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kasalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Bakiye = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kasalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kasalar_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kasalar_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: true),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Soyad = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    SifreHash = table.Column<string>(type: "text", nullable: false),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    Rol = table.Column<int>(type: "integer", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenSonKullanma = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Maas = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    YillikIzinHakki = table.Column<int>(type: "integer", nullable: false),
                    IseBaslamaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Musteriler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: true),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Soyad = table.Column<string>(type: "text", nullable: false),
                    Telefon = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    TcNo = table.Column<string>(type: "text", nullable: true),
                    VergiNo = table.Column<string>(type: "text", nullable: true),
                    Kurumsal = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musteriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Musteriler_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Musteriler_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KategoriId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Kod = table.Column<string>(type: "text", nullable: true),
                    Barkod = table.Column<string>(type: "text", nullable: true),
                    Marka = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    AlisFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SatisFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvOran = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    KritikStok = table.Column<int>(type: "integer", nullable: false),
                    SeriTakipli = table.Column<bool>(type: "boolean", nullable: false),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Urunler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Urunler_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KasaHareketleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KasaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tip = table.Column<int>(type: "integer", nullable: false),
                    Tutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OncekiBakiye = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SonrakiBakiye = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BelgeNo = table.Column<string>(type: "text", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    OlusturanId = table.Column<Guid>(type: "uuid", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KasaHareketleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KasaHareketleri_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KasaHareketleri_Kasalar_KasaId",
                        column: x => x.KasaId,
                        principalTable: "Kasalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KasaHareketleri_Kullanicilar_OlusturanId",
                        column: x => x.OlusturanId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KasaHareketleri_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelAvanslari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TalepTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OdemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnaylandiMi = table.Column<bool>(type: "boolean", nullable: false),
                    OdendiMi = table.Column<bool>(type: "boolean", nullable: false),
                    MaastanKesildiMi = table.Column<bool>(type: "boolean", nullable: false),
                    OnaylayanId = table.Column<Guid>(type: "uuid", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelAvanslari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelAvanslari_Kullanicilar_OnaylayanId",
                        column: x => x.OnaylayanId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonelAvanslari_Kullanicilar_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonelAvanslari_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelIzinleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uuid", nullable: false),
                    IzinTipi = table.Column<int>(type: "integer", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GunSayisi = table.Column<int>(type: "integer", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnaylayanId = table.Column<Guid>(type: "uuid", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    RedNedeni = table.Column<string>(type: "text", nullable: true),
                    UcretliMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelIzinleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelIzinleri_Kullanicilar_OnaylayanId",
                        column: x => x.OnaylayanId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonelIzinleri_Kullanicilar_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonelIzinleri_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelMaasOdemeleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Yil = table.Column<int>(type: "integer", nullable: false),
                    Ay = table.Column<int>(type: "integer", nullable: false),
                    BrutMaas = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetMaas = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EkMesaiUcreti = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Kesinti = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AvansKesinti = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ToplamOdeme = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OdendiMi = table.Column<bool>(type: "boolean", nullable: false),
                    OdemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OdeyenId = table.Column<Guid>(type: "uuid", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelMaasOdemeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelMaasOdemeleri_Kullanicilar_OdeyenId",
                        column: x => x.OdeyenId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonelMaasOdemeleri_Kullanicilar_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonelMaasOdemeleri_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelMesaileri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "interval", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ToplamSaat = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    SaatUcreti = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ToplamUcret = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    OnaylandiMi = table.Column<bool>(type: "boolean", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OnaylayanId = table.Column<Guid>(type: "uuid", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelMesaileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelMesaileri_Kullanicilar_OnaylayanId",
                        column: x => x.OnaylayanId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonelMesaileri_Kullanicilar_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonelMesaileri_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sayimlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SayimNo = table.Column<string>(type: "text", nullable: false),
                    DepoId = table.Column<Guid>(type: "uuid", nullable: false),
                    BaslatanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SistemStok = table.Column<int>(type: "integer", nullable: false),
                    FizikselStok = table.Column<int>(type: "integer", nullable: false),
                    Fark = table.Column<int>(type: "integer", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sayimlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sayimlar_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sayimlar_Depolar_DepoId",
                        column: x => x.DepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sayimlar_Kullanicilar_BaslatanId",
                        column: x => x.BaslatanId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sayimlar_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transferler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferNo = table.Column<string>(type: "text", nullable: false),
                    KaynakDepoId = table.Column<Guid>(type: "uuid", nullable: false),
                    HedefDepoId = table.Column<Guid>(type: "uuid", nullable: false),
                    OlusturanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    TransferTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    IptalNedeni = table.Column<string>(type: "text", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transferler_Depolar_HedefDepoId",
                        column: x => x.HedefDepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferler_Depolar_KaynakDepoId",
                        column: x => x.KaynakDepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferler_Kullanicilar_OlusturanId",
                        column: x => x.OlusturanId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transferler_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Satislar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SatisNo = table.Column<string>(type: "text", nullable: false),
                    MusteriId = table.Column<Guid>(type: "uuid", nullable: true),
                    MusteriAd = table.Column<string>(type: "text", nullable: true),
                    MusteriTelefon = table.Column<string>(type: "text", nullable: true),
                    SatisYapanId = table.Column<Guid>(type: "uuid", nullable: false),
                    SatisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OdemeTipi = table.Column<int>(type: "integer", nullable: false),
                    AraToplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvToplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IndirimTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    GenelToplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NakitTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    KartTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    IptalEdildi = table.Column<bool>(type: "boolean", nullable: false),
                    IptalNedeni = table.Column<string>(type: "text", nullable: true),
                    IptalTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Satislar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Satislar_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Satislar_Kullanicilar_SatisYapanId",
                        column: x => x.SatisYapanId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Satislar_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Satislar_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeknikServisler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServisNo = table.Column<string>(type: "text", nullable: false),
                    MusteriId = table.Column<Guid>(type: "uuid", nullable: true),
                    MusteriAd = table.Column<string>(type: "text", nullable: true),
                    MusteriTelefon = table.Column<string>(type: "text", nullable: true),
                    SorumluPersonelId = table.Column<Guid>(type: "uuid", nullable: true),
                    CihazTip = table.Column<string>(type: "text", nullable: false),
                    CihazMarka = table.Column<string>(type: "text", nullable: true),
                    CihazModel = table.Column<string>(type: "text", nullable: true),
                    SeriNo = table.Column<string>(type: "text", nullable: true),
                    ImeiNo = table.Column<string>(type: "text", nullable: true),
                    CihazSifre = table.Column<string>(type: "text", nullable: true),
                    Ariza = table.Column<string>(type: "text", nullable: false),
                    ArizaDetay = table.Column<string>(type: "text", nullable: true),
                    TespitNotlari = table.Column<string>(type: "text", nullable: true),
                    YapilanIslem = table.Column<string>(type: "text", nullable: true),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    GirisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TahminiTeslimTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TeslimTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IscilikUcreti = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ParcaToplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OdenenTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OdemeAlindi = table.Column<bool>(type: "boolean", nullable: false),
                    OdemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GarantiKapsami = table.Column<bool>(type: "boolean", nullable: false),
                    GarantiNot = table.Column<string>(type: "text", nullable: true),
                    IptalNedeni = table.Column<string>(type: "text", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeknikServisler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeknikServisler_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeknikServisler_Kullanicilar_SorumluPersonelId",
                        column: x => x.SorumluPersonelId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeknikServisler_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeknikServisler_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriNumaralari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: true),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepoId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeriNo = table.Column<string>(type: "text", nullable: false),
                    AlisFiyati = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    SatisFiyati = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    Satildi = table.Column<bool>(type: "boolean", nullable: false),
                    SatisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SatisId = table.Column<Guid>(type: "uuid", nullable: true),
                    GirisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriNumaralari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeriNumaralari_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SeriNumaralari_Depolar_DepoId",
                        column: x => x.DepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SeriNumaralari_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriNumaralari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Iadeler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IadeNo = table.Column<string>(type: "text", nullable: false),
                    SatisId = table.Column<Guid>(type: "uuid", nullable: false),
                    MusteriId = table.Column<Guid>(type: "uuid", nullable: true),
                    IslemYapanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    IadeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ToplamTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IadeEdilen = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Neden = table.Column<string>(type: "text", nullable: true),
                    RedNedeni = table.Column<string>(type: "text", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iadeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Iadeler_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Iadeler_Kullanicilar_IslemYapanId",
                        column: x => x.IslemYapanId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Iadeler_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Iadeler_Satislar_SatisId",
                        column: x => x.SatisId,
                        principalTable: "Satislar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Iadeler_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Faturalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FaturaNo = table.Column<string>(type: "text", nullable: false),
                    Tip = table.Column<int>(type: "integer", nullable: false),
                    MusteriId = table.Column<Guid>(type: "uuid", nullable: true),
                    MusteriAd = table.Column<string>(type: "text", nullable: true),
                    MusteriAdres = table.Column<string>(type: "text", nullable: true),
                    MusteriVergiNo = table.Column<string>(type: "text", nullable: true),
                    SatisId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeknikServisId = table.Column<Guid>(type: "uuid", nullable: true),
                    FaturaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AraToplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvToplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IndirimTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    GenelToplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Onaylandi = table.Column<bool>(type: "boolean", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IptalEdildi = table.Column<bool>(type: "boolean", nullable: false),
                    IptalNedeni = table.Column<string>(type: "text", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faturalar_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Faturalar_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Faturalar_Satislar_SatisId",
                        column: x => x.SatisId,
                        principalTable: "Satislar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Faturalar_TeknikServisler_TeknikServisId",
                        column: x => x.TeknikServisId,
                        principalTable: "TeknikServisler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Faturalar_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeknikServisKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeknikServisId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParcaAdi = table.Column<string>(type: "text", nullable: true),
                    Miktar = table.Column<int>(type: "integer", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Toplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeknikServisKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeknikServisKalemleri_TeknikServisler_TeknikServisId",
                        column: x => x.TeknikServisId,
                        principalTable: "TeknikServisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeknikServisKalemleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SatisKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SatisId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriNumarasiId = table.Column<Guid>(type: "uuid", nullable: true),
                    Miktar = table.Column<int>(type: "integer", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvOran = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    KdvTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IndirimTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Toplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatisKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SatisKalemleri_Satislar_SatisId",
                        column: x => x.SatisId,
                        principalTable: "Satislar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SatisKalemleri_SeriNumaralari_SeriNumarasiId",
                        column: x => x.SeriNumarasiId,
                        principalTable: "SeriNumaralari",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SatisKalemleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SayimKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SayimId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriNumarasiId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeriNo = table.Column<string>(type: "text", nullable: false),
                    SistemdeVar = table.Column<bool>(type: "boolean", nullable: false),
                    FizikiSayimda = table.Column<bool>(type: "boolean", nullable: false),
                    TaramaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SayimKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SayimKalemleri_Sayimlar_SayimId",
                        column: x => x.SayimId,
                        principalTable: "Sayimlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SayimKalemleri_SeriNumaralari_SeriNumarasiId",
                        column: x => x.SeriNumarasiId,
                        principalTable: "SeriNumaralari",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SayimKalemleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StokHareketleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriNumarasiId = table.Column<Guid>(type: "uuid", nullable: true),
                    Tip = table.Column<int>(type: "integer", nullable: false),
                    Miktar = table.Column<int>(type: "integer", nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BayiId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StokHareketleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StokHareketleri_Bayiler_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StokHareketleri_Depolar_DepoId",
                        column: x => x.DepoId,
                        principalTable: "Depolar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StokHareketleri_SeriNumaralari_SeriNumarasiId",
                        column: x => x.SeriNumarasiId,
                        principalTable: "SeriNumaralari",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StokHareketleri_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StokHareketleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriNumarasiId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeslimAlindi = table.Column<bool>(type: "boolean", nullable: false),
                    TeslimTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferKalemleri_SeriNumaralari_SeriNumarasiId",
                        column: x => x.SeriNumarasiId,
                        principalTable: "SeriNumaralari",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferKalemleri_Transferler_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Transferler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferKalemleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaturaKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FaturaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrunId = table.Column<Guid>(type: "uuid", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    Miktar = table.Column<int>(type: "integer", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvOran = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    KdvTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IndirimTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Toplam = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaturaKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaturaKalemleri_Faturalar_FaturaId",
                        column: x => x.FaturaId,
                        principalTable: "Faturalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaturaKalemleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IadeKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SatisKalemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriNumarasiId = table.Column<Guid>(type: "uuid", nullable: true),
                    IadeTutar = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Neden = table.Column<string>(type: "text", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Silindi = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IadeKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IadeKalemleri_Iadeler_IadeId",
                        column: x => x.IadeId,
                        principalTable: "Iadeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IadeKalemleri_SatisKalemleri_SatisKalemId",
                        column: x => x.SatisKalemId,
                        principalTable: "SatisKalemleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IadeKalemleri_SeriNumaralari_SeriNumarasiId",
                        column: x => x.SeriNumarasiId,
                        principalTable: "SeriNumaralari",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bayiler_TenantId",
                table: "Bayiler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Depolar_BayiId",
                table: "Depolar",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Depolar_TenantId",
                table: "Depolar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FaturaKalemleri_FaturaId",
                table: "FaturaKalemleri",
                column: "FaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FaturaKalemleri_UrunId",
                table: "FaturaKalemleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_BayiId",
                table: "Faturalar",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_FaturaNo",
                table: "Faturalar",
                column: "FaturaNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_MusteriId",
                table: "Faturalar",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_SatisId",
                table: "Faturalar",
                column: "SatisId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_TeknikServisId",
                table: "Faturalar",
                column: "TeknikServisId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_TenantId",
                table: "Faturalar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IadeKalemleri_IadeId",
                table: "IadeKalemleri",
                column: "IadeId");

            migrationBuilder.CreateIndex(
                name: "IX_IadeKalemleri_SatisKalemId",
                table: "IadeKalemleri",
                column: "SatisKalemId");

            migrationBuilder.CreateIndex(
                name: "IX_IadeKalemleri_SeriNumarasiId",
                table: "IadeKalemleri",
                column: "SeriNumarasiId");

            migrationBuilder.CreateIndex(
                name: "IX_Iadeler_BayiId",
                table: "Iadeler",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Iadeler_IslemYapanId",
                table: "Iadeler",
                column: "IslemYapanId");

            migrationBuilder.CreateIndex(
                name: "IX_Iadeler_MusteriId",
                table: "Iadeler",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_Iadeler_SatisId",
                table: "Iadeler",
                column: "SatisId");

            migrationBuilder.CreateIndex(
                name: "IX_Iadeler_TenantId",
                table: "Iadeler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_KasaHareketleri_BayiId",
                table: "KasaHareketleri",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_KasaHareketleri_KasaId",
                table: "KasaHareketleri",
                column: "KasaId");

            migrationBuilder.CreateIndex(
                name: "IX_KasaHareketleri_OlusturanId",
                table: "KasaHareketleri",
                column: "OlusturanId");

            migrationBuilder.CreateIndex(
                name: "IX_KasaHareketleri_TenantId",
                table: "KasaHareketleri",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Kasalar_BayiId",
                table: "Kasalar",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Kasalar_TenantId",
                table: "Kasalar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_TenantId",
                table: "Kategoriler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_UstKategoriId",
                table: "Kategoriler",
                column: "UstKategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_BayiId",
                table: "Kullanicilar",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Email",
                table: "Kullanicilar",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_TenantId",
                table: "Kullanicilar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Musteriler_BayiId",
                table: "Musteriler",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Musteriler_TenantId",
                table: "Musteriler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelAvanslari_OnaylayanId",
                table: "PersonelAvanslari",
                column: "OnaylayanId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelAvanslari_PersonelId",
                table: "PersonelAvanslari",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelAvanslari_TenantId",
                table: "PersonelAvanslari",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelIzinleri_OnaylayanId",
                table: "PersonelIzinleri",
                column: "OnaylayanId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelIzinleri_PersonelId",
                table: "PersonelIzinleri",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelIzinleri_TenantId",
                table: "PersonelIzinleri",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMaasOdemeleri_OdeyenId",
                table: "PersonelMaasOdemeleri",
                column: "OdeyenId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMaasOdemeleri_PersonelId",
                table: "PersonelMaasOdemeleri",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMaasOdemeleri_TenantId",
                table: "PersonelMaasOdemeleri",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMesaileri_OnaylayanId",
                table: "PersonelMesaileri",
                column: "OnaylayanId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMesaileri_PersonelId",
                table: "PersonelMesaileri",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMesaileri_TenantId",
                table: "PersonelMesaileri",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SatisKalemleri_SatisId",
                table: "SatisKalemleri",
                column: "SatisId");

            migrationBuilder.CreateIndex(
                name: "IX_SatisKalemleri_SeriNumarasiId",
                table: "SatisKalemleri",
                column: "SeriNumarasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SatisKalemleri_UrunId",
                table: "SatisKalemleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Satislar_BayiId",
                table: "Satislar",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Satislar_MusteriId",
                table: "Satislar",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_Satislar_SatisNo",
                table: "Satislar",
                column: "SatisNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Satislar_SatisYapanId",
                table: "Satislar",
                column: "SatisYapanId");

            migrationBuilder.CreateIndex(
                name: "IX_Satislar_TenantId",
                table: "Satislar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SayimKalemleri_SayimId",
                table: "SayimKalemleri",
                column: "SayimId");

            migrationBuilder.CreateIndex(
                name: "IX_SayimKalemleri_SeriNumarasiId",
                table: "SayimKalemleri",
                column: "SeriNumarasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SayimKalemleri_UrunId",
                table: "SayimKalemleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Sayimlar_BaslatanId",
                table: "Sayimlar",
                column: "BaslatanId");

            migrationBuilder.CreateIndex(
                name: "IX_Sayimlar_BayiId",
                table: "Sayimlar",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Sayimlar_DepoId",
                table: "Sayimlar",
                column: "DepoId");

            migrationBuilder.CreateIndex(
                name: "IX_Sayimlar_TenantId",
                table: "Sayimlar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriNumaralari_BayiId",
                table: "SeriNumaralari",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriNumaralari_DepoId",
                table: "SeriNumaralari",
                column: "DepoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriNumaralari_SeriNo",
                table: "SeriNumaralari",
                column: "SeriNo");

            migrationBuilder.CreateIndex(
                name: "IX_SeriNumaralari_TenantId",
                table: "SeriNumaralari",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriNumaralari_UrunId",
                table: "SeriNumaralari",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_BayiId",
                table: "StokHareketleri",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_DepoId",
                table: "StokHareketleri",
                column: "DepoId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_SeriNumarasiId",
                table: "StokHareketleri",
                column: "SeriNumarasiId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_TenantId",
                table: "StokHareketleri",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_UrunId",
                table: "StokHareketleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_TeknikServisKalemleri_TeknikServisId",
                table: "TeknikServisKalemleri",
                column: "TeknikServisId");

            migrationBuilder.CreateIndex(
                name: "IX_TeknikServisKalemleri_UrunId",
                table: "TeknikServisKalemleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_TeknikServisler_BayiId",
                table: "TeknikServisler",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_TeknikServisler_MusteriId",
                table: "TeknikServisler",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_TeknikServisler_ServisNo",
                table: "TeknikServisler",
                column: "ServisNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeknikServisler_SorumluPersonelId",
                table: "TeknikServisler",
                column: "SorumluPersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_TeknikServisler_TenantId",
                table: "TeknikServisler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferKalemleri_SeriNumarasiId",
                table: "TransferKalemleri",
                column: "SeriNumarasiId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferKalemleri_TransferId",
                table: "TransferKalemleri",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferKalemleri_UrunId",
                table: "TransferKalemleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferler_HedefDepoId",
                table: "Transferler",
                column: "HedefDepoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferler_KaynakDepoId",
                table: "Transferler",
                column: "KaynakDepoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferler_OlusturanId",
                table: "Transferler",
                column: "OlusturanId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferler_TenantId",
                table: "Transferler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferler_TransferNo",
                table: "Transferler",
                column: "TransferNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_Barkod",
                table: "Urunler",
                column: "Barkod");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_KategoriId",
                table: "Urunler",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_TenantId",
                table: "Urunler",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaturaKalemleri");

            migrationBuilder.DropTable(
                name: "IadeKalemleri");

            migrationBuilder.DropTable(
                name: "KasaHareketleri");

            migrationBuilder.DropTable(
                name: "PersonelAvanslari");

            migrationBuilder.DropTable(
                name: "PersonelIzinleri");

            migrationBuilder.DropTable(
                name: "PersonelMaasOdemeleri");

            migrationBuilder.DropTable(
                name: "PersonelMesaileri");

            migrationBuilder.DropTable(
                name: "SayimKalemleri");

            migrationBuilder.DropTable(
                name: "StokHareketleri");

            migrationBuilder.DropTable(
                name: "TeknikServisKalemleri");

            migrationBuilder.DropTable(
                name: "TransferKalemleri");

            migrationBuilder.DropTable(
                name: "Faturalar");

            migrationBuilder.DropTable(
                name: "Iadeler");

            migrationBuilder.DropTable(
                name: "SatisKalemleri");

            migrationBuilder.DropTable(
                name: "Kasalar");

            migrationBuilder.DropTable(
                name: "Sayimlar");

            migrationBuilder.DropTable(
                name: "Transferler");

            migrationBuilder.DropTable(
                name: "TeknikServisler");

            migrationBuilder.DropTable(
                name: "Satislar");

            migrationBuilder.DropTable(
                name: "SeriNumaralari");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Musteriler");

            migrationBuilder.DropTable(
                name: "Depolar");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Bayiler");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
