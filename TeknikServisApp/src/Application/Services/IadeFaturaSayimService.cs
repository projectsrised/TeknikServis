using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;
using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.Services;

public class IadeService : IIadeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public IadeService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<IadeDto?> GetByIdAsync(Guid id)
    {
        var iade = await _unitOfWork.Repository<Iade>()
            .Query()
            .Include(i => i.Satis)
            .Include(i => i.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
                    .ThenInclude(s => s.Urun)
            .FirstOrDefaultAsync(i => i.Id == id && !i.Silindi);

        return iade == null ? null : MapToDto(iade);
    }

    public async Task<IadeDto?> GetByIadeNoAsync(string iadeNo)
    {
        var iade = await _unitOfWork.Repository<Iade>()
            .Query()
            .Include(i => i.Satis)
            .FirstOrDefaultAsync(i => i.IadeNo == iadeNo && !i.Silindi);

        return iade == null ? null : MapToDto(iade);
    }

    public async Task<PagedResultDto<IadeDto>> GetListAsync(int page, int pageSize, Guid? bayiId = null, string? durum = null)
    {
        var query = _unitOfWork.Repository<Iade>()
            .Query()
            .Include(i => i.Satis)
            .Where(i => !i.Silindi);

        if (bayiId.HasValue)
            query = query.Where(i => i.BayiId == bayiId.Value);

        if (!string.IsNullOrEmpty(durum) && Enum.TryParse<IadeDurum>(durum, out var durumEnum))
            query = query.Where(i => i.Durum == durumEnum);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.IadeTarihi).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResultDto<IadeDto> { Items = items.Select(MapToDto).ToList(), TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<ApiResponseDto<IadeDto>> CreateAsync(IadeCreateDto dto)
    {
        var satis = await _unitOfWork.Repository<Satis>()
            .Query()
            .Include(s => s.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
            .FirstOrDefaultAsync(s => s.Id == dto.SatisId && !s.Silindi);

        if (satis == null)
            return ApiResponseDto<IadeDto>.Fail("Satış bulunamadı");

        var iadeNo = $"IAD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        var iade = new Iade
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = satis.BayiId,
            IadeNo = iadeNo,
            SatisId = satis.Id,
            MusteriId = satis.MusteriId,
            IslemYapanId = _tenantProvider.KullaniciId ?? Guid.Empty,
            Neden = dto.Neden,
            Durum = IadeDurum.Beklemede
        };

        decimal toplam = 0;
        foreach (var kalemDto in dto.Kalemler)
        {
            var satisKalem = satis.Kalemler.FirstOrDefault(k => k.Id == kalemDto.SatisKalemId);
            if (satisKalem == null) continue;

            var iadeKalem = new IadeKalem
            {
                IadeId = iade.Id,
                SatisKalemId = satisKalem.Id,
                SeriNumarasiId = satisKalem.SeriNumarasiId,
                IadeTutar = satisKalem.Toplam,
                Neden = kalemDto.Neden
            };
            iade.Kalemler.Add(iadeKalem);
            toplam += satisKalem.Toplam;
        }

        iade.ToplamTutar = toplam;
        await _unitOfWork.Repository<Iade>().AddAsync(iade);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<IadeDto>.Ok(MapToDto(iade), "İade talebi oluşturuldu");
    }

    public async Task<ApiResponseDto<IadeDto>> OnaylaAsync(Guid id)
    {
        var iade = await _unitOfWork.Repository<Iade>().GetByIdAsync(id);
        if (iade == null) return ApiResponseDto<IadeDto>.Fail("İade bulunamadı");

        iade.Durum = IadeDurum.Onaylandi;
        iade.OnayTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<IadeDto>.Ok(await GetByIdAsync(id) ?? MapToDto(iade), "İade onaylandı");
    }

    public async Task<ApiResponseDto<IadeDto>> TamamlaAsync(Guid id)
    {
        var iade = await _unitOfWork.Repository<Iade>()
            .Query()
            .Include(i => i.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
            .FirstOrDefaultAsync(i => i.Id == id && !i.Silindi);

        if (iade == null) return ApiResponseDto<IadeDto>.Fail("İade bulunamadı");

        foreach (var kalem in iade.Kalemler)
        {
            if (kalem.SeriNumarasi != null)
            {
                kalem.SeriNumarasi.Satildi = false;
                kalem.SeriNumarasi.SatisTarihi = null;
                kalem.SeriNumarasi.SatisId = null;
            }
        }

        iade.Durum = IadeDurum.Tamamlandi;
        iade.IadeEdilen = iade.ToplamTutar;
        iade.TamamlanmaTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<IadeDto>.Ok(await GetByIdAsync(id) ?? MapToDto(iade), "İade tamamlandı");
    }

    public async Task<ApiResponseDto<IadeDto>> ReddetAsync(Guid id, string? neden = null)
    {
        var iade = await _unitOfWork.Repository<Iade>().GetByIdAsync(id);
        if (iade == null) return ApiResponseDto<IadeDto>.Fail("İade bulunamadı");

        iade.Durum = IadeDurum.Reddedildi;
        iade.RedNedeni = neden;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<IadeDto>.Ok(await GetByIdAsync(id) ?? MapToDto(iade), "İade reddedildi");
    }

    private static IadeDto MapToDto(Iade i) => new()
    {
        Id = i.Id,
        IadeNo = i.IadeNo,
        SatisId = i.SatisId,
        SatisNo = i.Satis?.SatisNo ?? "",
        MusteriAd = i.Musteri?.AdSoyad,
        Durum = i.Durum,
        DurumAd = i.Durum.ToString(),
        IadeTarihi = i.IadeTarihi,
        ToplamTutar = i.ToplamTutar,
        IadeEdilen = i.IadeEdilen,
        Neden = i.Neden,
        Kalemler = i.Kalemler?.Select(k => new IadeKalemDto
        {
            Id = k.Id,
            UrunAd = k.SeriNumarasi?.Urun?.Ad ?? "",
            SeriNo = k.SeriNumarasi?.SeriNo ?? "",
            IadeTutar = k.IadeTutar,
            Neden = k.Neden
        }).ToList() ?? new()
    };
}

public class FaturaService : IFaturaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public FaturaService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<FaturaDto?> GetByIdAsync(Guid id)
    {
        var fatura = await _unitOfWork.Repository<Fatura>()
            .Query()
            .Include(f => f.Kalemler)
            .FirstOrDefaultAsync(f => f.Id == id && !f.Silindi);

        return fatura == null ? null : MapToDto(fatura);
    }

    public async Task<PagedResultDto<FaturaDto>> GetListAsync(int page, int pageSize, string? tip = null, bool? onaylandi = null)
    {
        var query = _unitOfWork.Repository<Fatura>().Query().Where(f => !f.Silindi);

        if (!string.IsNullOrEmpty(tip) && Enum.TryParse<FaturaTip>(tip, out var tipEnum))
            query = query.Where(f => f.Tip == tipEnum);

        if (onaylandi.HasValue)
            query = query.Where(f => f.Onaylandi == onaylandi.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(f => f.FaturaTarihi).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResultDto<FaturaDto> { Items = items.Select(MapToDto).ToList(), TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<ApiResponseDto<FaturaDto>> CreateAsync(FaturaCreateDto dto)
    {
        var faturaNo = $"FTR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        var fatura = new Fatura
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = _tenantProvider.BayiId ?? Guid.Empty,
            FaturaNo = faturaNo,
            Tip = dto.Tip,
            MusteriId = dto.MusteriId,
            MusteriAd = dto.MusteriAd,
            MusteriAdres = dto.MusteriAdres,
            MusteriVergiNo = dto.MusteriVergiNo,
            SatisId = dto.SatisId,
            TeknikServisId = dto.TeknikServisId
        };

        decimal araToplam = 0, kdvToplam = 0;
        foreach (var kalemDto in dto.Kalemler)
        {
            var kdvTutar = kalemDto.BirimFiyat * kalemDto.Miktar * kalemDto.KdvOran / 100;
            var kalem = new FaturaKalem
            {
                FaturaId = fatura.Id,
                UrunId = kalemDto.UrunId,
                Aciklama = kalemDto.Aciklama,
                Miktar = kalemDto.Miktar,
                BirimFiyat = kalemDto.BirimFiyat,
                KdvOran = kalemDto.KdvOran,
                KdvTutar = kdvTutar,
                Toplam = kalemDto.BirimFiyat * kalemDto.Miktar + kdvTutar
            };
            fatura.Kalemler.Add(kalem);
            araToplam += kalemDto.BirimFiyat * kalemDto.Miktar;
            kdvToplam += kdvTutar;
        }

        fatura.AraToplam = araToplam;
        fatura.KdvToplam = kdvToplam;
        fatura.GenelToplam = araToplam + kdvToplam;

        await _unitOfWork.Repository<Fatura>().AddAsync(fatura);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<FaturaDto>.Ok(MapToDto(fatura), "Fatura oluşturuldu");
    }

    public async Task<ApiResponseDto<FaturaDto>> OnaylaAsync(Guid id)
    {
        var fatura = await _unitOfWork.Repository<Fatura>().GetByIdAsync(id);
        if (fatura == null) return ApiResponseDto<FaturaDto>.Fail("Fatura bulunamadı");

        fatura.Onaylandi = true;
        fatura.OnayTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<FaturaDto>.Ok(await GetByIdAsync(id) ?? MapToDto(fatura), "Fatura onaylandı");
    }

    public async Task<ApiResponseDto<bool>> IptalAsync(Guid id, string? neden = null)
    {
        var fatura = await _unitOfWork.Repository<Fatura>().GetByIdAsync(id);
        if (fatura == null) return ApiResponseDto<bool>.Fail("Fatura bulunamadı");

        fatura.IptalEdildi = true;
        fatura.IptalNedeni = neden;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Fatura iptal edildi");
    }

    private static FaturaDto MapToDto(Fatura f) => new()
    {
        Id = f.Id,
        FaturaNo = f.FaturaNo,
        Tip = f.Tip,
        TipAd = f.Tip.ToString(),
        MusteriAd = f.MusteriAd,
        MusteriVergiNo = f.MusteriVergiNo,
        FaturaTarihi = f.FaturaTarihi,
        AraToplam = f.AraToplam,
        KdvToplam = f.KdvToplam,
        GenelToplam = f.GenelToplam,
        Onaylandi = f.Onaylandi,
        IptalEdildi = f.IptalEdildi,
        Kalemler = f.Kalemler?.Select(k => new FaturaKalemDto
        {
            Id = k.Id, Aciklama = k.Aciklama, Miktar = k.Miktar, BirimFiyat = k.BirimFiyat,
            KdvOran = k.KdvOran, KdvTutar = k.KdvTutar, Toplam = k.Toplam
        }).ToList() ?? new()
    };
}

public class SayimService : ISayimService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public SayimService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<SayimDto?> GetByIdAsync(Guid id)
    {
        var sayim = await _unitOfWork.Repository<Sayim>()
            .Query()
            .Include(s => s.Depo)
            .Include(s => s.Baslatan)
            .Include(s => s.Kalemler).ThenInclude(k => k.Urun)
            .FirstOrDefaultAsync(s => s.Id == id && !s.Silindi);

        return sayim == null ? null : MapToDto(sayim);
    }

    public async Task<SayimDto?> GetBySayimNoAsync(string sayimNo)
    {
        var sayim = await _unitOfWork.Repository<Sayim>()
            .Query()
            .Include(s => s.Depo)
            .Include(s => s.Baslatan)
            .FirstOrDefaultAsync(s => s.SayimNo == sayimNo && !s.Silindi);

        return sayim == null ? null : MapToDto(sayim);
    }

    public async Task<PagedResultDto<SayimDto>> GetListAsync(int page, int pageSize, Guid? depoId = null, string? durum = null)
    {
        var query = _unitOfWork.Repository<Sayim>()
            .Query()
            .Include(s => s.Depo)
            .Include(s => s.Baslatan)
            .Where(s => !s.Silindi);

        if (depoId.HasValue) query = query.Where(s => s.DepoId == depoId.Value);
        if (!string.IsNullOrEmpty(durum) && Enum.TryParse<SayimDurum>(durum, out var durumEnum))
            query = query.Where(s => s.Durum == durumEnum);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(s => s.BaslangicTarihi).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResultDto<SayimDto> { Items = items.Select(MapToDto).ToList(), TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<ApiResponseDto<SayimDto>> BaslatAsync(SayimBaslatDto dto)
    {
        var sistemStok = await _unitOfWork.Repository<SeriNumarasi>()
            .CountAsync(s => s.DepoId == dto.DepoId && !s.Satildi && !s.Silindi);

        var sayimNo = $"SYM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

        var sayim = new Sayim
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = _tenantProvider.BayiId ?? Guid.Empty,
            SayimNo = sayimNo,
            DepoId = dto.DepoId,
            BaslatanId = _tenantProvider.KullaniciId ?? Guid.Empty,
            SistemStok = sistemStok,
            Aciklama = dto.Aciklama,
            Durum = SayimDurum.Basladi
        };

        await _unitOfWork.Repository<Sayim>().AddAsync(sayim);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<SayimDto>.Ok(MapToDto(sayim), "Sayım başlatıldı");
    }

    public async Task<ApiResponseDto<SayimSeriTaramaResultDto>> SeriTaraAsync(Guid sayimId, string seriNo)
    {
        var sayim = await _unitOfWork.Repository<Sayim>()
            .Query()
            .Include(s => s.Kalemler)
            .FirstOrDefaultAsync(s => s.Id == sayimId && !s.Silindi);

        if (sayim == null)
            return ApiResponseDto<SayimSeriTaramaResultDto>.Fail("Sayım bulunamadı");

        var seriNumarasi = await _unitOfWork.Repository<SeriNumarasi>()
            .Query()
            .Include(s => s.Urun)
            .FirstOrDefaultAsync(s => s.SeriNo == seriNo && !s.Silindi);

        var sistemdeVar = seriNumarasi != null && seriNumarasi.DepoId == sayim.DepoId && !seriNumarasi.Satildi;
        var fizikiSayimda = sayim.Kalemler.Any(k => k.SeriNo == seriNo);

        if (!fizikiSayimda)
        {
            var kalem = new SayimKalem
            {
                SayimId = sayimId,
                UrunId = seriNumarasi?.UrunId ?? Guid.Empty,
                SeriNumarasiId = seriNumarasi?.Id,
                SeriNo = seriNo,
                SistemdeVar = sistemdeVar,
                FizikiSayimda = true
            };
            sayim.Kalemler.Add(kalem);
            sayim.FizikselStok = sayim.Kalemler.Count(k => k.FizikiSayimda);
            sayim.Fark = sayim.FizikselStok - sayim.SistemStok;
            await _unitOfWork.SaveChangesAsync();
        }

        return ApiResponseDto<SayimSeriTaramaResultDto>.Ok(new SayimSeriTaramaResultDto
        {
            Basarili = true,
            Mesaj = sistemdeVar ? "Ürün sistemde mevcut" : "Ürün sistemde bulunamadı",
            SeriNo = seriNo,
            SistemdeVar = sistemdeVar,
            FizikiSayimda = true,
            UrunAd = seriNumarasi?.Urun?.Ad
        });
    }

    public async Task<ApiResponseDto<SayimRaporDto>> TamamlaAsync(Guid id)
    {
        var sayim = await _unitOfWork.Repository<Sayim>()
            .Query()
            .Include(s => s.Depo)
            .Include(s => s.Kalemler).ThenInclude(k => k.Urun)
            .FirstOrDefaultAsync(s => s.Id == id && !s.Silindi);

        if (sayim == null) return ApiResponseDto<SayimRaporDto>.Fail("Sayım bulunamadı");

        sayim.Durum = SayimDurum.Tamamlandi;
        sayim.BitisTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<SayimRaporDto>.Ok(await GetRaporAsync(id) ?? new SayimRaporDto(), "Sayım tamamlandı");
    }

    public async Task<SayimRaporDto?> GetRaporAsync(Guid id)
    {
        var sayim = await _unitOfWork.Repository<Sayim>()
            .Query()
            .Include(s => s.Depo)
            .Include(s => s.Kalemler).ThenInclude(k => k.Urun)
            .FirstOrDefaultAsync(s => s.Id == id && !s.Silindi);

        if (sayim == null) return null;

        return new SayimRaporDto
        {
            SayimId = sayim.Id,
            SayimNo = sayim.SayimNo,
            DepoAd = sayim.Depo?.Ad ?? "",
            SistemStok = sayim.SistemStok,
            FizikselStok = sayim.FizikselStok,
            Fark = sayim.Fark,
            EksikUrun = sayim.Kalemler.Count(k => k.SistemdeVar && !k.FizikiSayimda),
            FazlaUrun = sayim.Kalemler.Count(k => !k.SistemdeVar && k.FizikiSayimda),
            Farklar = sayim.Kalemler.Where(k => k.SistemdeVar != k.FizikiSayimda).Select(k => new SayimFarkDto
            {
                UrunAd = k.Urun?.Ad ?? "",
                SeriNo = k.SeriNo,
                Durum = k.SistemdeVar ? "Eksik" : "Fazla"
            }).ToList()
        };
    }

    public async Task<ApiResponseDto<SayimDto>> IptalAsync(Guid id)
    {
        var sayim = await _unitOfWork.Repository<Sayim>().GetByIdAsync(id);
        if (sayim == null) return ApiResponseDto<SayimDto>.Fail("Sayım bulunamadı");

        sayim.Durum = SayimDurum.IptalEdildi;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<SayimDto>.Ok(await GetByIdAsync(id) ?? MapToDto(sayim), "Sayım iptal edildi");
    }

    private static SayimDto MapToDto(Sayim s) => new()
    {
        Id = s.Id,
        SayimNo = s.SayimNo,
        DepoId = s.DepoId,
        DepoAd = s.Depo?.Ad ?? "",
        BaslatanAd = s.Baslatan?.AdSoyad ?? "",
        Durum = s.Durum,
        DurumAd = s.Durum.ToString(),
        BaslangicTarihi = s.BaslangicTarihi,
        BitisTarihi = s.BitisTarihi,
        SistemStok = s.SistemStok,
        FizikselStok = s.FizikselStok,
        Fark = s.Fark,
        Kalemler = s.Kalemler?.Select(k => new SayimKalemDto
        {
            Id = k.Id, UrunAd = k.Urun?.Ad ?? "", SeriNo = k.SeriNo,
            SistemdeVar = k.SistemdeVar, FizikiSayimda = k.FizikiSayimda, TaramaTarihi = k.TaramaTarihi
        }).ToList() ?? new()
    };
}
