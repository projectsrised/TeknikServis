// Enums
export enum UserRole {
  SuperAdmin = 1,
  TenantAdmin = 2,
  BayiAdmin = 3,
  SatisSorumlusu = 4,
  TeknikServisPersoneli = 5,
}

export enum KategoriTip {
  Telefon = 1,
  Tablet = 2,
  Aksesuar = 3,
  YedekParca = 4,
  Diger = 5,
}

export enum OdemeTipi {
  Nakit = 1,
  KrediKarti = 2,
  Havale = 3,
  Taksit = 4,
  Karisik = 5,
}

export enum TeknikServisDurum {
  Beklemede = 1,
  Inceleniyor = 2,
  ParcaBekleniyor = 3,
  Onariliyor = 4,
  Tamamlandi = 5,
  TeslimEdildi = 6,
  IptalEdildi = 7,
}

export enum TransferDurum {
  Beklemede = 1,
  Onaylandi = 2,
  YoldaGidiyor = 3,
  TeslimAlindi = 4,
  Tamamlandi = 5,
  IptalEdildi = 6,
}

export enum IadeDurum {
  Beklemede = 1,
  Inceleniyor = 2,
  Onaylandi = 3,
  Reddedildi = 4,
  Tamamlandi = 5,
}

export enum KasaHareketTip {
  Giris = 1,
  Cikis = 2,
  NakitSatis = 3,
  KartSatis = 4,
  TeknikServisOdeme = 5,
  MaasOdeme = 6,
  AvansOdeme = 7,
  Gider = 8,
}

// API Response
export interface ApiResponse<T> {
  basarili: boolean;
  mesaj?: string;
  data?: T;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Auth
export interface LoginRequest {
  email: string;
  sifre: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  kullanici: Kullanici;
}

export interface Kullanici {
  id: string;
  tenantId: string;
  tenantAd?: string;
  bayiId?: string;
  bayiAd?: string;
  ad: string;
  soyad: string;
  adSoyad: string;
  email: string;
  telefon?: string;
  rol: UserRole;
  rolAd: string;
  aktif: boolean;
}

// Bayi
export interface Bayi {
  id: string;
  ad: string;
  kod?: string;
  adres?: string;
  il?: string;
  ilce?: string;
  telefon?: string;
  email?: string;
  yetkiliAd?: string;
  aktif: boolean;
  merkezMi: boolean;
}

export interface BayiCreate {
  ad: string;
  kod?: string;
  adres?: string;
  il?: string;
  ilce?: string;
  telefon?: string;
  email?: string;
  yetkiliAd?: string;
  merkezMi: boolean;
}

// Depo
export interface Depo {
  id: string;
  bayiId: string;
  bayiAd?: string;
  ad: string;
  kod?: string;
  adres?: string;
  merkezDepoMu: boolean;
  aktif: boolean;
}

// Musteri
export interface Musteri {
  id: string;
  ad: string;
  soyad: string;
  adSoyad: string;
  telefon: string;
  email: string;
  adres?: string;
  tcNo?: string;
  vergiNo?: string;
  kurumsal: boolean;
  tedarikci: boolean;
}

export interface MusteriCreate {
  ad: string;
  soyad: string;
  telefon: string;
  email: string;
  adres?: string;
  tcNo?: string;
  vergiNo?: string;
  kurumsal: boolean;
  tedarikci?: boolean;
}

// Marka
export interface Marka {
  id: string;
  ad: string;
  logo?: string;
  sira: number;
  aktif: boolean;
  modelSayisi: number;
}

export interface MarkaCreate {
  ad: string;
  logo?: string;
  sira: number;
}

export interface MarkaWithModels {
  id: string;
  ad: string;
  modeller: Model[];
}

// Model
export interface Model {
  id: string;
  markaId: string;
  markaAd?: string;
  ad: string;
  kod?: string;
  sira: number;
  aktif: boolean;
}

export interface ModelCreate {
  markaId: string;
  ad: string;
  kod?: string;
  sira: number;
}

// Kategori
export interface Kategori {
  id: string;
  ad: string;
  kod?: string;
  aciklama?: string;
  tip: KategoriTip;
  tipAd: string;
  ustKategoriId?: string;
  ustKategoriAd?: string;
  sira: number;
  aktif: boolean;
}

export interface KategoriCreate {
  ad: string;
  kod?: string;
  aciklama?: string;
  tip: KategoriTip;
  ustKategoriId?: string;
  sira: number;
}

// Urun
export interface Urun {
  id: string;
  kategoriId: string;
  kategoriAd?: string;
  markaId?: string;
  markaAd?: string;
  modelId?: string;
  modelAd?: string;
  ad: string;
  kod?: string;
  barkod?: string;
  marka?: string;
  model?: string;
  aciklama?: string;
  alisFiyat: number;
  satisFiyat: number;
  kdvOran: number;
  kritikStok: number;
  seriTakipli: boolean;
  aktif: boolean;
  mevcutStok: number;
}

export interface UrunCreate {
  kategoriId: string;
  markaId?: string;
  modelId?: string;
  ad: string;
  kod?: string;
  barkod?: string;
  marka?: string;
  model?: string;
  aciklama?: string;
  alisFiyat: number;
  satisFiyat: number;
  kdvOran: number;
  kritikStok: number;
  seriTakipli: boolean;
}

// Seri Numarasi
export interface SeriNumarasi {
  id: string;
  seriNo: string;
  urunId: string;
  urunAd?: string;
  depoId?: string;
  depoAd?: string;
  satildi: boolean;
  satisTarihi?: string;
  alisFiyati?: number;
  satisFiyati?: number;
}

// Satis
export interface Satis {
  id: string;
  satisNo: string;
  bayiId: string;
  bayiAd?: string;
  satisYapanId: string;
  satisYapanAd?: string;
  satisTarihi: string;
  musteriId?: string;
  musteriAd?: string;
  musteriTelefon?: string;
  araToplam: number;
  kdvToplam: number;
  indirimTutar: number;
  genelToplam: number;
  odemeTipi: OdemeTipi;
  odemeTipiAd: string;
  nakitTutar: number;
  kartTutar: number;
  iptalEdildi: boolean;
  iptalNedeni?: string;
  kalemler: SatisKalem[];
}

export interface SatisKalem {
  id: string;
  urunId: string;
  urunAd?: string;
  seriNo?: string;
  birimFiyat: number;
  kdvOran: number;
  kdvTutar: number;
  indirimTutar: number;
  toplam: number;
}

export interface SatisCreate {
  musteriId?: string;
  musteriAd?: string;
  musteriTelefon?: string;
  odemeTipi: OdemeTipi;
  nakitTutar: number;
  kartTutar: number;
  indirimTutar: number;
  aciklama?: string;
  kalemler: SatisKalemCreate[];
}

export interface SatisKalemCreate {
  seriNo: string;
  indirimTutar?: number;
}

// Teknik Servis
export interface TeknikServis {
  id: string;
  servisNo: string;
  bayiId: string;
  bayiAd?: string;
  musteriId?: string;
  musteriAd?: string;
  musteriTelefon?: string;
  sorumluPersonelAd?: string;
  cihazTip: string;
  cihazMarka?: string;
  cihazModel?: string;
  seriNo?: string;
  ariza: string;
  arizaDetay?: string;
  yapilanIslem?: string;
  durum: TeknikServisDurum;
  durumAd: string;
  girisTarihi: string;
  tahminiTeslimTarihi?: string;
  teslimTarihi?: string;
  iscilikUcreti: number;
  parcaToplam: number;
  toplamTutar: number;
  odenenTutar: number;
  odemeAlindi: boolean;
  garantiKapsami: boolean;
  kalemler: TeknikServisKalem[];
}

export interface TeknikServisKalem {
  id: string;
  urunId?: string;
  urunAd?: string;
  parcaAdi?: string;
  miktar: number;
  birimFiyat: number;
  toplam: number;
}

export interface TeknikServisCreate {
  musteriId?: string;
  musteriAd?: string;
  musteriTelefon?: string;
  sorumluPersonelId?: string;
  cihazTip: string;
  cihazMarka?: string;
  cihazModel?: string;
  seriNo?: string;
  imeiNo?: string;
  cihazSifre?: string;
  ariza: string;
  arizaDetay?: string;
  tahminiTeslimTarihi?: string;
  iscilikUcreti: number;
  garantiKapsami: boolean;
  garantiNot?: string;
}

// Transfer
export interface Transfer {
  id: string;
  transferNo: string;
  kaynakDepoId: string;
  kaynakDepoAd?: string;
  hedefDepoId: string;
  hedefDepoAd?: string;
  olusturanAd?: string;
  durum: TransferDurum;
  durumAd: string;
  transferTarihi: string;
  tamamlanmaTarihi?: string;
  aciklama?: string;
  kalemler: TransferKalem[];
}

export interface TransferKalem {
  id: string;
  urunId: string;
  urunAd?: string;
  seriNo?: string;
  teslimAlindi: boolean;
  teslimTarihi?: string;
}

// Kasa
export interface Kasa {
  id: string;
  bayiId: string;
  bayiAd?: string;
  ad: string;
  bakiye: number;
  aktif: boolean;
}

export interface KasaHareket {
  id: string;
  kasaId: string;
  kasaAd?: string;
  tip: KasaHareketTip;
  tipAd: string;
  tutar: number;
  oncekiBakiye: number;
  sonrakiBakiye: number;
  islemTarihi: string;
  belgeNo?: string;
  aciklama?: string;
}

// Personel
export interface Personel {
  id: string;
  bayiId?: string;
  bayiAd?: string;
  ad: string;
  soyad: string;
  adSoyad: string;
  email: string;
  telefon?: string;
  rol: UserRole;
  rolAd: string;
  maas?: number;
  yillikIzinHakki: number;
  iseBaslamaTarihi?: string;
  aktif: boolean;
}

// Raporlar
export interface OzetRapor {
  gunlukCiro: number;
  aylikCiro: number;
  bekleyenTeknikServis: number;
  kritikStokUrun: number;
  kasaBakiyesi: number;
  bugunkuSatisSayisi: number;
}

export interface BayiPerformans {
  bayiId: string;
  bayiAd?: string;
  satisSayisi: number;
  satisTutari: number;
  teknikServisSayisi: number;
  toplamCiro: number;
}

export interface PersonelPerformans {
  personelId: string;
  personelAdSoyad?: string;
  satisSayisi: number;
  satisTutari: number;
  teknikServisSayisi: number;
  toplamCiro: number;
}

// Stok
export interface StokDurum {
  urunId: string;
  urunAd?: string;
  barkod?: string;
  toplamStok: number;
  kritikStok: number;
  kritikStokAlti: boolean;
  depoBazliStoklar: DepoBazliStok[];
}

export interface DepoBazliStok {
  depoId: string;
  depoAd: string;
  bayiId?: string;
  bayiAd?: string;
  miktar: number;
}

// Stok Girişi with Fatura
export interface StokGirisFatura {
  tedarikciId: string;
  bayiId: string;
  depoId: string;
  faturaNo?: string;
  faturaTarihi?: string;
  aciklama?: string;
  kalemler: StokGirisKalem[];
}

export interface StokGirisKalem {
  urunId: string;
  alisFiyati: number;
  satisFiyati: number;
  adet: number;
  barkod?: string;
  seriNumaralari?: string[];
}

export interface StokGirisSonuc {
  faturaId: string;
  faturaNo: string;
  toplamAdet: number;
  toplamTutar: number;
  olusturulanBarkodlar: string[];
}
