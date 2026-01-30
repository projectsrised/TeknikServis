using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.DTOs;

#region Common DTOs
public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class ApiResponseDto<T>
{
    public bool Basarili { get; set; }
    public string? Mesaj { get; set; }
    public T? Data { get; set; }

    public static ApiResponseDto<T> Ok(T data, string? mesaj = null) => new() { Basarili = true, Data = data, Mesaj = mesaj };
    public static ApiResponseDto<T> Fail(string mesaj) => new() { Basarili = false, Mesaj = mesaj };
}
#endregion

#region Auth DTOs
public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Sifre { get; set; } = null!;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public KullaniciDto Kullanici { get; set; } = null!;
}

public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = null!;
}

public class ChangePasswordDto
{
    public string MevcutSifre { get; set; } = null!;
    public string YeniSifre { get; set; } = null!;
}

public class RegisterDto
{
    public Guid? BayiId { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Sifre { get; set; } = null!;
    public string? Telefon { get; set; }
    public UserRole Rol { get; set; }
}

public class KullaniciDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string? TenantAd { get; set; }
    public Guid? BayiId { get; set; }
    public string? BayiAd { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string AdSoyad { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefon { get; set; }
    public UserRole Rol { get; set; }
    public string RolAd { get; set; } = null!;
    public bool Aktif { get; set; }
}
#endregion

#region Marka & Model DTOs
public class MarkaDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = null!;
    public string? Logo { get; set; }
    public int Sira { get; set; }
    public bool Aktif { get; set; }
    public int ModelSayisi { get; set; }
}

public class MarkaCreateDto
{
    public string Ad { get; set; } = null!;
    public string? Logo { get; set; }
    public int Sira { get; set; }
}

public class MarkaUpdateDto
{
    public string? Ad { get; set; }
    public string? Logo { get; set; }
    public int? Sira { get; set; }
    public bool? Aktif { get; set; }
}

public class ModelDto
{
    public Guid Id { get; set; }
    public Guid MarkaId { get; set; }
    public string? MarkaAd { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public int Sira { get; set; }
    public bool Aktif { get; set; }
}

public class ModelCreateDto
{
    public Guid MarkaId { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public int Sira { get; set; }
}

public class ModelUpdateDto
{
    public string? Ad { get; set; }
    public string? Kod { get; set; }
    public int? Sira { get; set; }
    public bool? Aktif { get; set; }
}

public class MarkaModelDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = null!;
    public List<ModelDto> Modeller { get; set; } = new();
}
#endregion

#region Kategori DTOs
public class KategoriDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Aciklama { get; set; }
    public KategoriTip Tip { get; set; }
    public string TipAd { get; set; } = null!;
    public Guid? UstKategoriId { get; set; }
    public string? UstKategoriAd { get; set; }
    public int Sira { get; set; }
    public bool Aktif { get; set; }
}

public class KategoriTreeDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public KategoriTip Tip { get; set; }
    public string TipAd { get; set; } = null!;
    public int Sira { get; set; }
    public bool Aktif { get; set; }
    public List<KategoriTreeDto> AltKategoriler { get; set; } = new();
}

public class KategoriCreateDto
{
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Aciklama { get; set; }
    public KategoriTip Tip { get; set; }
    public Guid? UstKategoriId { get; set; }
    public int Sira { get; set; }
}

public class KategoriUpdateDto
{
    public string? Ad { get; set; }
    public string? Kod { get; set; }
    public string? Aciklama { get; set; }
    public KategoriTip? Tip { get; set; }
    public Guid? UstKategoriId { get; set; }
    public int? Sira { get; set; }
    public bool? Aktif { get; set; }
}
#endregion

#region Urun DTOs
public class UrunDto
{
    public Guid Id { get; set; }
    public Guid KategoriId { get; set; }
    public string? KategoriAd { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }  // Otomatik oluşturulur
    public string? Barkod { get; set; }  // Stok girişinde belirlenir
    public decimal AlisFiyat { get; set; }  // Stok girişinde belirlenir
    public decimal SatisFiyat { get; set; }  // Stok girişinde belirlenir
    public decimal KdvOran { get; set; }
    public int KritikStok { get; set; }
    public bool SeriTakipli { get; set; }
    public bool Aktif { get; set; }
    public int MevcutStok { get; set; }
}

public class UrunCreateDto
{
    public Guid KategoriId { get; set; }
    public string Ad { get; set; } = null!;
    // Kod otomatik oluşturulur
    public int KritikStok { get; set; } = 5;
    public bool SeriTakipli { get; set; } = true;
}

public class UrunUpdateDto
{
    public Guid? KategoriId { get; set; }
    public string? Ad { get; set; }
    public int? KritikStok { get; set; }
    public bool? Aktif { get; set; }
}

public class SeriNumarasiDto
{
    public Guid Id { get; set; }
    public string SeriNo { get; set; } = null!;
    public Guid UrunId { get; set; }
    public string? UrunAd { get; set; }
    public Guid? DepoId { get; set; }
    public string? DepoAd { get; set; }
    public bool Satildi { get; set; }
    public DateTime? SatisTarihi { get; set; }
    public decimal? AlisFiyati { get; set; }
    public decimal? SatisFiyati { get; set; }
}
#endregion

#region Bayi DTOs
public class BayiDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Adres { get; set; }
    public string? Il { get; set; }
    public string? Ilce { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? YetkiliAd { get; set; }
    public bool Aktif { get; set; }
    public bool MerkezMi { get; set; }
}

public class BayiCreateDto
{
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Adres { get; set; }
    public string? Il { get; set; }
    public string? Ilce { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? YetkiliAd { get; set; }
    public bool MerkezMi { get; set; }
}

public class BayiUpdateDto
{
    public string? Ad { get; set; }
    public string? Adres { get; set; }
    public string? Il { get; set; }
    public string? Ilce { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? YetkiliAd { get; set; }
    public bool? Aktif { get; set; }
}
#endregion

#region Depo DTOs
public class DepoDto
{
    public Guid Id { get; set; }
    public Guid BayiId { get; set; }
    public string? BayiAd { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Adres { get; set; }
    public bool MerkezDepoMu { get; set; }
    public bool Aktif { get; set; }
}

public class DepoCreateDto
{
    public Guid BayiId { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Adres { get; set; }
    public bool MerkezDepoMu { get; set; }
}

public class DepoUpdateDto
{
    public string? Ad { get; set; }
    public string? Adres { get; set; }
    public bool? Aktif { get; set; }
}
#endregion

#region Musteri DTOs
public class MusteriDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string AdSoyad { get; set; } = null!;
    public string Telefon { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Adres { get; set; }
    public string? TcNo { get; set; }
    public string? VergiNo { get; set; }
    public bool Kurumsal { get; set; }
    public bool Tedarikci { get; set; }
}

public class MusteriCreateDto
{
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string Telefon { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Adres { get; set; }
    public string? TcNo { get; set; }
    public string? VergiNo { get; set; }
    public bool Kurumsal { get; set; }
    public bool Tedarikci { get; set; }
}

public class MusteriUpdateDto
{
    public string? Ad { get; set; }
    public string? Soyad { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? Adres { get; set; }
    public bool? Tedarikci { get; set; }
}
#endregion

#region Kasa DTOs
public class KasaDto
{
    public Guid Id { get; set; }
    public Guid BayiId { get; set; }
    public string? BayiAd { get; set; }
    public string Ad { get; set; } = null!;
    public decimal Bakiye { get; set; }
    public bool Aktif { get; set; }
}

public class KasaCreateDto
{
    public Guid BayiId { get; set; }
    public string? Ad { get; set; }
    public decimal AcilisBakiyesi { get; set; }
}

public class KasaHareketDto
{
    public Guid Id { get; set; }
    public Guid KasaId { get; set; }
    public string? KasaAd { get; set; }
    public KasaHareketTip Tip { get; set; }
    public string TipAd { get; set; } = null!;
    public decimal Tutar { get; set; }
    public decimal OncekiBakiye { get; set; }
    public decimal SonrakiBakiye { get; set; }
    public DateTime IslemTarihi { get; set; }
    public string? BelgeNo { get; set; }
    public string? Aciklama { get; set; }
}

public class KasaHareketEkleDto
{
    public Guid KasaId { get; set; }
    public KasaHareketTip Tip { get; set; }
    public decimal Tutar { get; set; }
    public string? BelgeNo { get; set; }
    public string? Aciklama { get; set; }
}
#endregion

#region Personel DTOs
public class PersonelDto
{
    public Guid Id { get; set; }
    public Guid? BayiId { get; set; }
    public string? BayiAd { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string AdSoyad { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefon { get; set; }
    public UserRole Rol { get; set; }
    public string RolAd { get; set; } = null!;
    public decimal? Maas { get; set; }
    public int YillikIzinHakki { get; set; }
    public DateTime? IseBaslamaTarihi { get; set; }
    public bool Aktif { get; set; }
}

public class PersonelCreateDto
{
    public Guid BayiId { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Sifre { get; set; } = null!;
    public string? Telefon { get; set; }
    public UserRole Rol { get; set; }
    public decimal? Maas { get; set; }
    public int? YillikIzinHakki { get; set; }
    public DateTime? IseBaslamaTarihi { get; set; }
}

public class PersonelUpdateDto
{
    public string? Ad { get; set; }
    public string? Soyad { get; set; }
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public UserRole? Rol { get; set; }
    public decimal? Maas { get; set; }
    public int? YillikIzinHakki { get; set; }
    public bool? Aktif { get; set; }
}

public class MaasHesaplamaDto
{
    public Guid PersonelId { get; set; }
    public string? PersonelAdSoyad { get; set; }
    public int Yil { get; set; }
    public int Ay { get; set; }
    public decimal BrutMaas { get; set; }
    public decimal NetMaas { get; set; }
    public decimal EkMesaiSaati { get; set; }
    public decimal EkMesaiUcreti { get; set; }
    public decimal AvansKesinti { get; set; }
    public decimal ToplamOdeme { get; set; }
}

public class PersonelMaasOdemeDto
{
    public Guid Id { get; set; }
    public Guid PersonelId { get; set; }
    public string? PersonelAd { get; set; }
    public int Yil { get; set; }
    public int Ay { get; set; }
    public decimal BrutMaas { get; set; }
    public decimal NetMaas { get; set; }
    public decimal EkMesaiUcreti { get; set; }
    public decimal Kesinti { get; set; }
    public decimal AvansKesinti { get; set; }
    public decimal ToplamOdeme { get; set; }
    public bool OdendiMi { get; set; }
    public DateTime? OdemeTarihi { get; set; }
}

public class PersonelAvansDto
{
    public Guid Id { get; set; }
    public Guid PersonelId { get; set; }
    public string? PersonelAd { get; set; }
    public decimal Tutar { get; set; }
    public DateTime TalepTarihi { get; set; }
    public DateTime? OdemeTarihi { get; set; }
    public bool OnaylandiMi { get; set; }
    public bool OdendiMi { get; set; }
    public bool MaastanKesildiMi { get; set; }
}

public class PersonelIzinDto
{
    public Guid Id { get; set; }
    public Guid PersonelId { get; set; }
    public string? PersonelAd { get; set; }
    public PersonelIzinTip IzinTipi { get; set; }
    public string IzinTipiAd { get; set; } = null!;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public int GunSayisi { get; set; }
    public IzinDurum Durum { get; set; }
    public string DurumAd { get; set; } = null!;
    public bool UcretliMi { get; set; }
}

public class PersonelIzinCreateDto
{
    public Guid PersonelId { get; set; }
    public PersonelIzinTip IzinTipi { get; set; }
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public string? Aciklama { get; set; }
}

public class PersonelMesaiDto
{
    public Guid Id { get; set; }
    public Guid PersonelId { get; set; }
    public string? PersonelAd { get; set; }
    public DateTime Tarih { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public decimal ToplamSaat { get; set; }
    public decimal SaatUcreti { get; set; }
    public decimal ToplamUcret { get; set; }
    public bool OnaylandiMi { get; set; }
}

public class PersonelMesaiCreateDto
{
    public Guid PersonelId { get; set; }
    public DateTime Tarih { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public decimal SaatUcreti { get; set; }
    public string? Aciklama { get; set; }
}
#endregion

#region Stok DTOs
public class StokDurumDto
{
    public Guid UrunId { get; set; }
    public string? UrunAd { get; set; }
    public string? Barkod { get; set; }
    public int ToplamStok { get; set; }
    public int KritikStok { get; set; }
    public bool KritikStokAlti { get; set; }
    public List<DepoBazliStokDto> DepoBazliStoklar { get; set; } = new();
}

public class DepoBazliStokDto
{
    public Guid DepoId { get; set; }
    public string DepoAd { get; set; } = null!;
    public Guid? BayiId { get; set; }
    public string? BayiAd { get; set; }
    public int Miktar { get; set; }
}

public class UrunStokDto
{
    public Guid UrunId { get; set; }
    public string? UrunAd { get; set; }
    public string? Barkod { get; set; }
    public string? Marka { get; set; }
    public int MevcutStok { get; set; }
    public int KritikStok { get; set; }
}

public class StokHareketDto
{
    public Guid Id { get; set; }
    public Guid DepoId { get; set; }
    public string? DepoAd { get; set; }
    public Guid UrunId { get; set; }
    public string? UrunAd { get; set; }
    public string? SeriNo { get; set; }
    public StokHareketTip Tip { get; set; }
    public string TipAd { get; set; } = null!;
    public int Miktar { get; set; }
    public DateTime IslemTarihi { get; set; }
    public string? Aciklama { get; set; }
}

public class SeriNumarasiDetayDto
{
    public Guid Id { get; set; }
    public string SeriNo { get; set; } = null!;
    public Guid UrunId { get; set; }
    public string? UrunAd { get; set; }
    public string? Barkod { get; set; }
    public string? KategoriAd { get; set; }
    public DateTime? UretimTarihi { get; set; }
    public string? Mensei { get; set; }
    public string? MevcutDepoAd { get; set; }
    public string? MevcutBayiAd { get; set; }
    public bool Satildi { get; set; }
    public DateTime? SatisTarihi { get; set; }
    public DateTime GirisTarihi { get; set; }
    public decimal? AlisFiyati { get; set; }
    public decimal? SatisFiyati { get; set; }
}

public class StokGirisDto
{
    public Guid UrunId { get; set; }
    public Guid DepoId { get; set; }
    public List<string> SeriNumaralari { get; set; } = new();
    public decimal? AlisFiyati { get; set; }
    public string? Aciklama { get; set; }
}

public class StokCikisDto
{
    public Guid DepoId { get; set; }
    public List<string> SeriNumaralari { get; set; } = new();
    public string? Aciklama { get; set; }
}

public class SeriNumarasiUpdateDto
{
    public Guid? DepoId { get; set; }
    public decimal? AlisFiyati { get; set; }
    public decimal? SatisFiyati { get; set; }
    public string? Aciklama { get; set; }
}

// Yeni Stok Girişi - Fatura ile birlikte
public class StokGirisFaturaDto
{
    public Guid TedarikciId { get; set; }  // Tedarikci olan müşteri
    public Guid BayiId { get; set; }
    public Guid DepoId { get; set; }
    public string? FaturaNo { get; set; }  // Tedarikci fatura no
    public DateTime? FaturaTarihi { get; set; }
    public string? Aciklama { get; set; }
    public List<StokGirisKalemDto> Kalemler { get; set; } = new();
}

public class StokGirisKalemDto
{
    public Guid UrunId { get; set; }
    public decimal AlisFiyati { get; set; }
    public decimal SatisFiyati { get; set; }
    public int Adet { get; set; } = 1;
    public string? Barkod { get; set; }  // Null ise otomatik oluşturulur
    public List<string>? SeriNumaralari { get; set; }  // Seri takipli ürünler için
}

public class StokGirisSonucDto
{
    public Guid FaturaId { get; set; }
    public string FaturaNo { get; set; } = null!;
    public int ToplamAdet { get; set; }
    public decimal ToplamTutar { get; set; }
    public List<string> OlusturulanBarkodlar { get; set; } = new();
}
#endregion

#region Rapor DTOs
public class OzetRaporDto
{
    public decimal GunlukCiro { get; set; }
    public decimal AylikCiro { get; set; }
    public int BekleyenTeknikServis { get; set; }
    public int KritikStokUrun { get; set; }
    public decimal KasaBakiyesi { get; set; }
    public int BugunkuSatisSayisi { get; set; }
}

public class GunlukSatisRaporDto
{
    public DateTime Tarih { get; set; }
    public int SatisSayisi { get; set; }
    public decimal ToplamTutar { get; set; }
}

public class BayiPerformansDto
{
    public Guid BayiId { get; set; }
    public string? BayiAd { get; set; }
    public int SatisSayisi { get; set; }
    public decimal SatisTutari { get; set; }
    public int TeknikServisSayisi { get; set; }
    public decimal ToplamCiro { get; set; }
}

public class PersonelPerformansDto
{
    public Guid PersonelId { get; set; }
    public string? PersonelAdSoyad { get; set; }
    public int SatisSayisi { get; set; }
    public decimal SatisTutari { get; set; }
    public int TeknikServisSayisi { get; set; }
    public decimal ToplamCiro { get; set; }
}

public class UrunStokRaporDto
{
    public Guid UrunId { get; set; }
    public string? UrunAd { get; set; }
    public string? Barkod { get; set; }
    public string? KategoriAd { get; set; }
    public int ToplamStok { get; set; }
    public int KritikStok { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal ToplamDeger { get; set; }
}
#endregion

#region Satis DTOs
public class SatisDto
{
    public Guid Id { get; set; }
    public string SatisNo { get; set; } = null!;
    public Guid BayiId { get; set; }
    public string? BayiAd { get; set; }
    public Guid SatisYapanId { get; set; }
    public string? SatisYapanAd { get; set; }
    public DateTime SatisTarihi { get; set; }
    public Guid? MusteriId { get; set; }
    public string? MusteriAd { get; set; }
    public string? MusteriTelefon { get; set; }
    public decimal AraToplam { get; set; }
    public decimal KdvToplam { get; set; }
    public decimal IndirimTutar { get; set; }
    public decimal GenelToplam { get; set; }
    public OdemeTipi OdemeTipi { get; set; }
    public string OdemeTipiAd { get; set; } = null!;
    public decimal NakitTutar { get; set; }
    public decimal KartTutar { get; set; }
    public bool IptalEdildi { get; set; }
    public string? IptalNedeni { get; set; }
    public List<SatisKalemDto> Kalemler { get; set; } = new();
}

public class SatisKalemDto
{
    public Guid Id { get; set; }
    public Guid UrunId { get; set; }
    public string? UrunAd { get; set; }
    public string? SeriNo { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal KdvOran { get; set; }
    public decimal KdvTutar { get; set; }
    public decimal IndirimTutar { get; set; }
    public decimal Toplam { get; set; }
}

public class SatisCreateDto
{
    public Guid? MusteriId { get; set; }
    public string? MusteriAd { get; set; }
    public string? MusteriTelefon { get; set; }
    public OdemeTipi OdemeTipi { get; set; }
    public decimal NakitTutar { get; set; }
    public decimal KartTutar { get; set; }
    public decimal IndirimTutar { get; set; }
    public string? Aciklama { get; set; }
    public List<SatisKalemCreateDto> Kalemler { get; set; } = new();
}

public class SatisKalemCreateDto
{
    public string SeriNo { get; set; } = null!;
    public decimal? IndirimTutar { get; set; }
}

public class SatisValidasyonDto
{
    public bool Gecerli { get; set; }
    public string? Mesaj { get; set; }
    public SeriNumarasiDto? SeriNumarasi { get; set; }
    public decimal SatisFiyat { get; set; }
    public decimal KdvOran { get; set; }
}
#endregion

#region TeknikServis DTOs
public class TeknikServisDto
{
    public Guid Id { get; set; }
    public string ServisNo { get; set; } = null!;
    public Guid BayiId { get; set; }
    public string? BayiAd { get; set; }
    public Guid? MusteriId { get; set; }
    public string? MusteriAd { get; set; }
    public string? MusteriTelefon { get; set; }
    public string? SorumluPersonelAd { get; set; }
    public string CihazTip { get; set; } = null!;
    public string? CihazMarka { get; set; }
    public string? CihazModel { get; set; }
    public string? SeriNo { get; set; }
    public string Ariza { get; set; } = null!;
    public string? ArizaDetay { get; set; }
    public string? YapilanIslem { get; set; }
    public TeknikServisDurum Durum { get; set; }
    public string DurumAd { get; set; } = null!;
    public DateTime GirisTarihi { get; set; }
    public DateTime? TahminiTeslimTarihi { get; set; }
    public DateTime? TeslimTarihi { get; set; }
    public decimal IscilikUcreti { get; set; }
    public decimal ParcaToplam { get; set; }
    public decimal ToplamTutar { get; set; }
    public decimal OdenenTutar { get; set; }
    public bool OdemeAlindi { get; set; }
    public bool GarantiKapsami { get; set; }
    public List<TeknikServisKalemDto> Kalemler { get; set; } = new();
}

public class TeknikServisKalemDto
{
    public Guid Id { get; set; }
    public Guid? UrunId { get; set; }
    public string? UrunAd { get; set; }
    public string? ParcaAdi { get; set; }
    public int Miktar { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal Toplam { get; set; }
}

public class TeknikServisCreateDto
{
    public Guid? MusteriId { get; set; }
    public string? MusteriAd { get; set; }
    public string? MusteriTelefon { get; set; }
    public Guid? SorumluPersonelId { get; set; }
    public string CihazTip { get; set; } = null!;
    public string? CihazMarka { get; set; }
    public string? CihazModel { get; set; }
    public string? SeriNo { get; set; }
    public string? ImeiNo { get; set; }
    public string? CihazSifre { get; set; }
    public string Ariza { get; set; } = null!;
    public string? ArizaDetay { get; set; }
    public DateTime? TahminiTeslimTarihi { get; set; }
    public decimal IscilikUcreti { get; set; }
    public bool GarantiKapsami { get; set; }
    public string? GarantiNot { get; set; }
}

public class TeknikServisDurumGuncelleDto
{
    public TeknikServisDurum Durum { get; set; }
    public string? TespitNotlari { get; set; }
    public string? YapilanIslem { get; set; }
}

public class TeknikServisParcaEkleDto
{
    public Guid? UrunId { get; set; }
    public string? ParcaAdi { get; set; }
    public int Miktar { get; set; } = 1;
    public decimal BirimFiyat { get; set; }
}

public class TeknikServisOdemeAlDto
{
    public decimal Tutar { get; set; }
    public OdemeTipi OdemeTipi { get; set; }
}
#endregion

#region Transfer DTOs
public class TransferDto
{
    public Guid Id { get; set; }
    public string TransferNo { get; set; } = null!;
    public Guid KaynakDepoId { get; set; }
    public string? KaynakDepoAd { get; set; }
    public Guid HedefDepoId { get; set; }
    public string? HedefDepoAd { get; set; }
    public string? OlusturanAd { get; set; }
    public TransferDurum Durum { get; set; }
    public string DurumAd { get; set; } = null!;
    public DateTime TransferTarihi { get; set; }
    public DateTime? TamamlanmaTarihi { get; set; }
    public string? Aciklama { get; set; }
    public List<TransferKalemDto> Kalemler { get; set; } = new();
}

public class TransferKalemDto
{
    public Guid Id { get; set; }
    public Guid UrunId { get; set; }
    public string? UrunAd { get; set; }
    public string? SeriNo { get; set; }
    public bool TeslimAlindi { get; set; }
    public DateTime? TeslimTarihi { get; set; }
}

public class TransferCreateDto
{
    public Guid KaynakDepoId { get; set; }
    public Guid HedefDepoId { get; set; }
    public string? Aciklama { get; set; }
    public List<TransferKalemCreateDto> Kalemler { get; set; } = new();
}

public class TransferKalemCreateDto
{
    public string SeriNo { get; set; } = null!;
}
#endregion

#region Iade DTOs
public class IadeDto
{
    public Guid Id { get; set; }
    public string IadeNo { get; set; } = null!;
    public Guid SatisId { get; set; }
    public string? SatisNo { get; set; }
    public string? MusteriAd { get; set; }
    public IadeDurum Durum { get; set; }
    public string DurumAd { get; set; } = null!;
    public DateTime IadeTarihi { get; set; }
    public decimal ToplamTutar { get; set; }
    public decimal IadeEdilen { get; set; }
    public string? Neden { get; set; }
    public List<IadeKalemDto> Kalemler { get; set; } = new();
}

public class IadeKalemDto
{
    public Guid Id { get; set; }
    public string? UrunAd { get; set; }
    public string? SeriNo { get; set; }
    public decimal IadeTutar { get; set; }
    public string? Neden { get; set; }
}

public class IadeCreateDto
{
    public Guid SatisId { get; set; }
    public string? Neden { get; set; }
    public List<IadeKalemCreateDto> Kalemler { get; set; } = new();
}

public class IadeKalemCreateDto
{
    public Guid SatisKalemId { get; set; }
    public string? Neden { get; set; }
}
#endregion

#region Fatura DTOs
public class FaturaDto
{
    public Guid Id { get; set; }
    public string FaturaNo { get; set; } = null!;
    public FaturaTip Tip { get; set; }
    public string TipAd { get; set; } = null!;
    public string? MusteriAd { get; set; }
    public string? MusteriVergiNo { get; set; }
    public DateTime FaturaTarihi { get; set; }
    public decimal AraToplam { get; set; }
    public decimal KdvToplam { get; set; }
    public decimal GenelToplam { get; set; }
    public bool Onaylandi { get; set; }
    public bool IptalEdildi { get; set; }
    public List<FaturaKalemDto> Kalemler { get; set; } = new();
}

public class FaturaKalemDto
{
    public Guid Id { get; set; }
    public string? Aciklama { get; set; }
    public int Miktar { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal KdvOran { get; set; }
    public decimal KdvTutar { get; set; }
    public decimal IndirimTutar { get; set; }
    public decimal Toplam { get; set; }
}

public class FaturaCreateDto
{
    public FaturaTip Tip { get; set; }
    public Guid? MusteriId { get; set; }
    public string? MusteriAd { get; set; }
    public string? MusteriAdres { get; set; }
    public string? MusteriVergiNo { get; set; }
    public Guid? SatisId { get; set; }
    public Guid? TeknikServisId { get; set; }
    public List<FaturaKalemCreateDto> Kalemler { get; set; } = new();
}

public class FaturaKalemCreateDto
{
    public Guid? UrunId { get; set; }
    public string? Aciklama { get; set; }
    public int Miktar { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal KdvOran { get; set; }
}
#endregion

#region Sayim DTOs
public class SayimDto
{
    public Guid Id { get; set; }
    public string SayimNo { get; set; } = null!;
    public Guid DepoId { get; set; }
    public string? DepoAd { get; set; }
    public string? BaslatanAd { get; set; }
    public SayimDurum Durum { get; set; }
    public string DurumAd { get; set; } = null!;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public int SistemStok { get; set; }
    public int FizikselStok { get; set; }
    public int Fark { get; set; }
    public List<SayimKalemDto> Kalemler { get; set; } = new();
}

public class SayimKalemDto
{
    public Guid Id { get; set; }
    public string? UrunAd { get; set; }
    public string SeriNo { get; set; } = null!;
    public bool SistemdeVar { get; set; }
    public bool FizikiSayimda { get; set; }
    public DateTime TaramaTarihi { get; set; }
}

public class SayimBaslatDto
{
    public Guid DepoId { get; set; }
    public string? Aciklama { get; set; }
}

public class SayimSeriTaramaResultDto
{
    public bool Basarili { get; set; }
    public string? Mesaj { get; set; }
    public string SeriNo { get; set; } = null!;
    public bool SistemdeVar { get; set; }
    public bool FizikiSayimda { get; set; }
    public string? UrunAd { get; set; }
}

public class SayimRaporDto
{
    public Guid SayimId { get; set; }
    public string? SayimNo { get; set; }
    public string? DepoAd { get; set; }
    public int SistemStok { get; set; }
    public int FizikselStok { get; set; }
    public int Fark { get; set; }
    public int EksikUrun { get; set; }
    public int FazlaUrun { get; set; }
    public List<SayimFarkDto> Farklar { get; set; } = new();
}

public class SayimFarkDto
{
    public string? UrunAd { get; set; }
    public string? SeriNo { get; set; }
    public string? Durum { get; set; }
}
#endregion
