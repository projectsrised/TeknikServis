using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Domain.Entities;

#region Base Entities
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? GuncellemeTarihi { get; set; }
    public bool Silindi { get; set; } = false;
}

public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}

public abstract class BayiEntity : TenantEntity
{
    public Guid BayiId { get; set; }
    public Bayi Bayi { get; set; } = null!;
}
#endregion

#region Tenant & Bayi
public class Tenant : BaseEntity
{
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? VergiNo { get; set; }
    public string? Adres { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime? LisansBitisTarihi { get; set; }
    public ICollection<Bayi> Bayiler { get; set; } = new List<Bayi>();
    public ICollection<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
}

public class Bayi : TenantEntity
{
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Adres { get; set; }
    public string? Il { get; set; }
    public string? Ilce { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? YetkiliAd { get; set; }
    public bool Aktif { get; set; } = true;
    public bool MerkezMi { get; set; } = false;
    public ICollection<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
    public ICollection<Depo> Depolar { get; set; } = new List<Depo>();
    public ICollection<Kasa> Kasalar { get; set; } = new List<Kasa>();
}
#endregion

#region Kullanici
public class Kullanici : TenantEntity
{
    public Guid? BayiId { get; set; }
    public Bayi? Bayi { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string SifreHash { get; set; } = null!;
    public string? Telefon { get; set; }
    public UserRole Rol { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime? SonGirisTarihi { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenSonKullanma { get; set; }
    public decimal? Maas { get; set; }
    public int YillikIzinHakki { get; set; } = 14;
    public DateTime? IseBaslamaTarihi { get; set; }
    public string AdSoyad => $"{Ad} {Soyad}";
}
#endregion

#region Depo & Kasa
public class Depo : BayiEntity
{
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Adres { get; set; }
    public bool MerkezDepoMu { get; set; } = false;
    public bool Aktif { get; set; } = true;
    public ICollection<SeriNumarasi> SeriNumaralari { get; set; } = new List<SeriNumarasi>();
}

public class Kasa : BayiEntity
{
    public string Ad { get; set; } = null!;
    public decimal Bakiye { get; set; } = 0;
    public bool Aktif { get; set; } = true;
    public ICollection<KasaHareket> Hareketler { get; set; } = new List<KasaHareket>();
}

public class KasaHareket : BayiEntity
{
    public Guid KasaId { get; set; }
    public Kasa Kasa { get; set; } = null!;
    public KasaHareketTip Tip { get; set; }
    public decimal Tutar { get; set; }
    public decimal OncekiBakiye { get; set; }
    public decimal SonrakiBakiye { get; set; }
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public string? BelgeNo { get; set; }
    public string? Aciklama { get; set; }
    public Guid? OlusturanId { get; set; }
    public Kullanici? Olusturan { get; set; }
}
#endregion

#region Marka & Model
public class Marka : TenantEntity
{
    public string Ad { get; set; } = null!;
    public string? Logo { get; set; }
    public int Sira { get; set; }
    public bool Aktif { get; set; } = true;
    public ICollection<Model> Modeller { get; set; } = new List<Model>();
}

public class Model : TenantEntity
{
    public Guid MarkaId { get; set; }
    public Marka Marka { get; set; } = null!;
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public int Sira { get; set; }
    public bool Aktif { get; set; } = true;
}
#endregion

#region Kategori & Urun
// Kategori artık TenantEntity değil, BaseEntity - tüm tenantlar aynı kategorileri kullanacak
public class Kategori : BaseEntity
{
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Aciklama { get; set; }
    public KategoriTip Tip { get; set; }
    public Guid? UstKategoriId { get; set; }
    public Kategori? UstKategori { get; set; }
    public int Sira { get; set; }
    public bool Aktif { get; set; } = true;
    public ICollection<Kategori> AltKategoriler { get; set; } = new List<Kategori>();
    public ICollection<Urun> Urunler { get; set; } = new List<Urun>();
}

public class Urun : TenantEntity
{
    public Guid KategoriId { get; set; }
    public Kategori Kategori { get; set; } = null!;
    public Guid? MarkaId { get; set; }
    public Marka? UrunMarka { get; set; }
    public Guid? ModelId { get; set; }
    public Model? UrunModel { get; set; }
    public string Ad { get; set; } = null!;
    public string? Kod { get; set; }
    public string? Barkod { get; set; }
    public string? Marka { get; set; }  // Backward compatibility için tutuyoruz
    public string? Model { get; set; }  // Backward compatibility için tutuyoruz
    public string? Aciklama { get; set; }
    public decimal AlisFiyat { get; set; }
    public decimal SatisFiyat { get; set; }
    public decimal KdvOran { get; set; } = 20;
    public int KritikStok { get; set; } = 5;
    public bool SeriTakipli { get; set; } = true;
    public bool Aktif { get; set; } = true;
    public ICollection<SeriNumarasi> SeriNumaralari { get; set; } = new List<SeriNumarasi>();
}

public class SeriNumarasi : TenantEntity
{
    public Guid? BayiId { get; set; }
    public Bayi? Bayi { get; set; }
    public Guid UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
    public Guid? DepoId { get; set; }
    public Depo? Depo { get; set; }
    public string SeriNo { get; set; } = null!;
    public decimal? AlisFiyati { get; set; }
    public decimal? SatisFiyati { get; set; }
    public string? Aciklama { get; set; }
    public bool Satildi { get; set; } = false;
    public DateTime? SatisTarihi { get; set; }
    public Guid? SatisId { get; set; }
    public DateTime GirisTarihi { get; set; } = DateTime.UtcNow;
}

public class StokHareket : BayiEntity
{
    public Guid DepoId { get; set; }
    public Depo Depo { get; set; } = null!;
    public Guid UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
    public Guid? SeriNumarasiId { get; set; }
    public SeriNumarasi? SeriNumarasi { get; set; }
    public StokHareketTip Tip { get; set; }
    public int Miktar { get; set; }
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
    public string? Aciklama { get; set; }
}
#endregion

#region Musteri
public class Musteri : TenantEntity
{
    public Guid? BayiId { get; set; }
    public Bayi? Bayi { get; set; }
    public string Ad { get; set; } = null!;
    public string Soyad { get; set; } = null!;
    public string Telefon { get; set; } = null!;
    public string Email { get; set; } = null!;  // Email artık zorunlu
    public string? Adres { get; set; }
    public string? TcNo { get; set; }
    public string? VergiNo { get; set; }
    public bool Kurumsal { get; set; } = false;
    public bool Tedarikci { get; set; } = false;  // Tedarikci olarak kullanılabilir
    public string AdSoyad => $"{Ad} {Soyad}";
}
#endregion

#region Satis
public class Satis : BayiEntity
{
    public string SatisNo { get; set; } = null!;
    public Guid? MusteriId { get; set; }
    public Musteri? Musteri { get; set; }
    public string? MusteriAd { get; set; }
    public string? MusteriTelefon { get; set; }
    public Guid SatisYapanId { get; set; }
    public Kullanici SatisYapan { get; set; } = null!;
    public DateTime SatisTarihi { get; set; } = DateTime.UtcNow;
    public OdemeTipi OdemeTipi { get; set; }
    public decimal AraToplam { get; set; }
    public decimal KdvToplam { get; set; }
    public decimal IndirimTutar { get; set; }
    public decimal GenelToplam { get; set; }
    public decimal NakitTutar { get; set; }
    public decimal KartTutar { get; set; }
    public string? Aciklama { get; set; }
    public bool IptalEdildi { get; set; } = false;
    public string? IptalNedeni { get; set; }
    public DateTime? IptalTarihi { get; set; }
    public ICollection<SatisKalem> Kalemler { get; set; } = new List<SatisKalem>();
}

public class SatisKalem : BaseEntity
{
    public Guid SatisId { get; set; }
    public Satis Satis { get; set; } = null!;
    public Guid UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
    public Guid? SeriNumarasiId { get; set; }
    public SeriNumarasi? SeriNumarasi { get; set; }
    public int Miktar { get; set; } = 1;
    public decimal BirimFiyat { get; set; }
    public decimal KdvOran { get; set; }
    public decimal KdvTutar { get; set; }
    public decimal IndirimTutar { get; set; }
    public decimal Toplam { get; set; }
}
#endregion

#region TeknikServis
public class TeknikServisKayit : BayiEntity
{
    public string ServisNo { get; set; } = null!;
    public Guid? MusteriId { get; set; }
    public Musteri? Musteri { get; set; }
    public string? MusteriAd { get; set; }
    public string? MusteriTelefon { get; set; }
    public Guid? SorumluPersonelId { get; set; }
    public Kullanici? SorumluPersonel { get; set; }
    public string CihazTip { get; set; } = null!;
    public string? CihazMarka { get; set; }
    public string? CihazModel { get; set; }
    public string? SeriNo { get; set; }
    public string? ImeiNo { get; set; }
    public string? CihazSifre { get; set; }
    public string Ariza { get; set; } = null!;
    public string? ArizaDetay { get; set; }
    public string? TespitNotlari { get; set; }
    public string? YapilanIslem { get; set; }
    public TeknikServisDurum Durum { get; set; } = TeknikServisDurum.Beklemede;
    public DateTime GirisTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? TahminiTeslimTarihi { get; set; }
    public DateTime? TeslimTarihi { get; set; }
    public decimal IscilikUcreti { get; set; }
    public decimal ParcaToplam { get; set; }
    public decimal ToplamTutar { get; set; }
    public decimal OdenenTutar { get; set; }
    public bool OdemeAlindi { get; set; } = false;
    public DateTime? OdemeTarihi { get; set; }
    public bool GarantiKapsami { get; set; } = false;
    public string? GarantiNot { get; set; }
    public string? IptalNedeni { get; set; }
    public ICollection<TeknikServisKalem> Kalemler { get; set; } = new List<TeknikServisKalem>();
}

public class TeknikServisKalem : BaseEntity
{
    public Guid TeknikServisId { get; set; }
    public TeknikServisKayit TeknikServis { get; set; } = null!;
    public Guid? UrunId { get; set; }
    public Urun? Urun { get; set; }
    public string? ParcaAdi { get; set; }
    public int Miktar { get; set; } = 1;
    public decimal BirimFiyat { get; set; }
    public decimal Toplam { get; set; }
}
#endregion

#region Transfer
public class Transfer : TenantEntity
{
    public string TransferNo { get; set; } = null!;
    public Guid KaynakDepoId { get; set; }
    public Depo KaynakDepo { get; set; } = null!;
    public Guid HedefDepoId { get; set; }
    public Depo HedefDepo { get; set; } = null!;
    public Guid OlusturanId { get; set; }
    public Kullanici Olusturan { get; set; } = null!;
    public TransferDurum Durum { get; set; } = TransferDurum.Beklemede;
    public DateTime TransferTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? OnayTarihi { get; set; }
    public DateTime? TamamlanmaTarihi { get; set; }
    public string? Aciklama { get; set; }
    public string? IptalNedeni { get; set; }
    public ICollection<TransferKalem> Kalemler { get; set; } = new List<TransferKalem>();
}

public class TransferKalem : BaseEntity
{
    public Guid TransferId { get; set; }
    public Transfer Transfer { get; set; } = null!;
    public Guid UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
    public Guid? SeriNumarasiId { get; set; }
    public SeriNumarasi? SeriNumarasi { get; set; }
    public bool TeslimAlindi { get; set; } = false;
    public DateTime? TeslimTarihi { get; set; }
}
#endregion

#region Iade
public class Iade : BayiEntity
{
    public string IadeNo { get; set; } = null!;
    public Guid SatisId { get; set; }
    public Satis Satis { get; set; } = null!;
    public Guid? MusteriId { get; set; }
    public Musteri? Musteri { get; set; }
    public Guid IslemYapanId { get; set; }
    public Kullanici IslemYapan { get; set; } = null!;
    public IadeDurum Durum { get; set; } = IadeDurum.Beklemede;
    public DateTime IadeTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? OnayTarihi { get; set; }
    public DateTime? TamamlanmaTarihi { get; set; }
    public decimal ToplamTutar { get; set; }
    public decimal IadeEdilen { get; set; }
    public string? Neden { get; set; }
    public string? RedNedeni { get; set; }
    public ICollection<IadeKalem> Kalemler { get; set; } = new List<IadeKalem>();
}

public class IadeKalem : BaseEntity
{
    public Guid IadeId { get; set; }
    public Iade Iade { get; set; } = null!;
    public Guid SatisKalemId { get; set; }
    public SatisKalem SatisKalem { get; set; } = null!;
    public Guid? SeriNumarasiId { get; set; }
    public SeriNumarasi? SeriNumarasi { get; set; }
    public decimal IadeTutar { get; set; }
    public string? Neden { get; set; }
}
#endregion

#region Fatura
public class Fatura : BayiEntity
{
    public string FaturaNo { get; set; } = null!;
    public FaturaTip Tip { get; set; }
    public Guid? MusteriId { get; set; }
    public Musteri? Musteri { get; set; }
    public string? MusteriAd { get; set; }
    public string? MusteriAdres { get; set; }
    public string? MusteriVergiNo { get; set; }
    public Guid? SatisId { get; set; }
    public Satis? Satis { get; set; }
    public Guid? TeknikServisId { get; set; }
    public TeknikServisKayit? TeknikServis { get; set; }
    public DateTime FaturaTarihi { get; set; } = DateTime.UtcNow;
    public decimal AraToplam { get; set; }
    public decimal KdvToplam { get; set; }
    public decimal IndirimTutar { get; set; }
    public decimal GenelToplam { get; set; }
    public bool Onaylandi { get; set; } = false;
    public DateTime? OnayTarihi { get; set; }
    public bool IptalEdildi { get; set; } = false;
    public string? IptalNedeni { get; set; }
    public ICollection<FaturaKalem> Kalemler { get; set; } = new List<FaturaKalem>();
}

public class FaturaKalem : BaseEntity
{
    public Guid FaturaId { get; set; }
    public Fatura Fatura { get; set; } = null!;
    public Guid? UrunId { get; set; }
    public Urun? Urun { get; set; }
    public string? Aciklama { get; set; }
    public int Miktar { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal KdvOran { get; set; }
    public decimal KdvTutar { get; set; }
    public decimal IndirimTutar { get; set; }
    public decimal Toplam { get; set; }
}
#endregion

#region Sayim
public class Sayim : BayiEntity
{
    public string SayimNo { get; set; } = null!;
    public Guid DepoId { get; set; }
    public Depo Depo { get; set; } = null!;
    public Guid BaslatanId { get; set; }
    public Kullanici Baslatan { get; set; } = null!;
    public SayimDurum Durum { get; set; } = SayimDurum.Basladi;
    public DateTime BaslangicTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? BitisTarihi { get; set; }
    public int SistemStok { get; set; }
    public int FizikselStok { get; set; }
    public int Fark { get; set; }
    public string? Aciklama { get; set; }
    public ICollection<SayimKalem> Kalemler { get; set; } = new List<SayimKalem>();
}

public class SayimKalem : BaseEntity
{
    public Guid SayimId { get; set; }
    public Sayim Sayim { get; set; } = null!;
    public Guid UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
    public Guid? SeriNumarasiId { get; set; }
    public SeriNumarasi? SeriNumarasi { get; set; }
    public string SeriNo { get; set; } = null!;
    public bool SistemdeVar { get; set; }
    public bool FizikiSayimda { get; set; }
    public DateTime TaramaTarihi { get; set; } = DateTime.UtcNow;
}
#endregion

#region Personel
public class PersonelIzin : TenantEntity
{
    public Guid PersonelId { get; set; }
    public Kullanici Personel { get; set; } = null!;
    public PersonelIzinTip IzinTipi { get; set; }
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public int GunSayisi { get; set; }
    public IzinDurum Durum { get; set; } = IzinDurum.Beklemede;
    public DateTime? OnayTarihi { get; set; }
    public Guid? OnaylayanId { get; set; }
    public Kullanici? Onaylayan { get; set; }
    public string? Aciklama { get; set; }
    public string? RedNedeni { get; set; }
    public bool UcretliMi { get; set; } = true;
}

public class PersonelAvans : TenantEntity
{
    public Guid PersonelId { get; set; }
    public Kullanici Personel { get; set; } = null!;
    public decimal Tutar { get; set; }
    public DateTime TalepTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? OnayTarihi { get; set; }
    public DateTime? OdemeTarihi { get; set; }
    public bool OnaylandiMi { get; set; } = false;
    public bool OdendiMi { get; set; } = false;
    public bool MaastanKesildiMi { get; set; } = false;
    public Guid? OnaylayanId { get; set; }
    public Kullanici? Onaylayan { get; set; }
}

public class PersonelMesai : TenantEntity
{
    public Guid PersonelId { get; set; }
    public Kullanici Personel { get; set; } = null!;
    public DateTime Tarih { get; set; }
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public decimal ToplamSaat { get; set; }
    public decimal SaatUcreti { get; set; }
    public decimal ToplamUcret { get; set; }
    public string? Aciklama { get; set; }
    public bool OnaylandiMi { get; set; } = false;
    public DateTime? OnayTarihi { get; set; }
    public Guid? OnaylayanId { get; set; }
    public Kullanici? Onaylayan { get; set; }
}

public class PersonelMaasOdeme : TenantEntity
{
    public Guid PersonelId { get; set; }
    public Kullanici Personel { get; set; } = null!;
    public int Yil { get; set; }
    public int Ay { get; set; }
    public decimal BrutMaas { get; set; }
    public decimal NetMaas { get; set; }
    public decimal EkMesaiUcreti { get; set; }
    public decimal Kesinti { get; set; }
    public decimal AvansKesinti { get; set; }
    public decimal ToplamOdeme { get; set; }
    public bool OdendiMi { get; set; } = false;
    public DateTime? OdemeTarihi { get; set; }
    public Guid? OdeyenId { get; set; }
    public Kullanici? Odeyen { get; set; }
}
#endregion
