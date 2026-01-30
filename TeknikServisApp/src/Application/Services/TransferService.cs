using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;
using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.Services;

public class TransferService : ITransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public TransferService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<TransferDto?> GetByIdAsync(Guid id)
    {
        var transfer = await _unitOfWork.Repository<Transfer>()
            .Query()
            .Include(t => t.KaynakDepo)
            .Include(t => t.HedefDepo)
            .Include(t => t.Olusturan)
            .Include(t => t.Kalemler)
                .ThenInclude(k => k.Urun)
            .Include(t => t.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
            .FirstOrDefaultAsync(t => t.Id == id && !t.Silindi);

        return transfer == null ? null : MapToDto(transfer);
    }

    public async Task<TransferDto?> GetByTransferNoAsync(string transferNo)
    {
        var transfer = await _unitOfWork.Repository<Transfer>()
            .Query()
            .Include(t => t.KaynakDepo)
            .Include(t => t.HedefDepo)
            .Include(t => t.Olusturan)
            .FirstOrDefaultAsync(t => t.TransferNo == transferNo && !t.Silindi);

        return transfer == null ? null : MapToDto(transfer);
    }

    public async Task<PagedResultDto<TransferDto>> GetListAsync(int page, int pageSize, Guid? kaynakDepoId = null, Guid? hedefDepoId = null, string? durum = null)
    {
        var query = _unitOfWork.Repository<Transfer>()
            .Query()
            .Include(t => t.KaynakDepo)
            .Include(t => t.HedefDepo)
            .Include(t => t.Olusturan)
            .Where(t => !t.Silindi);

        if (kaynakDepoId.HasValue)
            query = query.Where(t => t.KaynakDepoId == kaynakDepoId.Value);

        if (hedefDepoId.HasValue)
            query = query.Where(t => t.HedefDepoId == hedefDepoId.Value);

        if (!string.IsNullOrEmpty(durum) && Enum.TryParse<TransferDurum>(durum, out var durumEnum))
            query = query.Where(t => t.Durum == durumEnum);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.TransferTarihi)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<TransferDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ApiResponseDto<TransferDto>> CreateAsync(TransferCreateDto dto)
    {
        if (!dto.Kalemler.Any())
            return ApiResponseDto<TransferDto>.Fail("En az bir ürün eklemelisiniz");

        var transferNo = await GenerateTransferNoAsync();

        var transfer = new Transfer
        {
            TenantId = _tenantProvider.TenantId,
            TransferNo = transferNo,
            KaynakDepoId = dto.KaynakDepoId,
            HedefDepoId = dto.HedefDepoId,
            OlusturanId = _tenantProvider.KullaniciId ?? Guid.Empty,
            Aciklama = dto.Aciklama,
            Durum = TransferDurum.Beklemede
        };

        foreach (var kalemDto in dto.Kalemler)
        {
            var seriNumarasi = await _unitOfWork.Repository<SeriNumarasi>()
                .Query()
                .Include(s => s.Urun)
                .FirstOrDefaultAsync(s => s.SeriNo == kalemDto.SeriNo && s.DepoId == dto.KaynakDepoId && !s.Satildi && !s.Silindi);

            if (seriNumarasi == null)
                return ApiResponseDto<TransferDto>.Fail($"Seri numarası kaynak depoda bulunamadı: {kalemDto.SeriNo}");

            var kalem = new TransferKalem
            {
                TransferId = transfer.Id,
                UrunId = seriNumarasi.UrunId,
                SeriNumarasiId = seriNumarasi.Id
            };

            transfer.Kalemler.Add(kalem);
        }

        await _unitOfWork.Repository<Transfer>().AddAsync(transfer);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TransferDto>.Ok(await GetByIdAsync(transfer.Id) ?? MapToDto(transfer), "Transfer oluşturuldu");
    }

    public async Task<ApiResponseDto<TransferDto>> OnaylaAsync(Guid id)
    {
        var transfer = await _unitOfWork.Repository<Transfer>()
            .Query()
            .Include(t => t.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
            .FirstOrDefaultAsync(t => t.Id == id && !t.Silindi);

        if (transfer == null)
            return ApiResponseDto<TransferDto>.Fail("Transfer bulunamadı");

        if (transfer.Durum != TransferDurum.Beklemede)
            return ApiResponseDto<TransferDto>.Fail("Sadece bekleyen transferler onaylanabilir");

        transfer.Durum = TransferDurum.Onaylandi;
        transfer.OnayTarihi = DateTime.UtcNow;

        // Seri numaralarını kaynak depodan çıkar
        foreach (var kalem in transfer.Kalemler)
        {
            if (kalem.SeriNumarasi != null)
                kalem.SeriNumarasi.DepoId = null;
        }

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TransferDto>.Ok(await GetByIdAsync(id) ?? MapToDto(transfer), "Transfer onaylandı");
    }

    public async Task<ApiResponseDto<TransferDto>> TamamlaAsync(Guid id)
    {
        var transfer = await _unitOfWork.Repository<Transfer>()
            .Query()
            .Include(t => t.Kalemler)
            .FirstOrDefaultAsync(t => t.Id == id && !t.Silindi);

        if (transfer == null)
            return ApiResponseDto<TransferDto>.Fail("Transfer bulunamadı");

        if (transfer.Kalemler.Any(k => !k.TeslimAlindi))
            return ApiResponseDto<TransferDto>.Fail("Tüm kalemler teslim alınmadan transfer tamamlanamaz");

        transfer.Durum = TransferDurum.Tamamlandi;
        transfer.TamamlanmaTarihi = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TransferDto>.Ok(await GetByIdAsync(id) ?? MapToDto(transfer), "Transfer tamamlandı");
    }

    public async Task<ApiResponseDto<TransferKalemDto>> KalemTeslimAlAsync(Guid transferId, Guid kalemId)
    {
        var transfer = await _unitOfWork.Repository<Transfer>()
            .Query()
            .Include(t => t.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
            .Include(t => t.Kalemler)
                .ThenInclude(k => k.Urun)
            .FirstOrDefaultAsync(t => t.Id == transferId && !t.Silindi);

        if (transfer == null)
            return ApiResponseDto<TransferKalemDto>.Fail("Transfer bulunamadı");

        var kalem = transfer.Kalemler.FirstOrDefault(k => k.Id == kalemId);
        if (kalem == null)
            return ApiResponseDto<TransferKalemDto>.Fail("Kalem bulunamadı");

        if (kalem.TeslimAlindi)
            return ApiResponseDto<TransferKalemDto>.Fail("Kalem zaten teslim alınmış");

        kalem.TeslimAlindi = true;
        kalem.TeslimTarihi = DateTime.UtcNow;

        if (kalem.SeriNumarasi != null)
            kalem.SeriNumarasi.DepoId = transfer.HedefDepoId;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TransferKalemDto>.Ok(new TransferKalemDto
        {
            Id = kalem.Id,
            UrunId = kalem.UrunId,
            UrunAd = kalem.Urun?.Ad ?? "",
            SeriNo = kalem.SeriNumarasi?.SeriNo ?? "",
            TeslimAlindi = kalem.TeslimAlindi,
            TeslimTarihi = kalem.TeslimTarihi
        }, "Kalem teslim alındı");
    }

    public async Task<ApiResponseDto<TransferDto>> IptalAsync(Guid id, string? neden = null)
    {
        var transfer = await _unitOfWork.Repository<Transfer>()
            .Query()
            .Include(t => t.Kalemler)
                .ThenInclude(k => k.SeriNumarasi)
            .FirstOrDefaultAsync(t => t.Id == id && !t.Silindi);

        if (transfer == null)
            return ApiResponseDto<TransferDto>.Fail("Transfer bulunamadı");

        // Seri numaralarını kaynak depoya geri koy
        foreach (var kalem in transfer.Kalemler)
        {
            if (kalem.SeriNumarasi != null && kalem.SeriNumarasi.DepoId == null)
                kalem.SeriNumarasi.DepoId = transfer.KaynakDepoId;
        }

        transfer.Durum = TransferDurum.IptalEdildi;
        transfer.IptalNedeni = neden;
        transfer.GuncellemeTarihi = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<TransferDto>.Ok(await GetByIdAsync(id) ?? MapToDto(transfer), "Transfer iptal edildi");
    }

    private async Task<string> GenerateTransferNoAsync()
    {
        var today = DateTime.UtcNow;
        var prefix = $"TRF-{today:yyyyMMdd}-";
        
        var last = await _unitOfWork.Repository<Transfer>()
            .Query()
            .Where(t => t.TransferNo.StartsWith(prefix))
            .OrderByDescending(t => t.TransferNo)
            .FirstOrDefaultAsync();

        var sequence = 1;
        if (last != null)
        {
            var lastNumber = last.TransferNo.Split('-').Last();
            if (int.TryParse(lastNumber, out var num))
                sequence = num + 1;
        }

        return $"{prefix}{sequence:D4}";
    }

    private static TransferDto MapToDto(Transfer t) => new()
    {
        Id = t.Id,
        TransferNo = t.TransferNo,
        KaynakDepoId = t.KaynakDepoId,
        KaynakDepoAd = t.KaynakDepo?.Ad ?? "",
        HedefDepoId = t.HedefDepoId,
        HedefDepoAd = t.HedefDepo?.Ad ?? "",
        OlusturanAd = t.Olusturan?.AdSoyad ?? "",
        Durum = t.Durum,
        DurumAd = t.Durum.ToString(),
        TransferTarihi = t.TransferTarihi,
        TamamlanmaTarihi = t.TamamlanmaTarihi,
        Aciklama = t.Aciklama,
        Kalemler = t.Kalemler?.Select(k => new TransferKalemDto
        {
            Id = k.Id,
            UrunId = k.UrunId,
            UrunAd = k.Urun?.Ad ?? "",
            SeriNo = k.SeriNumarasi?.SeriNo ?? "",
            TeslimAlindi = k.TeslimAlindi,
            TeslimTarihi = k.TeslimTarihi
        }).ToList() ?? new()
    };
}
