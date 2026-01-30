using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;

namespace TeknikServisApp.Application.Services;

public class BayiService : IBayiService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public BayiService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }
public async Task<List<BayiDto>> GetAllAsync()
{
    var bayiler = await _unitOfWork.Repository<Bayi>()
        .Query()
        .Where(b => !b.Silindi)
        .OrderBy(b => b.Ad)
        .ToListAsync();

    return bayiler.Select(MapToDto).ToList();
}
    public async Task<BayiDto?> GetByIdAsync(Guid id)
    {
        var bayi = await _unitOfWork.Repository<Bayi>()
            .Query()
            .FirstOrDefaultAsync(b => b.Id == id && !b.Silindi);

        return bayi == null ? null : MapToDto(bayi);
    }

    public async Task<PagedResultDto<BayiDto>> GetListAsync(int page, int pageSize, bool? aktif = null)
    {
        var query = _unitOfWork.Repository<Bayi>().Query().Where(b => !b.Silindi);

        if (aktif.HasValue)
            query = query.Where(b => b.Aktif == aktif.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(b => b.Ad)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<BayiDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponseDto<BayiDto>> CreateAsync(BayiCreateDto dto)
    {
        var exists = await _unitOfWork.Repository<Bayi>().AnyAsync(b => b.Kod == dto.Kod && !b.Silindi);
        if (exists)
            return ApiResponseDto<BayiDto>.Fail("Bu kod ile kayıtlı bayi zaten var");

        var bayi = new Bayi
        {
            TenantId = _tenantProvider.TenantId,
            Ad = dto.Ad,
            Kod = dto.Kod,
            Adres = dto.Adres,
            Il = dto.Il,
            Ilce = dto.Ilce,
            Telefon = dto.Telefon,
            Email = dto.Email,
            YetkiliAd = dto.YetkiliAd,
            MerkezMi = dto.MerkezMi
        };

        await _unitOfWork.Repository<Bayi>().AddAsync(bayi);

        // Otomatik depo oluştur
        var depo = new Depo
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = bayi.Id,
            Ad = "Ana Depo",
            Kod = $"{dto.Kod}-DEP001",
            MerkezDepoMu = dto.MerkezMi,
            Adres = dto.Adres
        };
        await _unitOfWork.Repository<Depo>().AddAsync(depo);

        // Otomatik kasa oluştur
        var kasa = new Kasa
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = bayi.Id,
            Ad = "Ana Kasa",
            Bakiye = 0
        };
        await _unitOfWork.Repository<Kasa>().AddAsync(kasa);

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<BayiDto>.Ok(MapToDto(bayi), "Bayi başarıyla oluşturuldu");
    }

    public async Task<ApiResponseDto<BayiDto>> UpdateAsync(Guid id, BayiUpdateDto dto)
    {
        var bayi = await _unitOfWork.Repository<Bayi>().GetByIdAsync(id);
        if (bayi == null)
            return ApiResponseDto<BayiDto>.Fail("Bayi bulunamadı");

        if (dto.Ad != null) bayi.Ad = dto.Ad;
        if (dto.Adres != null) bayi.Adres = dto.Adres;
        if (dto.Il != null) bayi.Il = dto.Il;
        if (dto.Ilce != null) bayi.Ilce = dto.Ilce;
        if (dto.Telefon != null) bayi.Telefon = dto.Telefon;
        if (dto.Email != null) bayi.Email = dto.Email;
        if (dto.YetkiliAd != null) bayi.YetkiliAd = dto.YetkiliAd;
        if (dto.Aktif.HasValue) bayi.Aktif = dto.Aktif.Value;

        bayi.GuncellemeTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<BayiDto>.Ok(MapToDto(bayi), "Bayi güncellendi");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        var bayi = await _unitOfWork.Repository<Bayi>().GetByIdAsync(id);
        if (bayi == null)
            return ApiResponseDto<bool>.Fail("Bayi bulunamadı");

        bayi.Silindi = true;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Bayi silindi");
    }

    public async Task<List<DepoDto>> GetDepolarAsync(Guid bayiId)
    {
        var depolar = await _unitOfWork.Repository<Depo>()
            .Query()
            .Where(d => d.BayiId == bayiId && !d.Silindi)
            .ToListAsync();

        return depolar.Select(d => new DepoDto
        {
            Id = d.Id,
            BayiId = d.BayiId,
            Ad = d.Ad,
            Kod = d.Kod,
            MerkezDepoMu = d.MerkezDepoMu,
            Aktif = d.Aktif
        }).ToList();
    }

    public async Task<KasaDto?> GetKasaAsync(Guid bayiId)
    {
        var kasa = await _unitOfWork.Repository<Kasa>()
            .Query()
            .FirstOrDefaultAsync(k => k.BayiId == bayiId && !k.Silindi);

        return kasa == null ? null : new KasaDto
        {
            Id = kasa.Id,
            BayiId = kasa.BayiId,
            Ad = kasa.Ad,
            Bakiye = kasa.Bakiye,
            Aktif = kasa.Aktif
        };
    }

    private static BayiDto MapToDto(Bayi b) => new()
    {
        Id = b.Id,
        Ad = b.Ad,
        Kod = b.Kod,
        Adres = b.Adres,
        Il = b.Il,
        Ilce = b.Ilce,
        Telefon = b.Telefon,
        Email = b.Email,
        YetkiliAd = b.YetkiliAd,
        Aktif = b.Aktif,
        MerkezMi = b.MerkezMi
    };
}

public class DepoService : IDepoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public DepoService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<DepoDto?> GetByIdAsync(Guid id)
    {
        var depo = await _unitOfWork.Repository<Depo>()
            .Query()
            .Include(d => d.Bayi)
            .FirstOrDefaultAsync(d => d.Id == id && !d.Silindi);

        return depo == null ? null : MapToDto(depo);
    }

    public async Task<List<DepoDto>> GetByBayiIdAsync(Guid bayiId)
    {
        var depolar = await _unitOfWork.Repository<Depo>()
            .Query()
            .Include(d => d.Bayi)
            .Where(d => d.BayiId == bayiId && !d.Silindi)
            .ToListAsync();

        return depolar.Select(MapToDto).ToList();
    }

    public async Task<ApiResponseDto<DepoDto>> CreateAsync(DepoCreateDto dto)
    {
        var depo = new Depo
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = dto.BayiId,
            Ad = dto.Ad,
            Kod = dto.Kod,
            MerkezDepoMu = dto.MerkezDepoMu,
            Adres = dto.Adres
        };

        await _unitOfWork.Repository<Depo>().AddAsync(depo);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<DepoDto>.Ok(MapToDto(depo), "Depo oluşturuldu");
    }

    public async Task<ApiResponseDto<DepoDto>> UpdateAsync(Guid id, DepoUpdateDto dto)
    {
        var depo = await _unitOfWork.Repository<Depo>().GetByIdAsync(id);
        if (depo == null)
            return ApiResponseDto<DepoDto>.Fail("Depo bulunamadı");

        if (dto.Ad != null) depo.Ad = dto.Ad;
        if (dto.Adres != null) depo.Adres = dto.Adres;
        if (dto.Aktif.HasValue) depo.Aktif = dto.Aktif.Value;

        depo.GuncellemeTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<DepoDto>.Ok(MapToDto(depo), "Depo güncellendi");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        var depo = await _unitOfWork.Repository<Depo>().GetByIdAsync(id);
        if (depo == null)
            return ApiResponseDto<bool>.Fail("Depo bulunamadı");

        // Depoda ürün var mı kontrol et
        var hasProducts = await _unitOfWork.Repository<SeriNumarasi>()
            .AnyAsync(s => s.DepoId == id && !s.Satildi && !s.Silindi);

        if (hasProducts)
            return ApiResponseDto<bool>.Fail("Depoda ürün bulunduğu için silinemez");

        depo.Silindi = true;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Depo silindi");
    }

    private static DepoDto MapToDto(Depo d) => new()
    {
        Id = d.Id,
        BayiId = d.BayiId,
        BayiAd = d.Bayi?.Ad,
        Ad = d.Ad,
        Kod = d.Kod,
        MerkezDepoMu = d.MerkezDepoMu,
        Adres = d.Adres,
        Aktif = d.Aktif
    };
}

public class MusteriService : IMusteriService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public MusteriService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<MusteriDto?> GetByIdAsync(Guid id)
    {
        var musteri = await _unitOfWork.Repository<Musteri>()
            .Query()
            .FirstOrDefaultAsync(m => m.Id == id && !m.Silindi);

        return musteri == null ? null : MapToDto(musteri);
    }

    public async Task<MusteriDto?> GetByTelefonAsync(string telefon)
    {
        var musteri = await _unitOfWork.Repository<Musteri>()
            .Query()
            .FirstOrDefaultAsync(m => m.Telefon == telefon && !m.Silindi);

        return musteri == null ? null : MapToDto(musteri);
    }

    public async Task<PagedResultDto<MusteriDto>> GetListAsync(int page, int pageSize, string? arama = null)
    {
        var query = _unitOfWork.Repository<Musteri>().Query().Where(m => !m.Silindi);

        if (!string.IsNullOrEmpty(arama))
        {
            arama = arama.ToLower();
            query = query.Where(m =>
                m.Ad.ToLower().Contains(arama) ||
                m.Soyad.ToLower().Contains(arama) ||
                m.Telefon.Contains(arama) ||
                (m.Email != null && m.Email.ToLower().Contains(arama)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(m => m.Ad).ThenBy(m => m.Soyad)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<MusteriDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponseDto<MusteriDto>> CreateAsync(MusteriCreateDto dto)
    {
        // Telefon unique kontrolü
        var telefonExists = await _unitOfWork.Repository<Musteri>()
            .AnyAsync(m => m.Telefon == dto.Telefon && !m.Silindi);
        if (telefonExists)
            return ApiResponseDto<MusteriDto>.Fail("Bu telefon numarası zaten kayıtlı");

        // Email unique kontrolü
        var emailExists = await _unitOfWork.Repository<Musteri>()
            .AnyAsync(m => m.Email == dto.Email && !m.Silindi);
        if (emailExists)
            return ApiResponseDto<MusteriDto>.Fail("Bu email adresi zaten kayıtlı");

        var musteri = new Musteri
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = _tenantProvider.BayiId,
            Ad = dto.Ad,
            Soyad = dto.Soyad,
            Telefon = dto.Telefon,
            Email = dto.Email,
            Adres = dto.Adres,
            TcNo = dto.TcNo,
            VergiNo = dto.VergiNo,
            Kurumsal = dto.Kurumsal,
            Tedarikci = dto.Tedarikci
        };

        await _unitOfWork.Repository<Musteri>().AddAsync(musteri);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<MusteriDto>.Ok(MapToDto(musteri), "Müşteri oluşturuldu");
    }

    public async Task<ApiResponseDto<MusteriDto>> UpdateAsync(Guid id, MusteriUpdateDto dto)
    {
        var musteri = await _unitOfWork.Repository<Musteri>().GetByIdAsync(id);
        if (musteri == null)
            return ApiResponseDto<MusteriDto>.Fail("Müşteri bulunamadı");

        // Telefon değişiyorsa unique kontrolü
        if (dto.Telefon != null && dto.Telefon != musteri.Telefon)
        {
            var telefonExists = await _unitOfWork.Repository<Musteri>()
                .AnyAsync(m => m.Telefon == dto.Telefon && m.Id != id && !m.Silindi);
            if (telefonExists)
                return ApiResponseDto<MusteriDto>.Fail("Bu telefon numarası zaten kayıtlı");
            musteri.Telefon = dto.Telefon;
        }

        // Email değişiyorsa unique kontrolü
        if (dto.Email != null && dto.Email != musteri.Email)
        {
            var emailExists = await _unitOfWork.Repository<Musteri>()
                .AnyAsync(m => m.Email == dto.Email && m.Id != id && !m.Silindi);
            if (emailExists)
                return ApiResponseDto<MusteriDto>.Fail("Bu email adresi zaten kayıtlı");
            musteri.Email = dto.Email;
        }

        if (dto.Ad != null) musteri.Ad = dto.Ad;
        if (dto.Soyad != null) musteri.Soyad = dto.Soyad;
        if (dto.Adres != null) musteri.Adres = dto.Adres;
        if (dto.Tedarikci.HasValue) musteri.Tedarikci = dto.Tedarikci.Value;

        musteri.GuncellemeTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<MusteriDto>.Ok(MapToDto(musteri), "Müşteri güncellendi");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        var musteri = await _unitOfWork.Repository<Musteri>().GetByIdAsync(id);
        if (musteri == null)
            return ApiResponseDto<bool>.Fail("Müşteri bulunamadı");

        musteri.Silindi = true;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Müşteri silindi");
    }

    public async Task<List<SatisDto>> GetSatislarAsync(Guid musteriId)
    {
        var satislar = await _unitOfWork.Repository<Satis>()
            .Query()
            .Include(s => s.Bayi)
            .Where(s => s.MusteriId == musteriId && !s.Silindi)
            .OrderByDescending(s => s.SatisTarihi)
            .Take(20)
            .ToListAsync();

        return satislar.Select(s => new SatisDto
        {
            Id = s.Id,
            SatisNo = s.SatisNo,
            BayiAd = s.Bayi?.Ad ?? "",
            SatisTarihi = s.SatisTarihi,
            GenelToplam = s.GenelToplam,
            IptalEdildi = s.IptalEdildi
        }).ToList();
    }

    public async Task<List<TeknikServisDto>> GetTeknikServislerAsync(Guid musteriId)
    {
        var servisler = await _unitOfWork.Repository<TeknikServisKayit>()
            .Query()
            .Include(t => t.Bayi)
            .Where(t => t.MusteriId == musteriId && !t.Silindi)
            .OrderByDescending(t => t.GirisTarihi)
            .Take(20)
            .ToListAsync();

        return servisler.Select(t => new TeknikServisDto
        {
            Id = t.Id,
            ServisNo = t.ServisNo,
            BayiAd = t.Bayi?.Ad ?? "",
            CihazTip = t.CihazTip,
            CihazMarka = t.CihazMarka,
            Ariza = t.Ariza,
            Durum = t.Durum,
            DurumAd = t.Durum.ToString(),
            GirisTarihi = t.GirisTarihi
        }).ToList();
    }

    private static MusteriDto MapToDto(Musteri m) => new()
    {
        Id = m.Id,
        Ad = m.Ad,
        Soyad = m.Soyad,
        AdSoyad = m.AdSoyad,
        Telefon = m.Telefon,
        Email = m.Email,
        Adres = m.Adres,
        TcNo = m.TcNo,
        VergiNo = m.VergiNo,
        Kurumsal = m.Kurumsal,
        Tedarikci = m.Tedarikci
    };
}
