using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;
using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.Services;

public class TeknikServisService : ITeknikServisService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public TeknikServisService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<TeknikServisDto?> GetByIdAsync(Guid id)
    {
        var servis = await _unitOfWork.Repository<TeknikServisKayit>()
            .Query()
            .Include(t => t.Bayi)
            .Include(t => t.SorumluPersonel)
            .Include(t => t.Kalemler)
                .ThenInclude(k => k.Urun)
            .FirstOrDefaultAsync(t => t.Id == id && !t.Silindi);

        return servis == null ? null : MapToDto(servis);
    }

    public async Task<TeknikServisDto?> GetByServisNoAsync(string servisNo)
    {
        var servis = await _unitOfWork.Repository<TeknikServisKayit>()
            .Query()
            .Include(t => t.Bayi)
            .Include(t => t.SorumluPersonel)
            .FirstOrDefaultAsync(t => t.ServisNo == servisNo && !t.Silindi);

        return servis == null ? null : MapToDto(servis);
    }

    public async Task<List<TeknikServisDto>> GetByMusteriTelefonAsync(string telefon)
    {
        var servisler = await _unitOfWork.Repository<TeknikServisKayit>()
            .Query()
            .Include(t => t.Bayi)
            .Where(t => t.MusteriTelefon == telefon && !t.Silindi)
            .OrderByDescending(t => t.GirisTarihi)
            .ToListAsync();

        return servisler.Select(MapToDto).ToList();
    }

    public async Task<PagedResultDto<TeknikServisDto>> GetListAsync(int page, int pageSize, Guid? bayiId = null, string? durum = null)
    {
        var query = _unitOfWork.Repository<TeknikServisKayit>()
            .Query()
            .Include(t => t.Bayi)
            .Include(t => t.SorumluPersonel)
            .Where(t => !t.Silindi);

        if (bayiId.HasValue)
            query = query.Where(t => t.BayiId == bayiId.Value);

        if (!string.IsNullOrEmpty(durum) && Enum.TryParse<TeknikServisDurum>(durum, out var durumEnum))
            query = query.Where(t => t.Durum == durumEnum);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.GirisTarihi)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<TeknikServisDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponseDto<TeknikServisDto>> CreateAsync(TeknikServisCreateDto dto)
    {
        var servisNo = await GenerateServisNoAsync();

        var servis = new TeknikServisKayit
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = _tenantProvider.BayiId ?? Guid.Empty,
            ServisNo = servisNo,
            MusteriId = dto.MusteriId,
            MusteriAd = dto.MusteriAd,
            MusteriTelefon = dto.MusteriTelefon,
            SorumluPersonelId = dto.SorumluPersonelId,
            CihazTip = dto.CihazTip,
            CihazMarka = dto.CihazMarka,
            CihazModel = dto.CihazModel,
            SeriNo = dto.SeriNo,
            ImeiNo = dto.ImeiNo,
            CihazSifre = dto.CihazSifre,
            Ariza = dto.Ariza,
            ArizaDetay = dto.ArizaDetay,
            TahminiTeslimTarihi = dto.TahminiTeslimTarihi,
            IscilikUcreti = dto.IscilikUcreti,
            GarantiKapsami = dto.GarantiKapsami,
            GarantiNot = dto.GarantiNot,
            Durum = TeknikServisDurum.Beklemede
        };

        await _unitOfWork.Repository<TeknikServisKayit>().AddAsync(servis);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TeknikServisDto>.Ok(MapToDto(servis), "Teknik servis kaydı oluşturuldu");
    }

    public async Task<ApiResponseDto<TeknikServisDto>> DurumGuncelleAsync(Guid id, TeknikServisDurumGuncelleDto dto)
    {
        var servis = await _unitOfWork.Repository<TeknikServisKayit>().GetByIdAsync(id);
        if (servis == null)
            return ApiResponseDto<TeknikServisDto>.Fail("Servis kaydı bulunamadı");

        servis.Durum = dto.Durum;
        if (dto.TespitNotlari != null) servis.TespitNotlari = dto.TespitNotlari;
        if (dto.YapilanIslem != null) servis.YapilanIslem = dto.YapilanIslem;
        servis.GuncellemeTarihi = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TeknikServisDto>.Ok(await GetByIdAsync(id) ?? MapToDto(servis), "Durum güncellendi");
    }

    public async Task<ApiResponseDto<TeknikServisDto>> ParcaEkleAsync(Guid id, TeknikServisParcaEkleDto dto)
    {
        var servis = await _unitOfWork.Repository<TeknikServisKayit>()
            .Query()
            .Include(t => t.Kalemler)
            .FirstOrDefaultAsync(t => t.Id == id && !t.Silindi);

        if (servis == null)
            return ApiResponseDto<TeknikServisDto>.Fail("Servis kaydı bulunamadı");

        var kalem = new TeknikServisKalem
        {
            TeknikServisId = id,
            UrunId = dto.UrunId,
            ParcaAdi = dto.ParcaAdi,
            Miktar = dto.Miktar,
            BirimFiyat = dto.BirimFiyat,
            Toplam = dto.Miktar * dto.BirimFiyat
        };

        servis.Kalemler.Add(kalem);
        servis.ParcaToplam = servis.Kalemler.Sum(k => k.Toplam);
        servis.ToplamTutar = servis.IscilikUcreti + servis.ParcaToplam;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TeknikServisDto>.Ok(await GetByIdAsync(id) ?? MapToDto(servis), "Parça eklendi");
    }

    public async Task<ApiResponseDto<TeknikServisDto>> TeslimEtAsync(Guid id)
    {
        var servis = await _unitOfWork.Repository<TeknikServisKayit>().GetByIdAsync(id);
        if (servis == null)
            return ApiResponseDto<TeknikServisDto>.Fail("Servis kaydı bulunamadı");

        if (!servis.OdemeAlindi)
            return ApiResponseDto<TeknikServisDto>.Fail("Ödeme alınmadan teslim edilemez");

        servis.Durum = TeknikServisDurum.TeslimEdildi;
        servis.TeslimTarihi = DateTime.UtcNow;
        servis.GuncellemeTarihi = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TeknikServisDto>.Ok(await GetByIdAsync(id) ?? MapToDto(servis), "Cihaz teslim edildi");
    }

    public async Task<ApiResponseDto<TeknikServisDto>> OdemeAlAsync(Guid id, TeknikServisOdemeAlDto dto)
    {
        var servis = await _unitOfWork.Repository<TeknikServisKayit>().GetByIdAsync(id);
        if (servis == null)
            return ApiResponseDto<TeknikServisDto>.Fail("Servis kaydı bulunamadı");

        servis.OdenenTutar += dto.Tutar;
        if (servis.OdenenTutar >= servis.ToplamTutar)
        {
            servis.OdemeAlindi = true;
            servis.OdemeTarihi = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TeknikServisDto>.Ok(await GetByIdAsync(id) ?? MapToDto(servis), "Ödeme alındı");
    }

    public async Task<ApiResponseDto<TeknikServisDto>> IptalAsync(Guid id, string? neden = null)
    {
        var servis = await _unitOfWork.Repository<TeknikServisKayit>().GetByIdAsync(id);
        if (servis == null)
            return ApiResponseDto<TeknikServisDto>.Fail("Servis kaydı bulunamadı");

        servis.Durum = TeknikServisDurum.IptalEdildi;
        servis.IptalNedeni = neden;
        servis.GuncellemeTarihi = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TeknikServisDto>.Ok(await GetByIdAsync(id) ?? MapToDto(servis), "Servis iptal edildi");
    }

    private async Task<string> GenerateServisNoAsync()
    {
        var today = DateTime.UtcNow;
        var prefix = $"SRV-{today:yyyyMMdd}-";
        
        var last = await _unitOfWork.Repository<TeknikServisKayit>()
            .Query()
            .Where(t => t.ServisNo.StartsWith(prefix))
            .OrderByDescending(t => t.ServisNo)
            .FirstOrDefaultAsync();

        var sequence = 1;
        if (last != null)
        {
            var lastNumber = last.ServisNo.Split('-').Last();
            if (int.TryParse(lastNumber, out var num))
                sequence = num + 1;
        }

        return $"{prefix}{sequence:D4}";
    }

    private static TeknikServisDto MapToDto(TeknikServisKayit t) => new()
    {
        Id = t.Id,
        ServisNo = t.ServisNo,
        BayiId = t.BayiId,
        BayiAd = t.Bayi?.Ad ?? "",
        MusteriAd = t.MusteriAd,
        MusteriTelefon = t.MusteriTelefon,
        SorumluPersonelAd = t.SorumluPersonel?.AdSoyad,
        CihazTip = t.CihazTip,
        CihazMarka = t.CihazMarka,
        CihazModel = t.CihazModel,
        SeriNo = t.SeriNo,
        Ariza = t.Ariza,
        ArizaDetay = t.ArizaDetay,
        YapilanIslem = t.YapilanIslem,
        Durum = t.Durum,
        DurumAd = t.Durum.ToString(),
        GirisTarihi = t.GirisTarihi,
        TahminiTeslimTarihi = t.TahminiTeslimTarihi,
        TeslimTarihi = t.TeslimTarihi,
        IscilikUcreti = t.IscilikUcreti,
        ParcaToplam = t.ParcaToplam,
        ToplamTutar = t.ToplamTutar,
        OdenenTutar = t.OdenenTutar,
        OdemeAlindi = t.OdemeAlindi,
        GarantiKapsami = t.GarantiKapsami,
        Kalemler = t.Kalemler?.Select(k => new TeknikServisKalemDto
        {
            Id = k.Id,
            UrunAd = k.Urun?.Ad,
            ParcaAdi = k.ParcaAdi,
            Miktar = k.Miktar,
            BirimFiyat = k.BirimFiyat,
            Toplam = k.Toplam
        }).ToList() ?? new()
    };
}
