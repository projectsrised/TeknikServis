using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;

namespace TeknikServisApp.Application.Services;

public class MarkaService : IMarkaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public MarkaService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<List<MarkaDto>> GetAllAsync()
    {
        var markalar = await _unitOfWork.Repository<Marka>()
            .Query()
            .Include(m => m.Modeller.Where(mo => !mo.Silindi))
            .Where(m => !m.Silindi)
            .OrderBy(m => m.Sira)
            .ThenBy(m => m.Ad)
            .ToListAsync();

        return markalar.Select(MapToDto).ToList();
    }

    public async Task<List<MarkaModelDto>> GetAllWithModelsAsync()
    {
        var markalar = await _unitOfWork.Repository<Marka>()
            .Query()
            .Include(m => m.Modeller.Where(mo => !mo.Silindi && mo.Aktif))
            .Where(m => !m.Silindi && m.Aktif)
            .OrderBy(m => m.Sira)
            .ThenBy(m => m.Ad)
            .ToListAsync();

        return markalar.Select(m => new MarkaModelDto
        {
            Id = m.Id,
            Ad = m.Ad,
            Modeller = m.Modeller.OrderBy(mo => mo.Sira).ThenBy(mo => mo.Ad).Select(mo => new ModelDto
            {
                Id = mo.Id,
                MarkaId = mo.MarkaId,
                MarkaAd = m.Ad,
                Ad = mo.Ad,
                Kod = mo.Kod,
                Sira = mo.Sira,
                Aktif = mo.Aktif
            }).ToList()
        }).ToList();
    }

    public async Task<MarkaDto?> GetByIdAsync(Guid id)
    {
        var marka = await _unitOfWork.Repository<Marka>()
            .Query()
            .Include(m => m.Modeller.Where(mo => !mo.Silindi))
            .FirstOrDefaultAsync(m => m.Id == id && !m.Silindi);

        return marka == null ? null : MapToDto(marka);
    }

    public async Task<ApiResponseDto<MarkaDto>> CreateAsync(MarkaCreateDto dto)
    {
        var exists = await _unitOfWork.Repository<Marka>()
            .AnyAsync(m => m.Ad == dto.Ad && !m.Silindi);

        if (exists)
            return ApiResponseDto<MarkaDto>.Fail("Bu marka zaten mevcut");

        var marka = new Marka
        {
            TenantId = _tenantProvider.TenantId,
            Ad = dto.Ad,
            Logo = dto.Logo,
            Sira = dto.Sira
        };

        await _unitOfWork.Repository<Marka>().AddAsync(marka);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<MarkaDto>.Ok(MapToDto(marka), "Marka oluşturuldu");
    }

    public async Task<ApiResponseDto<MarkaDto>> UpdateAsync(Guid id, MarkaUpdateDto dto)
    {
        var marka = await _unitOfWork.Repository<Marka>().GetByIdAsync(id);
        if (marka == null)
            return ApiResponseDto<MarkaDto>.Fail("Marka bulunamadı");

        if (dto.Ad != null) marka.Ad = dto.Ad;
        if (dto.Logo != null) marka.Logo = dto.Logo;
        if (dto.Sira.HasValue) marka.Sira = dto.Sira.Value;
        if (dto.Aktif.HasValue) marka.Aktif = dto.Aktif.Value;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<MarkaDto>.Ok(MapToDto(marka), "Marka güncellendi");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        var marka = await _unitOfWork.Repository<Marka>().GetByIdAsync(id);
        if (marka == null)
            return ApiResponseDto<bool>.Fail("Marka bulunamadı");

        var hasModels = await _unitOfWork.Repository<Model>()
            .AnyAsync(m => m.MarkaId == id && !m.Silindi);

        if (hasModels)
            return ApiResponseDto<bool>.Fail("Bu markaya ait modeller var, önce modelleri silin");

        marka.Silindi = true;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Marka silindi");
    }

    private static MarkaDto MapToDto(Marka m) => new()
    {
        Id = m.Id,
        Ad = m.Ad,
        Logo = m.Logo,
        Sira = m.Sira,
        Aktif = m.Aktif,
        ModelSayisi = m.Modeller?.Count(mo => !mo.Silindi) ?? 0
    };
}

public class ModelService : IModelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public ModelService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<List<ModelDto>> GetAllAsync()
    {
        var modeller = await _unitOfWork.Repository<Model>()
            .Query()
            .Include(m => m.Marka)
            .Where(m => !m.Silindi)
            .OrderBy(m => m.Marka.Ad)
            .ThenBy(m => m.Sira)
            .ThenBy(m => m.Ad)
            .ToListAsync();

        return modeller.Select(MapToDto).ToList();
    }

    public async Task<List<ModelDto>> GetByMarkaIdAsync(Guid markaId)
    {
        var modeller = await _unitOfWork.Repository<Model>()
            .Query()
            .Include(m => m.Marka)
            .Where(m => m.MarkaId == markaId && !m.Silindi && m.Aktif)
            .OrderBy(m => m.Sira)
            .ThenBy(m => m.Ad)
            .ToListAsync();

        return modeller.Select(MapToDto).ToList();
    }

    public async Task<ModelDto?> GetByIdAsync(Guid id)
    {
        var model = await _unitOfWork.Repository<Model>()
            .Query()
            .Include(m => m.Marka)
            .FirstOrDefaultAsync(m => m.Id == id && !m.Silindi);

        return model == null ? null : MapToDto(model);
    }

    public async Task<ApiResponseDto<ModelDto>> CreateAsync(ModelCreateDto dto)
    {
        var markaExists = await _unitOfWork.Repository<Marka>()
            .AnyAsync(m => m.Id == dto.MarkaId && !m.Silindi);

        if (!markaExists)
            return ApiResponseDto<ModelDto>.Fail("Marka bulunamadı");

        var exists = await _unitOfWork.Repository<Model>()
            .AnyAsync(m => m.MarkaId == dto.MarkaId && m.Ad == dto.Ad && !m.Silindi);

        if (exists)
            return ApiResponseDto<ModelDto>.Fail("Bu marka için bu model zaten mevcut");

        var model = new Model
        {
            TenantId = _tenantProvider.TenantId,
            MarkaId = dto.MarkaId,
            Ad = dto.Ad,
            Kod = dto.Kod,
            Sira = dto.Sira
        };

        await _unitOfWork.Repository<Model>().AddAsync(model);
        await _unitOfWork.SaveChangesAsync();

        // Load marka for response
        var createdModel = await _unitOfWork.Repository<Model>()
            .Query()
            .Include(m => m.Marka)
            .FirstOrDefaultAsync(m => m.Id == model.Id);

        return ApiResponseDto<ModelDto>.Ok(MapToDto(createdModel!), "Model oluşturuldu");
    }

    public async Task<ApiResponseDto<ModelDto>> UpdateAsync(Guid id, ModelUpdateDto dto)
    {
        var model = await _unitOfWork.Repository<Model>()
            .Query()
            .Include(m => m.Marka)
            .FirstOrDefaultAsync(m => m.Id == id && !m.Silindi);

        if (model == null)
            return ApiResponseDto<ModelDto>.Fail("Model bulunamadı");

        if (dto.Ad != null) model.Ad = dto.Ad;
        if (dto.Kod != null) model.Kod = dto.Kod;
        if (dto.Sira.HasValue) model.Sira = dto.Sira.Value;
        if (dto.Aktif.HasValue) model.Aktif = dto.Aktif.Value;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<ModelDto>.Ok(MapToDto(model), "Model güncellendi");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        var model = await _unitOfWork.Repository<Model>().GetByIdAsync(id);
        if (model == null)
            return ApiResponseDto<bool>.Fail("Model bulunamadı");

        var hasProducts = await _unitOfWork.Repository<Urun>()
            .AnyAsync(u => u.ModelId == id && !u.Silindi);

        if (hasProducts)
            return ApiResponseDto<bool>.Fail("Bu modele ait ürünler var, silinemez");

        model.Silindi = true;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Model silindi");
    }

    private static ModelDto MapToDto(Model m) => new()
    {
        Id = m.Id,
        MarkaId = m.MarkaId,
        MarkaAd = m.Marka?.Ad,
        Ad = m.Ad,
        Kod = m.Kod,
        Sira = m.Sira,
        Aktif = m.Aktif
    };
}
