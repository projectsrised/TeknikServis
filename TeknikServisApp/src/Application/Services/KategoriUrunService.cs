using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Helpers;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;
using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.Services;

public class KategoriService : IKategoriService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserService _currentUserService;

    public KategoriService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
        _currentUserService = currentUserService;
    }

    public async Task<List<KategoriDto>> GetAllAsync()
    {
        // Kategori global - TenantId filtresi yok
        var kategoriler = await _unitOfWork.Repository<Kategori>()
            .Query()
            .IgnoreQueryFilters() // Tenant filter'ı yoksay
            .Include(k => k.UstKategori)
            .Where(k => !k.Silindi)
            .OrderBy(k => k.Sira)
            .ToListAsync();

        return kategoriler.Select(MapToDto).ToList();
    }

    public async Task<List<KategoriTreeDto>> GetTreeAsync()
    {
        var kategoriler = await _unitOfWork.Repository<Kategori>()
            .Query()
            .IgnoreQueryFilters() // Tenant filter'ı yoksay
            .Where(k => !k.Silindi)
            .OrderBy(k => k.Sira)
            .ToListAsync();

        var rootKategoriler = kategoriler.Where(k => k.UstKategoriId == null);
        return rootKategoriler.Select(k => BuildTree(k, kategoriler)).ToList();
    }

    private KategoriTreeDto BuildTree(Kategori kategori, List<Kategori> tumKategoriler)
    {
        var altKategoriler = tumKategoriler.Where(k => k.UstKategoriId == kategori.Id);
        return new KategoriTreeDto
        {
            Id = kategori.Id,
            Ad = kategori.Ad,
            Kod = kategori.Kod,
            Tip = kategori.Tip,
            TipAd = kategori.Tip.ToString(),
            Sira = kategori.Sira,
            Aktif = kategori.Aktif,
            AltKategoriler = altKategoriler.Select(k => BuildTree(k, tumKategoriler)).ToList()
        };
    }

    public async Task<KategoriDto?> GetByIdAsync(Guid id)
    {
        var kategori = await _unitOfWork.Repository<Kategori>()
            .Query()
            .IgnoreQueryFilters()
            .Include(k => k.UstKategori)
            .FirstOrDefaultAsync(k => k.Id == id && !k.Silindi);

        return kategori == null ? null : MapToDto(kategori);
    }

    public async Task<ApiResponseDto<KategoriDto>> CreateAsync(KategoriCreateDto dto)
    {
        // Sadece SuperAdmin kategori ekleyebilir
        if (_currentUserService.Rol != UserRole.SuperAdmin)
            return ApiResponseDto<KategoriDto>.Fail("Sadece SuperAdmin kategori ekleyebilir");

        // Kod kontrolü
        if (!string.IsNullOrEmpty(dto.Kod))
        {
            var kodExists = await _unitOfWork.Repository<Kategori>()
                .Query()
                .IgnoreQueryFilters()
                .AnyAsync(k => k.Kod == dto.Kod && !k.Silindi);

            if (kodExists)
                return ApiResponseDto<KategoriDto>.Fail("Bu kod zaten kullanılıyor");
        }

        // %70 benzerlik kontrolü
        var tumKategoriler = await _unitOfWork.Repository<Kategori>()
            .Query()
            .IgnoreQueryFilters()
            .Where(k => !k.Silindi)
            .Select(k => new { k.Id, k.Ad })
            .ToListAsync();

        foreach (var existingKategori in tumKategoriler)
        {
            if (StringHelper.IsSimilar(dto.Ad, existingKategori.Ad, 0.70))
            {
                return ApiResponseDto<KategoriDto>.Fail(
                    $"'{dto.Ad}' kategorisi, mevcut '{existingKategori.Ad}' kategorisi ile çok benzer. Lütfen farklı bir isim kullanın.");
            }
        }

        var kategori = new Kategori
        {
            Ad = dto.Ad,
            Kod = dto.Kod ?? GenerateKategoriKod(dto.Ad),
            Tip = dto.Tip,
            UstKategoriId = dto.UstKategoriId,
            Sira = dto.Sira,
            Aciklama = dto.Aciklama
        };

        await _unitOfWork.Repository<Kategori>().AddAsync(kategori);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<KategoriDto>.Ok(MapToDto(kategori), "Kategori oluşturuldu");
    }

    public async Task<ApiResponseDto<KategoriDto>> UpdateAsync(Guid id, KategoriUpdateDto dto)
    {
        // Sadece SuperAdmin kategori güncelleyebilir
        if (_currentUserService.Rol != UserRole.SuperAdmin)
            return ApiResponseDto<KategoriDto>.Fail("Sadece SuperAdmin kategori güncelleyebilir");

        var kategori = await _unitOfWork.Repository<Kategori>()
            .Query()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(k => k.Id == id && !k.Silindi);

        if (kategori == null)
            return ApiResponseDto<KategoriDto>.Fail("Kategori bulunamadı");

        // İsim değişiyorsa benzerlik kontrolü
        if (dto.Ad != null && dto.Ad != kategori.Ad)
        {
            var tumKategoriler = await _unitOfWork.Repository<Kategori>()
                .Query()
                .IgnoreQueryFilters()
                .Where(k => !k.Silindi && k.Id != id)
                .Select(k => new { k.Id, k.Ad })
                .ToListAsync();

            foreach (var existingKategori in tumKategoriler)
            {
                if (StringHelper.IsSimilar(dto.Ad, existingKategori.Ad, 0.70))
                {
                    return ApiResponseDto<KategoriDto>.Fail(
                        $"'{dto.Ad}' kategorisi, mevcut '{existingKategori.Ad}' kategorisi ile çok benzer.");
                }
            }

            kategori.Ad = dto.Ad;
        }

        if (dto.Kod != null) kategori.Kod = dto.Kod;
        if (dto.Tip.HasValue) kategori.Tip = dto.Tip.Value;
        if (dto.UstKategoriId.HasValue) kategori.UstKategoriId = dto.UstKategoriId;
        if (dto.Sira.HasValue) kategori.Sira = dto.Sira.Value;
        if (dto.Aktif.HasValue) kategori.Aktif = dto.Aktif.Value;
        if (dto.Aciklama != null) kategori.Aciklama = dto.Aciklama;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<KategoriDto>.Ok(MapToDto(kategori), "Kategori güncellendi");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        // Sadece SuperAdmin kategori silebilir
        if (_currentUserService.Rol != UserRole.SuperAdmin)
            return ApiResponseDto<bool>.Fail("Sadece SuperAdmin kategori silebilir");

        var kategori = await _unitOfWork.Repository<Kategori>()
            .Query()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(k => k.Id == id && !k.Silindi);

        if (kategori == null)
            return ApiResponseDto<bool>.Fail("Kategori bulunamadı");

        var hasProducts = await _unitOfWork.Repository<Urun>()
            .AnyAsync(u => u.KategoriId == id && !u.Silindi);

        if (hasProducts)
            return ApiResponseDto<bool>.Fail("Bu kategoride ürünler var, silinemez");

        kategori.Silindi = true;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Kategori silindi");
    }

    private string GenerateKategoriKod(string ad)
    {
        var normalized = StringHelper.NormalizeString(ad);
        var kod = normalized.Length > 3 ? normalized.Substring(0, 3).ToUpper() : normalized.ToUpper();
        return $"KAT-{kod}-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }

    private static KategoriDto MapToDto(Kategori k) => new()
    {
        Id = k.Id,
        Ad = k.Ad,
        Kod = k.Kod,
        Tip = k.Tip,
        TipAd = k.Tip.ToString(),
        UstKategoriId = k.UstKategoriId,
        UstKategoriAd = k.UstKategori?.Ad,
        Sira = k.Sira,
        Aktif = k.Aktif,
        Aciklama = k.Aciklama
    };
}

public class UrunService : IUrunService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public UrunService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<PagedResultDto<UrunDto>> GetListAsync(int page, int pageSize, Guid? kategoriId = null, bool? aktif = null, string? arama = null)
    {
        var query = _unitOfWork.Repository<Urun>()
            .Query()
            .Include(u => u.Kategori)
            .Where(u => !u.Silindi);

        if (kategoriId.HasValue)
            query = query.Where(u => u.KategoriId == kategoriId.Value);

        if (aktif.HasValue)
            query = query.Where(u => u.Aktif == aktif.Value);

        if (!string.IsNullOrEmpty(arama))
            query = query.Where(u => u.Ad.Contains(arama) || (u.Barkod != null && u.Barkod.Contains(arama)) || (u.Kod != null && u.Kod.Contains(arama)));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.Ad)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<UrunDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UrunDto?> GetByIdAsync(Guid id)
    {
        var urun = await _unitOfWork.Repository<Urun>()
            .Query()
            .Include(u => u.Kategori)
            .FirstOrDefaultAsync(u => u.Id == id && !u.Silindi);

        return urun == null ? null : MapToDto(urun);
    }

    public async Task<UrunDto?> GetByBarkodAsync(string barkod)
    {
        var urun = await _unitOfWork.Repository<Urun>()
            .Query()
            .Include(u => u.Kategori)
            .FirstOrDefaultAsync(u => u.Barkod == barkod && !u.Silindi);

        return urun == null ? null : MapToDto(urun);
    }

    public async Task<List<SeriNumarasiDto>> GetSeriNumaralariAsync(Guid urunId, Guid? depoId = null)
    {
        var query = _unitOfWork.Repository<SeriNumarasi>()
            .Query()
            .Include(s => s.Urun)
            .Include(s => s.Depo)
            .Where(s => s.UrunId == urunId && !s.Satildi && !s.Silindi);

        if (depoId.HasValue)
            query = query.Where(s => s.DepoId == depoId.Value);

        var seriler = await query.ToListAsync();

        return seriler.Select(s => new SeriNumarasiDto
        {
            Id = s.Id,
            SeriNo = s.SeriNo,
            UrunId = s.UrunId,
            UrunAd = s.Urun?.Ad ?? "",
            DepoId = s.DepoId,
            DepoAd = s.Depo?.Ad,
            Satildi = s.Satildi,
            AlisFiyati = s.AlisFiyati,
            SatisFiyati = s.SatisFiyati
        }).ToList();
    }

    public async Task<int> GetStokAsync(Guid urunId, Guid? depoId = null)
    {
        var query = _unitOfWork.Repository<SeriNumarasi>()
            .Query()
            .Where(s => s.UrunId == urunId && !s.Satildi && !s.Silindi && s.DepoId.HasValue);

        if (depoId.HasValue)
            query = query.Where(s => s.DepoId == depoId.Value);

        return await query.CountAsync();
    }

    public async Task<ApiResponseDto<UrunDto>> CreateAsync(UrunCreateDto dto)
    {
        // Kod otomatik oluşturulacak
        var kod = await GenerateUrunKod();

        var urun = new Urun
        {
            TenantId = _tenantProvider.TenantId,
            KategoriId = dto.KategoriId,
            Ad = dto.Ad,
            Kod = kod,
            KritikStok = dto.KritikStok,
            SeriTakipli = dto.SeriTakipli,
            // Fiyatlar stok girişinde belirlenecek
            AlisFiyat = 0,
            SatisFiyat = 0,
            KdvOran = 20
        };

        await _unitOfWork.Repository<Urun>().AddAsync(urun);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<UrunDto>.Ok(MapToDto(urun), "Ürün oluşturuldu");
    }

    public async Task<ApiResponseDto<UrunDto>> UpdateAsync(Guid id, UrunUpdateDto dto)
    {
        var urun = await _unitOfWork.Repository<Urun>().GetByIdAsync(id);
        if (urun == null)
            return ApiResponseDto<UrunDto>.Fail("Ürün bulunamadı");

        if (dto.Ad != null) urun.Ad = dto.Ad;
        if (dto.KategoriId.HasValue) urun.KategoriId = dto.KategoriId.Value;
        if (dto.KritikStok.HasValue) urun.KritikStok = dto.KritikStok.Value;
        if (dto.Aktif.HasValue) urun.Aktif = dto.Aktif.Value;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<UrunDto>.Ok(await GetByIdAsync(id) ?? MapToDto(urun), "Ürün güncellendi");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        var urun = await _unitOfWork.Repository<Urun>().GetByIdAsync(id);
        if (urun == null)
            return ApiResponseDto<bool>.Fail("Ürün bulunamadı");

        var hasStock = await _unitOfWork.Repository<SeriNumarasi>()
            .AnyAsync(s => s.UrunId == id && !s.Satildi && !s.Silindi);

        if (hasStock)
            return ApiResponseDto<bool>.Fail("Bu üründe stok var, silinemez");

        urun.Silindi = true;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Ürün silindi");
    }

    private async Task<string> GenerateUrunKod()
    {
        var count = await _unitOfWork.Repository<Urun>().Query().CountAsync();
        return $"URN-{(count + 1):D6}";
    }

    private static UrunDto MapToDto(Urun u) => new()
    {
        Id = u.Id,
        Ad = u.Ad,
        Kod = u.Kod,
        Barkod = u.Barkod,
        KategoriId = u.KategoriId,
        KategoriAd = u.Kategori?.Ad ?? "",
        KritikStok = u.KritikStok,
        SeriTakipli = u.SeriTakipli,
        AlisFiyat = u.AlisFiyat,
        SatisFiyat = u.SatisFiyat,
        KdvOran = u.KdvOran,
        Aktif = u.Aktif
    };
}
