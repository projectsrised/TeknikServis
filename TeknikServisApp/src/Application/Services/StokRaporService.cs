using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;
using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.Services;

public class StokService : IStokService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public StokService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<StokDurumDto> GetUrunStokDurumuAsync(Guid urunId, Guid? bayiId = null)
    {
        var urun = await _unitOfWork.Repository<Urun>().GetByIdAsync(urunId);
        if (urun == null) return new StokDurumDto();

        var query = _unitOfWork.Repository<SeriNumarasi>()
            .Query()
            .Include(s => s.Depo).ThenInclude(d => d.Bayi)
            .Where(s => s.UrunId == urunId && !s.Satildi && !s.Silindi && s.DepoId.HasValue);

        if (bayiId.HasValue)
            query = query.Where(s => s.BayiId == bayiId.Value);

        var seriler = await query.ToListAsync();

        var depoBazliStoklar = seriler
            .GroupBy(s => new { s.DepoId, s.Depo?.Ad, s.BayiId, BayiAd = s.Depo?.Bayi?.Ad })
            .Select(g => new DepoBazliStokDto
            {
                DepoId = g.Key.DepoId ?? Guid.Empty,
                DepoAd = g.Key.Ad ?? "",
                BayiId = g.Key.BayiId,
                BayiAd = g.Key.BayiAd ?? "",
                Miktar = g.Count()
            }).ToList();

        var toplamStok = seriler.Count;

        return new StokDurumDto
        {
            UrunId = urunId,
            UrunAd = urun.Ad,
            Barkod = urun.Barkod,
            ToplamStok = toplamStok,
            KritikStok = urun.KritikStok,
            KritikStokAlti = toplamStok < urun.KritikStok,
            DepoBazliStoklar = depoBazliStoklar
        };
    }
public async Task<List<StokDurumDto>> GetTumStokDurumuAsync(Guid? bayiId = null)
{
    var urunler = await _unitOfWork.Repository<Urun>()
        .Query()
        .Where(u => !u.Silindi && u.Aktif)
        .ToListAsync();

    var result = new List<StokDurumDto>();

    foreach (var urun in urunler)
    {
        var stokDurum = await GetUrunStokDurumuAsync(urun.Id, bayiId);
        result.Add(stokDurum);
    }

    return result;
}
    public async Task<List<UrunStokDto>> GetKritikStokUrunlerAsync(Guid? bayiId = null)
    {
        var urunler = await _unitOfWork.Repository<Urun>()
            .Query()
            .Where(u => !u.Silindi && u.Aktif)
            .ToListAsync();

        var kritikUrunler = new List<UrunStokDto>();

        foreach (var urun in urunler)
        {
            var query = _unitOfWork.Repository<SeriNumarasi>()
                .Query()
                .Where(s => s.UrunId == urun.Id && !s.Satildi && !s.Silindi && s.DepoId.HasValue);

            if (bayiId.HasValue)
                query = query.Where(s => s.BayiId == bayiId.Value);

            var stok = await query.CountAsync();

            if (stok < urun.KritikStok)
            {
                kritikUrunler.Add(new UrunStokDto
                {
                    UrunId = urun.Id,
                    UrunAd = urun.Ad,
                    Barkod = urun.Barkod,
                    Marka = urun.Marka,
                    MevcutStok = stok,
                    KritikStok = urun.KritikStok
                });
            }
        }

        return kritikUrunler;
    }

    public async Task<PagedResultDto<StokHareketDto>> GetHareketlerAsync(int page, int pageSize, Guid? urunId = null, Guid? depoId = null, DateTime? baslangic = null, DateTime? bitis = null)
    {
        var query = _unitOfWork.Repository<StokHareket>()
            .Query()
            .Include(h => h.Depo)
            .Include(h => h.Urun)
            .Include(h => h.SeriNumarasi)
            .Where(h => !h.Silindi);

        if (urunId.HasValue) query = query.Where(h => h.UrunId == urunId.Value);
        if (depoId.HasValue) query = query.Where(h => h.DepoId == depoId.Value);
        if (baslangic.HasValue) query = query.Where(h => h.IslemTarihi >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(h => h.IslemTarihi <= bitis.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(h => h.IslemTarihi)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<StokHareketDto>
        {
            Items = items.Select(h => new StokHareketDto
            {
                Id = h.Id,
                DepoId = h.DepoId,
                DepoAd = h.Depo?.Ad ?? "",
                UrunId = h.UrunId,
                UrunAd = h.Urun?.Ad ?? "",
                SeriNo = h.SeriNumarasi?.SeriNo,
                Tip = h.Tip,
                TipAd = h.Tip.ToString(),
                Miktar = h.Miktar,
                IslemTarihi = h.IslemTarihi,
                Aciklama = h.Aciklama
            }).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<SeriNumarasiDetayDto?> GetSeriNumarasiBilgiAsync(string seriNo)
    {
        var seri = await _unitOfWork.Repository<SeriNumarasi>()
            .Query()
            .Include(s => s.Urun)
            .Include(s => s.Depo).ThenInclude(d => d.Bayi)
            .FirstOrDefaultAsync(s => s.SeriNo == seriNo && !s.Silindi);

        if (seri == null) return null;

        return new SeriNumarasiDetayDto
        {
            Id = seri.Id,
            SeriNo = seri.SeriNo,
            UrunId = seri.UrunId,
            UrunAd = seri.Urun?.Ad ?? "",
            MevcutDepoAd = seri.Depo?.Ad,
            MevcutBayiAd = seri.Depo?.Bayi?.Ad,
            Satildi = seri.Satildi,
            SatisTarihi = seri.SatisTarihi,
            AlisFiyati = seri.AlisFiyati,
            SatisFiyati = seri.SatisFiyati
        };
    }

    public async Task<ApiResponseDto<List<SeriNumarasiDto>>> StokGirisiAsync(StokGirisDto dto)
    {
        var urun = await _unitOfWork.Repository<Urun>().GetByIdAsync(dto.UrunId);
        if (urun == null) return ApiResponseDto<List<SeriNumarasiDto>>.Fail("Ürün bulunamadı");

        var depo = await _unitOfWork.Repository<Depo>().GetByIdAsync(dto.DepoId);
        if (depo == null) return ApiResponseDto<List<SeriNumarasiDto>>.Fail("Depo bulunamadı");

        var eklenenler = new List<SeriNumarasiDto>();

        foreach (var seriNo in dto.SeriNumaralari)
        {
            var exists = await _unitOfWork.Repository<SeriNumarasi>()
                .AnyAsync(s => s.SeriNo == seriNo && !s.Silindi);

            if (exists)
                return ApiResponseDto<List<SeriNumarasiDto>>.Fail($"Seri numarası zaten mevcut: {seriNo}");

            var seri = new SeriNumarasi
            {
                TenantId = _tenantProvider.TenantId,
                BayiId = depo.BayiId,
                UrunId = dto.UrunId,
                DepoId = dto.DepoId,
                SeriNo = seriNo,
                AlisFiyati = dto.AlisFiyati ?? urun.AlisFiyat,
                SatisFiyati = urun.SatisFiyat,
                Aciklama = dto.Aciklama
            };

            await _unitOfWork.Repository<SeriNumarasi>().AddAsync(seri);

            // Stok hareketi kaydet
            var hareket = new StokHareket
            {
                TenantId = _tenantProvider.TenantId,
                BayiId = depo.BayiId,
                DepoId = dto.DepoId,
                UrunId = dto.UrunId,
                SeriNumarasiId = seri.Id,
                Tip = StokHareketTip.Giris,
                Miktar = 1,
                Aciklama = dto.Aciklama
            };

            await _unitOfWork.Repository<StokHareket>().AddAsync(hareket);

            eklenenler.Add(new SeriNumarasiDto
            {
                Id = seri.Id,
                SeriNo = seri.SeriNo,
                UrunId = seri.UrunId,
                UrunAd = urun.Ad,
                DepoId = seri.DepoId,
                DepoAd = depo.Ad
            });
        }

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<List<SeriNumarasiDto>>.Ok(eklenenler, $"{eklenenler.Count} adet stok girişi yapıldı");
    }

    public async Task<ApiResponseDto<bool>> StokCikisiAsync(StokCikisDto dto)
    {
        var depo = await _unitOfWork.Repository<Depo>().GetByIdAsync(dto.DepoId);
        if (depo == null) return ApiResponseDto<bool>.Fail("Depo bulunamadı");

        foreach (var seriNo in dto.SeriNumaralari)
        {
            var seri = await _unitOfWork.Repository<SeriNumarasi>()
                .Query()
                .FirstOrDefaultAsync(s => s.SeriNo == seriNo && s.DepoId == dto.DepoId && !s.Satildi && !s.Silindi);

            if (seri == null)
                return ApiResponseDto<bool>.Fail($"Seri numarası depoda bulunamadı: {seriNo}");

            seri.DepoId = null;

            var hareket = new StokHareket
            {
                TenantId = _tenantProvider.TenantId,
                BayiId = depo.BayiId,
                DepoId = dto.DepoId,
                UrunId = seri.UrunId,
                SeriNumarasiId = seri.Id,
                Tip = StokHareketTip.Cikis,
                Miktar = 1,
                Aciklama = dto.Aciklama
            };

            await _unitOfWork.Repository<StokHareket>().AddAsync(hareket);
        }

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, $"{dto.SeriNumaralari.Count} adet stok çıkışı yapıldı");
    }

    public async Task<List<MusteriDto>> GetTedarikciListAsync()
    {
        var tedarikciler = await _unitOfWork.Repository<Musteri>()
            .Query()
            .Where(m => m.Tedarikci && !m.Silindi)
            .OrderBy(m => m.Ad)
            .ToListAsync();

        return tedarikciler.Select(m => new MusteriDto
        {
            Id = m.Id,
            Ad = m.Ad,
            Soyad = m.Soyad,
            AdSoyad = m.AdSoyad,
            Telefon = m.Telefon,
            Email = m.Email,
            Kurumsal = m.Kurumsal,
            Tedarikci = m.Tedarikci
        }).ToList();
    }

    public async Task<ApiResponseDto<StokGirisSonucDto>> StokGirisiWithFaturaAsync(StokGirisFaturaDto dto)
    {
        // Validasyonlar
        var tedarikci = await _unitOfWork.Repository<Musteri>().GetByIdAsync(dto.TedarikciId);
        if (tedarikci == null || !tedarikci.Tedarikci)
            return ApiResponseDto<StokGirisSonucDto>.Fail("Geçerli bir tedarikci seçilmedi");

        var bayi = await _unitOfWork.Repository<Bayi>().GetByIdAsync(dto.BayiId);
        if (bayi == null)
            return ApiResponseDto<StokGirisSonucDto>.Fail("Bayi bulunamadı");

        var depo = await _unitOfWork.Repository<Depo>().GetByIdAsync(dto.DepoId);
        if (depo == null)
            return ApiResponseDto<StokGirisSonucDto>.Fail("Depo bulunamadı");

        if (dto.Kalemler == null || !dto.Kalemler.Any())
            return ApiResponseDto<StokGirisSonucDto>.Fail("En az bir kalem eklenmelidir");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Alış faturası oluştur
            var faturaNo = await GenerateFaturaNo();
            var fatura = new Fatura
            {
                TenantId = _tenantProvider.TenantId,
                BayiId = dto.BayiId,
                FaturaNo = faturaNo,
                Tip = FaturaTip.Alis,
                MusteriId = dto.TedarikciId,
                MusteriAd = tedarikci.AdSoyad,
                MusteriVergiNo = tedarikci.VergiNo,
                FaturaTarihi = dto.FaturaTarihi ?? DateTime.UtcNow,
                Onaylandi = true,
                OnayTarihi = DateTime.UtcNow
            };

            decimal araToplam = 0;
            decimal kdvToplam = 0;
            var olusturulanBarkodlar = new List<string>();
            var toplamAdet = 0;

            foreach (var kalem in dto.Kalemler)
            {
                var urun = await _unitOfWork.Repository<Urun>().GetByIdAsync(kalem.UrunId);
                if (urun == null)
                    return ApiResponseDto<StokGirisSonucDto>.Fail($"Ürün bulunamadı: {kalem.UrunId}");

                // Barkod oluştur (yoksa)
                var barkod = kalem.Barkod;
                if (string.IsNullOrEmpty(barkod))
                {
                    barkod = GenerateBarkod();
                    olusturulanBarkodlar.Add(barkod);

                    // Ürünün barkodunu güncelle (ilk stok girişinde)
                    if (string.IsNullOrEmpty(urun.Barkod))
                    {
                        urun.Barkod = barkod;
                    }
                }

                // Ürün fiyatlarını güncelle
                urun.AlisFiyat = kalem.AlisFiyati;
                urun.SatisFiyat = kalem.SatisFiyati;

                // Fatura kalemi
                var kdvTutar = kalem.AlisFiyati * kalem.Adet * urun.KdvOran / 100;
                var kalemToplam = kalem.AlisFiyati * kalem.Adet + kdvTutar;

                var faturaKalem = new FaturaKalem
                {
                    FaturaId = fatura.Id,
                    UrunId = urun.Id,
                    Aciklama = urun.Ad,
                    Miktar = kalem.Adet,
                    BirimFiyat = kalem.AlisFiyati,
                    KdvOran = urun.KdvOran,
                    KdvTutar = kdvTutar,
                    Toplam = kalemToplam
                };

                fatura.Kalemler.Add(faturaKalem);
                araToplam += kalem.AlisFiyati * kalem.Adet;
                kdvToplam += kdvTutar;

                // Seri numaralı mı?
                if (urun.SeriTakipli && kalem.SeriNumaralari != null && kalem.SeriNumaralari.Any())
                {
                    // Seri numaralı stok girişi
                    foreach (var seriNo in kalem.SeriNumaralari)
                    {
                        var exists = await _unitOfWork.Repository<SeriNumarasi>()
                            .AnyAsync(s => s.SeriNo == seriNo && !s.Silindi);

                        if (exists)
                            return ApiResponseDto<StokGirisSonucDto>.Fail($"Seri numarası zaten mevcut: {seriNo}");

                        var seri = new SeriNumarasi
                        {
                            TenantId = _tenantProvider.TenantId,
                            BayiId = dto.BayiId,
                            UrunId = urun.Id,
                            DepoId = dto.DepoId,
                            SeriNo = seriNo,
                            AlisFiyati = kalem.AlisFiyati,
                            SatisFiyati = kalem.SatisFiyati,
                            Aciklama = $"Fatura: {faturaNo}"
                        };

                        await _unitOfWork.Repository<SeriNumarasi>().AddAsync(seri);

                        var hareket = new StokHareket
                        {
                            TenantId = _tenantProvider.TenantId,
                            BayiId = dto.BayiId,
                            DepoId = dto.DepoId,
                            UrunId = urun.Id,
                            SeriNumarasiId = seri.Id,
                            Tip = StokHareketTip.Giris,
                            Miktar = 1,
                            Aciklama = $"Alış Faturası: {faturaNo}"
                        };

                        await _unitOfWork.Repository<StokHareket>().AddAsync(hareket);
                        toplamAdet++;
                    }
                }
                else
                {
                    // Seri numarasız stok girişi - her adet için barkod bazlı seri oluştur
                    for (int i = 0; i < kalem.Adet; i++)
                    {
                        var seriNo = $"{barkod}-{DateTime.UtcNow:yyyyMMddHHmmss}-{i + 1}";

                        var seri = new SeriNumarasi
                        {
                            TenantId = _tenantProvider.TenantId,
                            BayiId = dto.BayiId,
                            UrunId = urun.Id,
                            DepoId = dto.DepoId,
                            SeriNo = seriNo,
                            AlisFiyati = kalem.AlisFiyati,
                            SatisFiyati = kalem.SatisFiyati,
                            Aciklama = $"Fatura: {faturaNo}"
                        };

                        await _unitOfWork.Repository<SeriNumarasi>().AddAsync(seri);

                        var hareket = new StokHareket
                        {
                            TenantId = _tenantProvider.TenantId,
                            BayiId = dto.BayiId,
                            DepoId = dto.DepoId,
                            UrunId = urun.Id,
                            SeriNumarasiId = seri.Id,
                            Tip = StokHareketTip.Giris,
                            Miktar = 1,
                            Aciklama = $"Alış Faturası: {faturaNo}"
                        };

                        await _unitOfWork.Repository<StokHareket>().AddAsync(hareket);
                        toplamAdet++;
                    }
                }
            }

            fatura.AraToplam = araToplam;
            fatura.KdvToplam = kdvToplam;
            fatura.GenelToplam = araToplam + kdvToplam;

            await _unitOfWork.Repository<Fatura>().AddAsync(fatura);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return ApiResponseDto<StokGirisSonucDto>.Ok(new StokGirisSonucDto
            {
                FaturaId = fatura.Id,
                FaturaNo = faturaNo,
                ToplamAdet = toplamAdet,
                ToplamTutar = fatura.GenelToplam,
                OlusturulanBarkodlar = olusturulanBarkodlar
            }, "Stok girişi ve alış faturası oluşturuldu");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponseDto<StokGirisSonucDto>.Fail($"Stok girişi başarısız: {ex.Message}");
        }
    }

    private async Task<string> GenerateFaturaNo()
    {
        var today = DateTime.UtcNow;
        var prefix = $"AF-{today:yyyyMMdd}";
        var count = await _unitOfWork.Repository<Fatura>()
            .CountAsync(f => f.FaturaNo.StartsWith(prefix));
        return $"{prefix}-{(count + 1):D4}";
    }

    private string GenerateBarkod()
    {
        var random = new Random();
        var barkod = "869";  // Türkiye ülke kodu
        for (int i = 0; i < 10; i++)
        {
            barkod += random.Next(0, 10).ToString();
        }
        return barkod;
    }
}

public class RaporService : IRaporService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public RaporService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<OzetRaporDto> GetOzetRaporAsync(Guid? bayiId = null)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var satisQuery = _unitOfWork.Repository<Satis>().Query().Where(s => !s.Silindi && !s.IptalEdildi);
        var servisQuery = _unitOfWork.Repository<TeknikServisKayit>().Query().Where(t => !t.Silindi);

        if (bayiId.HasValue)
        {
            satisQuery = satisQuery.Where(s => s.BayiId == bayiId.Value);
            servisQuery = servisQuery.Where(t => t.BayiId == bayiId.Value);
        }

        var gunlukCiro = await satisQuery.Where(s => s.SatisTarihi >= today).SumAsync(s => s.GenelToplam);
        var aylikCiro = await satisQuery.Where(s => s.SatisTarihi >= monthStart).SumAsync(s => s.GenelToplam);
        var bugunkuSatis = await satisQuery.Where(s => s.SatisTarihi >= today).CountAsync();
        var bekleyenServis = await servisQuery.Where(t => t.Durum == TeknikServisDurum.Beklemede || t.Durum == TeknikServisDurum.Inceleniyor).CountAsync();

        var kritikStokUrunSayisi = 0;
        var urunler = await _unitOfWork.Repository<Urun>().Query().Where(u => !u.Silindi).ToListAsync();
        foreach (var urun in urunler)
        {
            var stok = await _unitOfWork.Repository<SeriNumarasi>()
                .CountAsync(s => s.UrunId == urun.Id && !s.Satildi && !s.Silindi && s.DepoId.HasValue);
            if (stok < urun.KritikStok) kritikStokUrunSayisi++;
        }

        decimal kasaBakiyesi = 0;
        if (bayiId.HasValue)
        {
            var kasa = await _unitOfWork.Repository<Kasa>().FirstOrDefaultAsync(k => k.BayiId == bayiId.Value && !k.Silindi);
            kasaBakiyesi = kasa?.Bakiye ?? 0;
        }

        return new OzetRaporDto
        {
            GunlukCiro = gunlukCiro,
            AylikCiro = aylikCiro,
            BekleyenTeknikServis = bekleyenServis,
            KritikStokUrun = kritikStokUrunSayisi,
            KasaBakiyesi = kasaBakiyesi,
            BugunkuSatisSayisi = bugunkuSatis
        };
    }

    public async Task<List<GunlukSatisRaporDto>> GetSatisRaporuAsync(DateTime baslangic, DateTime bitis, Guid? bayiId = null)
    {
        var query = _unitOfWork.Repository<Satis>().Query().Where(s => !s.Silindi && !s.IptalEdildi && s.SatisTarihi >= baslangic && s.SatisTarihi <= bitis);

        if (bayiId.HasValue) query = query.Where(s => s.BayiId == bayiId.Value);

        var satislar = await query.ToListAsync();

        return satislar
            .GroupBy(s => s.SatisTarihi.Date)
            .Select(g => new GunlukSatisRaporDto
            {
                Tarih = g.Key,
                SatisSayisi = g.Count(),
                ToplamTutar = g.Sum(s => s.GenelToplam)
            })
            .OrderBy(r => r.Tarih)
            .ToList();
    }

    public async Task<List<BayiPerformansDto>> GetBayiPerformansAsync(DateTime baslangic, DateTime bitis)
    {
        var bayiler = await _unitOfWork.Repository<Bayi>().Query().Where(b => !b.Silindi).ToListAsync();
        var result = new List<BayiPerformansDto>();

        foreach (var bayi in bayiler)
        {
            var satisSayisi = await _unitOfWork.Repository<Satis>()
                .CountAsync(s => s.BayiId == bayi.Id && s.SatisTarihi >= baslangic && s.SatisTarihi <= bitis && !s.Silindi && !s.IptalEdildi);

            var satisTutari = await _unitOfWork.Repository<Satis>()
                .Query()
                .Where(s => s.BayiId == bayi.Id && s.SatisTarihi >= baslangic && s.SatisTarihi <= bitis && !s.Silindi && !s.IptalEdildi)
                .SumAsync(s => s.GenelToplam);

            var servisSayisi = await _unitOfWork.Repository<TeknikServisKayit>()
                .CountAsync(t => t.BayiId == bayi.Id && t.GirisTarihi >= baslangic && t.GirisTarihi <= bitis && !t.Silindi);

            result.Add(new BayiPerformansDto
            {
                BayiId = bayi.Id,
                BayiAd = bayi.Ad,
                SatisSayisi = satisSayisi,
                SatisTutari = satisTutari,
                TeknikServisSayisi = servisSayisi,
                ToplamCiro = satisTutari
            });
        }

        return result.OrderByDescending(r => r.ToplamCiro).ToList();
    }

    public async Task<List<PersonelPerformansDto>> GetPersonelPerformansAsync(DateTime baslangic, DateTime bitis, Guid? bayiId = null)
    {
        var query = _unitOfWork.Repository<Kullanici>().Query().Where(k => !k.Silindi && k.BayiId.HasValue);
        if (bayiId.HasValue) query = query.Where(k => k.BayiId == bayiId.Value);

        var personeller = await query.ToListAsync();
        var result = new List<PersonelPerformansDto>();

        foreach (var personel in personeller)
        {
            var satisSayisi = await _unitOfWork.Repository<Satis>()
                .CountAsync(s => s.SatisYapanId == personel.Id && s.SatisTarihi >= baslangic && s.SatisTarihi <= bitis && !s.Silindi && !s.IptalEdildi);

            var satisTutari = await _unitOfWork.Repository<Satis>()
                .Query()
                .Where(s => s.SatisYapanId == personel.Id && s.SatisTarihi >= baslangic && s.SatisTarihi <= bitis && !s.Silindi && !s.IptalEdildi)
                .SumAsync(s => s.GenelToplam);

            var servisSayisi = await _unitOfWork.Repository<TeknikServisKayit>()
                .CountAsync(t => t.SorumluPersonelId == personel.Id && t.GirisTarihi >= baslangic && t.GirisTarihi <= bitis && !t.Silindi);

            result.Add(new PersonelPerformansDto
            {
                PersonelId = personel.Id,
                PersonelAdSoyad = personel.AdSoyad,
                SatisSayisi = satisSayisi,
                SatisTutari = satisTutari,
                TeknikServisSayisi = servisSayisi,
                ToplamCiro = satisTutari
            });
        }

        return result.OrderByDescending(r => r.ToplamCiro).ToList();
    }

    public async Task<List<UrunStokRaporDto>> GetStokRaporuAsync(Guid? bayiId = null)
    {
        var urunler = await _unitOfWork.Repository<Urun>()
            .Query()
            .Include(u => u.Kategori)
            .Where(u => !u.Silindi)
            .ToListAsync();

        var result = new List<UrunStokRaporDto>();

        foreach (var urun in urunler)
        {
            var query = _unitOfWork.Repository<SeriNumarasi>()
                .Query()
                .Where(s => s.UrunId == urun.Id && !s.Satildi && !s.Silindi && s.DepoId.HasValue);

            if (bayiId.HasValue) query = query.Where(s => s.BayiId == bayiId.Value);

            var stok = await query.CountAsync();

            result.Add(new UrunStokRaporDto
            {
                UrunId = urun.Id,
                UrunAd = urun.Ad,
                Barkod = urun.Barkod,
                KategoriAd = urun.Kategori?.Ad ?? "",
                ToplamStok = stok,
                KritikStok = urun.KritikStok,
                BirimFiyat = urun.SatisFiyat,
                ToplamDeger = stok * urun.SatisFiyat
            });
        }

        return result.OrderBy(r => r.UrunAd).ToList();
    }
}
