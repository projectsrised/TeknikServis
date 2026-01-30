using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;
using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.Services;

public class SatisService : ISatisService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public SatisService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<SatisDto?> GetByIdAsync(Guid id)
    {
        var satis = await _unitOfWork.Repository<Satis>()
            .Query()
            .Include(s => s.Bayi)
            .Include(s => s.SatisYapan)
            .Include(s => s.Kalemler)
                .ThenInclude(k => k.Urun)
            .Include(s => s.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
            .FirstOrDefaultAsync(s => s.Id == id && !s.Silindi);

        return satis == null ? null : MapToDto(satis);
    }

    public async Task<SatisDto?> GetBySatisNoAsync(string satisNo)
    {
        var satis = await _unitOfWork.Repository<Satis>()
            .Query()
            .Include(s => s.Bayi)
            .Include(s => s.SatisYapan)
            .Include(s => s.Kalemler)
                .ThenInclude(k => k.Urun)
            .FirstOrDefaultAsync(s => s.SatisNo == satisNo && !s.Silindi);

        return satis == null ? null : MapToDto(satis);
    }

    public async Task<PagedResultDto<SatisDto>> GetListAsync(int page, int pageSize, Guid? bayiId = null, DateTime? baslangic = null, DateTime? bitis = null)
    {
        var query = _unitOfWork.Repository<Satis>()
            .Query()
            .Include(s => s.Bayi)
            .Include(s => s.SatisYapan)
            .Where(s => !s.Silindi);

        if (bayiId.HasValue)
            query = query.Where(s => s.BayiId == bayiId.Value);

        if (baslangic.HasValue)
            query = query.Where(s => s.SatisTarihi >= baslangic.Value);

        if (bitis.HasValue)
            query = query.Where(s => s.SatisTarihi <= bitis.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.SatisTarihi)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<SatisDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponseDto<SatisDto>> CreateAsync(SatisCreateDto dto)
    {
        if (!dto.Kalemler.Any())
            return ApiResponseDto<SatisDto>.Fail("En az bir ürün eklemelisiniz");

        var bayiId = _tenantProvider.BayiId ?? Guid.Empty;
        var kullaniciId = _tenantProvider.KullaniciId ?? Guid.Empty;

        // Satış numarası oluştur
        var satisNo = await GenerateSatisNoAsync();

        var satis = new Satis
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = bayiId,
            SatisNo = satisNo,
            MusteriId = dto.MusteriId,
            MusteriAd = dto.MusteriAd,
            MusteriTelefon = dto.MusteriTelefon,
            SatisYapanId = kullaniciId,
            OdemeTipi = dto.OdemeTipi,
            NakitTutar = dto.NakitTutar,
            KartTutar = dto.KartTutar,
            IndirimTutar = dto.IndirimTutar,
            Aciklama = dto.Aciklama
        };

        decimal araToplam = 0;
        decimal kdvToplam = 0;

        foreach (var kalemDto in dto.Kalemler)
        {
            var seriNumarasi = await _unitOfWork.Repository<SeriNumarasi>()
                .Query()
                .Include(s => s.Urun)
                .FirstOrDefaultAsync(s => s.SeriNo == kalemDto.SeriNo && !s.Satildi && !s.Silindi);

            if (seriNumarasi == null)
                return ApiResponseDto<SatisDto>.Fail($"Seri numarası bulunamadı veya satılmış: {kalemDto.SeriNo}");

            var birimFiyat = seriNumarasi.SatisFiyati ?? seriNumarasi.Urun.SatisFiyat;
            var kdvOran = seriNumarasi.Urun.KdvOran;
            var indirim = kalemDto.IndirimTutar ?? 0;
            var kdvTutar = (birimFiyat - indirim) * kdvOran / 100;
            var toplam = birimFiyat - indirim + kdvTutar;

            var kalem = new SatisKalem
            {
                SatisId = satis.Id,
                UrunId = seriNumarasi.UrunId,
                SeriNumarasiId = seriNumarasi.Id,
                BirimFiyat = birimFiyat,
                KdvOran = kdvOran,
                KdvTutar = kdvTutar,
                IndirimTutar = indirim,
                Toplam = toplam
            };

            satis.Kalemler.Add(kalem);

            araToplam += birimFiyat - indirim;
            kdvToplam += kdvTutar;

            // Seri numarasını satıldı olarak işaretle
            seriNumarasi.Satildi = true;
            seriNumarasi.SatisTarihi = DateTime.UtcNow;
            seriNumarasi.SatisId = satis.Id;
            seriNumarasi.DepoId = null;
        }

        satis.AraToplam = araToplam;
        satis.KdvToplam = kdvToplam;
        satis.GenelToplam = araToplam + kdvToplam - dto.IndirimTutar;

        await _unitOfWork.Repository<Satis>().AddAsync(satis);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<SatisDto>.Ok(await GetByIdAsync(satis.Id) ?? MapToDto(satis), "Satış başarıyla oluşturuldu");
    }

    public async Task<ApiResponseDto<SatisDto>> IptalAsync(Guid id, string? neden = null)
    {
        var satis = await _unitOfWork.Repository<Satis>()
            .Query()
            .Include(s => s.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
            .FirstOrDefaultAsync(s => s.Id == id && !s.Silindi);

        if (satis == null)
            return ApiResponseDto<SatisDto>.Fail("Satış bulunamadı");

        if (satis.IptalEdildi)
            return ApiResponseDto<SatisDto>.Fail("Satış zaten iptal edilmiş");

        satis.IptalEdildi = true;
        satis.IptalNedeni = neden;
        satis.IptalTarihi = DateTime.UtcNow;

        // Seri numaralarını geri al
        foreach (var kalem in satis.Kalemler)
        {
            if (kalem.SeriNumarasi != null)
            {
                kalem.SeriNumarasi.Satildi = false;
                kalem.SeriNumarasi.SatisTarihi = null;
                kalem.SeriNumarasi.SatisId = null;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<SatisDto>.Ok(await GetByIdAsync(id) ?? MapToDto(satis), "Satış iptal edildi");
    }

    public async Task<SatisValidasyonDto> ValidateSeriNumarasiAsync(string seriNo)
    {
        var seriNumarasi = await _unitOfWork.Repository<SeriNumarasi>()
            .Query()
            .Include(s => s.Urun)
            .FirstOrDefaultAsync(s => s.SeriNo == seriNo && !s.Silindi);

        if (seriNumarasi == null)
            return new SatisValidasyonDto { Gecerli = false, Mesaj = "Seri numarası bulunamadı" };

        if (seriNumarasi.Satildi)
            return new SatisValidasyonDto { Gecerli = false, Mesaj = "Bu ürün zaten satılmış" };

        return new SatisValidasyonDto
        {
            Gecerli = true,
            Mesaj = "Ürün satışa uygun",
            SeriNumarasi = new SeriNumarasiDto
            {
                Id = seriNumarasi.Id,
                SeriNo = seriNumarasi.SeriNo,
                UrunId = seriNumarasi.UrunId,
                UrunAd = seriNumarasi.Urun.Ad,
                Satildi = seriNumarasi.Satildi
            },
            SatisFiyat = seriNumarasi.SatisFiyati ?? seriNumarasi.Urun.SatisFiyat,
            KdvOran = seriNumarasi.Urun.KdvOran
        };
    }

    private async Task<string> GenerateSatisNoAsync()
    {
        var today = DateTime.UtcNow;
        var prefix = $"STS-{today:yyyyMMdd}-";
        
        var lastSatis = await _unitOfWork.Repository<Satis>()
            .Query()
            .Where(s => s.SatisNo.StartsWith(prefix))
            .OrderByDescending(s => s.SatisNo)
            .FirstOrDefaultAsync();

        var sequence = 1;
        if (lastSatis != null)
        {
            var lastNumber = lastSatis.SatisNo.Split('-').Last();
            if (int.TryParse(lastNumber, out var num))
                sequence = num + 1;
        }

        return $"{prefix}{sequence:D4}";
    }

    private static SatisDto MapToDto(Satis s) => new()
    {
        Id = s.Id,
        SatisNo = s.SatisNo,
        BayiId = s.BayiId,
        BayiAd = s.Bayi?.Ad ?? "",
        SatisYapanId = s.SatisYapanId,
        SatisYapanAd = s.SatisYapan?.AdSoyad ?? "",
        SatisTarihi = s.SatisTarihi,
        MusteriAd = s.MusteriAd,
        MusteriTelefon = s.MusteriTelefon,
        AraToplam = s.AraToplam,
        KdvToplam = s.KdvToplam,
        IndirimTutar = s.IndirimTutar,
        GenelToplam = s.GenelToplam,
        OdemeTipi = s.OdemeTipi,
        OdemeTipiAd = s.OdemeTipi.ToString(),
        NakitTutar = s.NakitTutar,
        KartTutar = s.KartTutar,
        IptalEdildi = s.IptalEdildi,
        IptalNedeni = s.IptalNedeni,
        Kalemler = s.Kalemler.Select(k => new SatisKalemDto
        {
            Id = k.Id,
            UrunId = k.UrunId,
            UrunAd = k.Urun?.Ad ?? "",
            SeriNo = k.SeriNumarasi?.SeriNo ?? "",
            BirimFiyat = k.BirimFiyat,
            KdvOran = k.KdvOran,
            KdvTutar = k.KdvTutar,
            IndirimTutar = k.IndirimTutar,
            Toplam = k.Toplam
        }).ToList()
    };
}
