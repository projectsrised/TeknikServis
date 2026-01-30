using Microsoft.EntityFrameworkCore;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;
using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.Services;

public class KasaService : IKasaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public KasaService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<KasaDto?> GetByIdAsync(Guid id)
    {
        var kasa = await _unitOfWork.Repository<Kasa>()
            .Query()
            .Include(k => k.Bayi)
            .FirstOrDefaultAsync(k => k.Id == id && !k.Silindi);

        return kasa == null ? null : MapToDto(kasa);
    }

    public async Task<KasaDto?> GetByBayiIdAsync(Guid bayiId)
    {
        var kasa = await _unitOfWork.Repository<Kasa>()
            .Query()
            .Include(k => k.Bayi)
            .FirstOrDefaultAsync(k => k.BayiId == bayiId && !k.Silindi);

        return kasa == null ? null : MapToDto(kasa);
    }

    public async Task<PagedResultDto<KasaHareketDto>> GetHareketlerAsync(Guid kasaId, int page, int pageSize, DateTime? baslangic = null, DateTime? bitis = null)
    {
        var query = _unitOfWork.Repository<KasaHareket>()
            .Query()
            .Include(h => h.Kasa)
            .Where(h => h.KasaId == kasaId && !h.Silindi);

        if (baslangic.HasValue) query = query.Where(h => h.IslemTarihi >= baslangic.Value);
        if (bitis.HasValue) query = query.Where(h => h.IslemTarihi <= bitis.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(h => h.IslemTarihi).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResultDto<KasaHareketDto>
        {
            Items = items.Select(h => new KasaHareketDto
            {
                Id = h.Id, KasaId = h.KasaId, KasaAd = h.Kasa?.Ad ?? "", Tip = h.Tip, TipAd = h.Tip.ToString(),
                Tutar = h.Tutar, OncekiBakiye = h.OncekiBakiye, SonrakiBakiye = h.SonrakiBakiye,
                IslemTarihi = h.IslemTarihi, BelgeNo = h.BelgeNo, Aciklama = h.Aciklama
            }).ToList(),
            TotalCount = total, Page = page, PageSize = pageSize
        };
    }

    public async Task<ApiResponseDto<KasaHareketDto>> HareketEkleAsync(KasaHareketEkleDto dto)
    {
        var kasa = await _unitOfWork.Repository<Kasa>().GetByIdAsync(dto.KasaId);
        if (kasa == null) return ApiResponseDto<KasaHareketDto>.Fail("Kasa bulunamadı");

        var oncekiBakiye = kasa.Bakiye;
        var tutar = dto.Tip == KasaHareketTip.Cikis || dto.Tip == KasaHareketTip.MaasOdeme || 
                    dto.Tip == KasaHareketTip.AvansOdeme || dto.Tip == KasaHareketTip.Gider 
                    ? -dto.Tutar : dto.Tutar;

        kasa.Bakiye += tutar;

        var hareket = new KasaHareket
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = kasa.BayiId,
            KasaId = dto.KasaId,
            Tip = dto.Tip,
            Tutar = dto.Tutar,
            OncekiBakiye = oncekiBakiye,
            SonrakiBakiye = kasa.Bakiye,
            BelgeNo = dto.BelgeNo,
            Aciklama = dto.Aciklama,
            OlusturanId = _tenantProvider.KullaniciId
        };

        await _unitOfWork.Repository<KasaHareket>().AddAsync(hareket);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<KasaHareketDto>.Ok(new KasaHareketDto
        {
            Id = hareket.Id, KasaId = hareket.KasaId, Tip = hareket.Tip, TipAd = hareket.Tip.ToString(),
            Tutar = hareket.Tutar, OncekiBakiye = hareket.OncekiBakiye, SonrakiBakiye = hareket.SonrakiBakiye,
            IslemTarihi = hareket.IslemTarihi, BelgeNo = hareket.BelgeNo, Aciklama = hareket.Aciklama
        }, "Kasa hareketi eklendi");
    }

    public async Task<decimal> GetGunlukCiroAsync(Guid bayiId, DateTime? tarih = null)
    {
        var date = tarih ?? DateTime.UtcNow.Date;
        var nextDate = date.AddDays(1);

        return await _unitOfWork.Repository<KasaHareket>()
            .Query()
            .Where(h => h.BayiId == bayiId && h.IslemTarihi >= date && h.IslemTarihi < nextDate &&
                        (h.Tip == KasaHareketTip.NakitSatis || h.Tip == KasaHareketTip.KartSatis) && !h.Silindi)
            .SumAsync(h => h.Tutar);
    }

    public async Task<ApiResponseDto<KasaDto>> CreateAsync(KasaCreateDto dto)
    {
        var kasa = new Kasa
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = dto.BayiId,
            Ad = dto.Ad ?? "Ana Kasa",
            Bakiye = dto.AcilisBakiyesi
        };

        await _unitOfWork.Repository<Kasa>().AddAsync(kasa);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<KasaDto>.Ok(MapToDto(kasa), "Kasa oluşturuldu");
    }

    private static KasaDto MapToDto(Kasa k) => new()
    {
        Id = k.Id, BayiId = k.BayiId, BayiAd = k.Bayi?.Ad ?? "", Ad = k.Ad, Bakiye = k.Bakiye, Aktif = k.Aktif
    };
}

public class PersonelService : IPersonelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;

    public PersonelService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
    }

    public async Task<PersonelDto?> GetByIdAsync(Guid id)
    {
        var personel = await _unitOfWork.Repository<Kullanici>()
            .Query()
            .Include(k => k.Bayi)
            .FirstOrDefaultAsync(k => k.Id == id && !k.Silindi);

        return personel == null ? null : MapToDto(personel);
    }

    public async Task<PagedResultDto<PersonelDto>> GetListAsync(int page, int pageSize, Guid? bayiId = null, bool? aktif = null)
    {
        var query = _unitOfWork.Repository<Kullanici>()
            .Query()
            .Include(k => k.Bayi)
            .Where(k => !k.Silindi && k.BayiId.HasValue);

        if (bayiId.HasValue) query = query.Where(k => k.BayiId == bayiId.Value);
        if (aktif.HasValue) query = query.Where(k => k.Aktif == aktif.Value);

        var total = await query.CountAsync();
        var items = await query.OrderBy(k => k.Ad).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResultDto<PersonelDto> { Items = items.Select(MapToDto).ToList(), TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<ApiResponseDto<PersonelDto>> CreateAsync(PersonelCreateDto dto)
    {
        var exists = await _unitOfWork.Repository<Kullanici>().AnyAsync(k => k.Email == dto.Email && !k.Silindi);
        if (exists) return ApiResponseDto<PersonelDto>.Fail("Bu email zaten kullanılıyor");

        var personel = new Kullanici
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = dto.BayiId,
            Ad = dto.Ad,
            Soyad = dto.Soyad,
            Email = dto.Email,
            SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre),
            Telefon = dto.Telefon,
            Rol = dto.Rol,
            Maas = dto.Maas,
            YillikIzinHakki = dto.YillikIzinHakki ?? 14,
            IseBaslamaTarihi = dto.IseBaslamaTarihi ?? DateTime.UtcNow
        };

        await _unitOfWork.Repository<Kullanici>().AddAsync(personel);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelDto>.Ok(MapToDto(personel), "Personel oluşturuldu");
    }

    public async Task<ApiResponseDto<PersonelDto>> UpdateAsync(Guid id, PersonelUpdateDto dto)
    {
        var personel = await _unitOfWork.Repository<Kullanici>().GetByIdAsync(id);
        if (personel == null) return ApiResponseDto<PersonelDto>.Fail("Personel bulunamadı");

        if (dto.Ad != null) personel.Ad = dto.Ad;
        if (dto.Soyad != null) personel.Soyad = dto.Soyad;
        if (dto.Telefon != null) personel.Telefon = dto.Telefon;
        if (dto.Email != null) personel.Email = dto.Email;
        if (dto.Rol.HasValue) personel.Rol = dto.Rol.Value;
        if (dto.Maas.HasValue) personel.Maas = dto.Maas.Value;
        if (dto.YillikIzinHakki.HasValue) personel.YillikIzinHakki = dto.YillikIzinHakki.Value;
        if (dto.Aktif.HasValue) personel.Aktif = dto.Aktif.Value;

        personel.GuncellemeTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelDto>.Ok(MapToDto(personel), "Personel güncellendi");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        var personel = await _unitOfWork.Repository<Kullanici>().GetByIdAsync(id);
        if (personel == null) return ApiResponseDto<bool>.Fail("Personel bulunamadı");

        personel.Silindi = true;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Personel silindi");
    }

    public async Task<MaasHesaplamaDto> MaasHesaplaAsync(Guid personelId, int yil, int ay)
    {
        var personel = await _unitOfWork.Repository<Kullanici>().GetByIdAsync(personelId);
        if (personel == null) return new MaasHesaplamaDto();

        var mesailer = await _unitOfWork.Repository<PersonelMesai>()
            .Query()
            .Where(m => m.PersonelId == personelId && m.Tarih.Year == yil && m.Tarih.Month == ay && m.OnaylandiMi && !m.Silindi)
            .ToListAsync();

        var avanslar = await _unitOfWork.Repository<PersonelAvans>()
            .Query()
            .Where(a => a.PersonelId == personelId && a.OnaylandiMi && !a.MaastanKesildiMi && !a.Silindi)
            .ToListAsync();

        var brutMaas = personel.Maas ?? 0;
        var ekMesaiSaati = mesailer.Sum(m => m.ToplamSaat);
        var ekMesaiUcreti = mesailer.Sum(m => m.ToplamUcret);
        var avansKesinti = avanslar.Sum(a => a.Tutar);
        var netMaas = brutMaas * 0.85m; // Basit hesaplama

        return new MaasHesaplamaDto
        {
            PersonelId = personelId,
            PersonelAdSoyad = personel.AdSoyad,
            Yil = yil, Ay = ay,
            BrutMaas = brutMaas,
            NetMaas = netMaas,
            EkMesaiSaati = ekMesaiSaati,
            EkMesaiUcreti = ekMesaiUcreti,
            AvansKesinti = avansKesinti,
            ToplamOdeme = netMaas + ekMesaiUcreti - avansKesinti
        };
    }

    public async Task<ApiResponseDto<PersonelMaasOdemeDto>> MaasOdeAsync(Guid personelId, int yil, int ay)
    {
        var hesaplama = await MaasHesaplaAsync(personelId, yil, ay);

        var odeme = new PersonelMaasOdeme
        {
            TenantId = _tenantProvider.TenantId,
            PersonelId = personelId,
            Yil = yil, Ay = ay,
            BrutMaas = hesaplama.BrutMaas,
            NetMaas = hesaplama.NetMaas,
            EkMesaiUcreti = hesaplama.EkMesaiUcreti,
            AvansKesinti = hesaplama.AvansKesinti,
            ToplamOdeme = hesaplama.ToplamOdeme,
            OdendiMi = true,
            OdemeTarihi = DateTime.UtcNow
        };

        await _unitOfWork.Repository<PersonelMaasOdeme>().AddAsync(odeme);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelMaasOdemeDto>.Ok(new PersonelMaasOdemeDto
        {
            Id = odeme.Id, PersonelId = odeme.PersonelId, Yil = odeme.Yil, Ay = odeme.Ay,
            BrutMaas = odeme.BrutMaas, NetMaas = odeme.NetMaas, ToplamOdeme = odeme.ToplamOdeme,
            OdendiMi = odeme.OdendiMi, OdemeTarihi = odeme.OdemeTarihi
        }, "Maaş ödendi");
    }

    public async Task<List<PersonelMaasOdemeDto>> GetMaasOdemeleriAsync(Guid personelId)
    {
        var odemeler = await _unitOfWork.Repository<PersonelMaasOdeme>()
            .Query()
            .Include(o => o.Personel)
            .Where(o => o.PersonelId == personelId && !o.Silindi)
            .OrderByDescending(o => o.Yil).ThenByDescending(o => o.Ay)
            .ToListAsync();

        return odemeler.Select(o => new PersonelMaasOdemeDto
        {
            Id = o.Id, PersonelId = o.PersonelId, PersonelAd = o.Personel?.AdSoyad ?? "",
            Yil = o.Yil, Ay = o.Ay, BrutMaas = o.BrutMaas, NetMaas = o.NetMaas,
            EkMesaiUcreti = o.EkMesaiUcreti, Kesinti = o.Kesinti, AvansKesinti = o.AvansKesinti,
            ToplamOdeme = o.ToplamOdeme, OdendiMi = o.OdendiMi, OdemeTarihi = o.OdemeTarihi
        }).ToList();
    }

    public async Task<ApiResponseDto<PersonelAvansDto>> AvansTalepEtAsync(Guid personelId, decimal tutar)
    {
        var avans = new PersonelAvans
        {
            TenantId = _tenantProvider.TenantId,
            PersonelId = personelId,
            Tutar = tutar
        };

        await _unitOfWork.Repository<PersonelAvans>().AddAsync(avans);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelAvansDto>.Ok(new PersonelAvansDto
        {
            Id = avans.Id, PersonelId = avans.PersonelId, Tutar = avans.Tutar, TalepTarihi = avans.TalepTarihi
        }, "Avans talebi oluşturuldu");
    }

    public async Task<ApiResponseDto<PersonelAvansDto>> AvansOnaylaAsync(Guid avansId, bool onayla)
    {
        var avans = await _unitOfWork.Repository<PersonelAvans>().GetByIdAsync(avansId);
        if (avans == null) return ApiResponseDto<PersonelAvansDto>.Fail("Avans bulunamadı");

        avans.OnaylandiMi = onayla;
        avans.OnayTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelAvansDto>.Ok(new PersonelAvansDto
        {
            Id = avans.Id, PersonelId = avans.PersonelId, Tutar = avans.Tutar,
            OnaylandiMi = avans.OnaylandiMi, OdendiMi = avans.OdendiMi
        }, onayla ? "Avans onaylandı" : "Avans reddedildi");
    }

    public async Task<List<PersonelAvansDto>> GetAvanslarAsync(Guid personelId)
    {
        var avanslar = await _unitOfWork.Repository<PersonelAvans>()
            .Query()
            .Include(a => a.Personel)
            .Where(a => a.PersonelId == personelId && !a.Silindi)
            .OrderByDescending(a => a.TalepTarihi)
            .ToListAsync();

        return avanslar.Select(a => new PersonelAvansDto
        {
            Id = a.Id, PersonelId = a.PersonelId, PersonelAd = a.Personel?.AdSoyad ?? "",
            Tutar = a.Tutar, TalepTarihi = a.TalepTarihi, OdemeTarihi = a.OdemeTarihi,
            OnaylandiMi = a.OnaylandiMi, OdendiMi = a.OdendiMi, MaastanKesildiMi = a.MaastanKesildiMi
        }).ToList();
    }

    public async Task<ApiResponseDto<PersonelIzinDto>> IzinTalepEtAsync(PersonelIzinCreateDto dto)
    {
        var gunSayisi = (dto.BitisTarihi - dto.BaslangicTarihi).Days + 1;

        var izin = new PersonelIzin
        {
            TenantId = _tenantProvider.TenantId,
            PersonelId = dto.PersonelId,
            IzinTipi = dto.IzinTipi,
            BaslangicTarihi = dto.BaslangicTarihi,
            BitisTarihi = dto.BitisTarihi,
            GunSayisi = gunSayisi,
            Aciklama = dto.Aciklama
        };

        await _unitOfWork.Repository<PersonelIzin>().AddAsync(izin);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelIzinDto>.Ok(new PersonelIzinDto
        {
            Id = izin.Id, PersonelId = izin.PersonelId, IzinTipi = izin.IzinTipi, IzinTipiAd = izin.IzinTipi.ToString(),
            BaslangicTarihi = izin.BaslangicTarihi, BitisTarihi = izin.BitisTarihi, GunSayisi = izin.GunSayisi,
            Durum = izin.Durum, DurumAd = izin.Durum.ToString()
        }, "İzin talebi oluşturuldu");
    }

    public async Task<ApiResponseDto<PersonelIzinDto>> IzinOnaylaAsync(Guid izinId, bool onayla, string? neden = null)
    {
        var izin = await _unitOfWork.Repository<PersonelIzin>().GetByIdAsync(izinId);
        if (izin == null) return ApiResponseDto<PersonelIzinDto>.Fail("İzin bulunamadı");

        izin.Durum = onayla ? IzinDurum.Onaylandi : IzinDurum.Reddedildi;
        izin.OnayTarihi = DateTime.UtcNow;
        if (!onayla) izin.RedNedeni = neden;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelIzinDto>.Ok(new PersonelIzinDto
        {
            Id = izin.Id, PersonelId = izin.PersonelId, IzinTipi = izin.IzinTipi, IzinTipiAd = izin.IzinTipi.ToString(),
            BaslangicTarihi = izin.BaslangicTarihi, BitisTarihi = izin.BitisTarihi, GunSayisi = izin.GunSayisi,
            Durum = izin.Durum, DurumAd = izin.Durum.ToString()
        }, onayla ? "İzin onaylandı" : "İzin reddedildi");
    }

    public async Task<List<PersonelIzinDto>> GetIzinlerAsync(Guid personelId)
    {
        var izinler = await _unitOfWork.Repository<PersonelIzin>()
            .Query()
            .Include(i => i.Personel)
            .Where(i => i.PersonelId == personelId && !i.Silindi)
            .OrderByDescending(i => i.BaslangicTarihi)
            .ToListAsync();

        return izinler.Select(i => new PersonelIzinDto
        {
            Id = i.Id, PersonelId = i.PersonelId, PersonelAd = i.Personel?.AdSoyad ?? "",
            IzinTipi = i.IzinTipi, IzinTipiAd = i.IzinTipi.ToString(),
            BaslangicTarihi = i.BaslangicTarihi, BitisTarihi = i.BitisTarihi, GunSayisi = i.GunSayisi,
            Durum = i.Durum, DurumAd = i.Durum.ToString(), UcretliMi = i.UcretliMi
        }).ToList();
    }

    public async Task<int> KalanIzinGunuAsync(Guid personelId, int yil)
    {
        var personel = await _unitOfWork.Repository<Kullanici>().GetByIdAsync(personelId);
        if (personel == null) return 0;

        var kullanilanIzin = await _unitOfWork.Repository<PersonelIzin>()
            .Query()
            .Where(i => i.PersonelId == personelId && i.BaslangicTarihi.Year == yil &&
                        i.Durum == IzinDurum.Onaylandi && i.IzinTipi == PersonelIzinTip.YillikIzin && !i.Silindi)
            .SumAsync(i => i.GunSayisi);

        return personel.YillikIzinHakki - kullanilanIzin;
    }

    public async Task<ApiResponseDto<PersonelMesaiDto>> MesaiKaydetAsync(PersonelMesaiCreateDto dto)
    {
        var toplamSaat = (decimal)(dto.BitisSaati - dto.BaslangicSaati).TotalHours;

        var mesai = new PersonelMesai
        {
            TenantId = _tenantProvider.TenantId,
            PersonelId = dto.PersonelId,
            Tarih = dto.Tarih,
            BaslangicSaati = dto.BaslangicSaati,
            BitisSaati = dto.BitisSaati,
            ToplamSaat = toplamSaat,
            SaatUcreti = dto.SaatUcreti,
            ToplamUcret = toplamSaat * dto.SaatUcreti,
            Aciklama = dto.Aciklama
        };

        await _unitOfWork.Repository<PersonelMesai>().AddAsync(mesai);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelMesaiDto>.Ok(new PersonelMesaiDto
        {
            Id = mesai.Id, PersonelId = mesai.PersonelId, Tarih = mesai.Tarih,
            BaslangicSaati = mesai.BaslangicSaati, BitisSaati = mesai.BitisSaati,
            ToplamSaat = mesai.ToplamSaat, SaatUcreti = mesai.SaatUcreti, ToplamUcret = mesai.ToplamUcret
        }, "Mesai kaydedildi");
    }

    public async Task<ApiResponseDto<PersonelMesaiDto>> MesaiOnaylaAsync(Guid mesaiId)
    {
        var mesai = await _unitOfWork.Repository<PersonelMesai>().GetByIdAsync(mesaiId);
        if (mesai == null) return ApiResponseDto<PersonelMesaiDto>.Fail("Mesai bulunamadı");

        mesai.OnaylandiMi = true;
        mesai.OnayTarihi = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<PersonelMesaiDto>.Ok(new PersonelMesaiDto
        {
            Id = mesai.Id, PersonelId = mesai.PersonelId, Tarih = mesai.Tarih,
            ToplamSaat = mesai.ToplamSaat, ToplamUcret = mesai.ToplamUcret, OnaylandiMi = mesai.OnaylandiMi
        }, "Mesai onaylandı");
    }

    public async Task<List<PersonelMesaiDto>> GetMesailerAsync(Guid personelId, int? yil = null, int? ay = null)
    {
        var query = _unitOfWork.Repository<PersonelMesai>()
            .Query()
            .Include(m => m.Personel)
            .Where(m => m.PersonelId == personelId && !m.Silindi);

        if (yil.HasValue) query = query.Where(m => m.Tarih.Year == yil.Value);
        if (ay.HasValue) query = query.Where(m => m.Tarih.Month == ay.Value);

        var mesailer = await query.OrderByDescending(m => m.Tarih).ToListAsync();

        return mesailer.Select(m => new PersonelMesaiDto
        {
            Id = m.Id, PersonelId = m.PersonelId, PersonelAd = m.Personel?.AdSoyad ?? "",
            Tarih = m.Tarih, BaslangicSaati = m.BaslangicSaati, BitisSaati = m.BitisSaati,
            ToplamSaat = m.ToplamSaat, SaatUcreti = m.SaatUcreti, ToplamUcret = m.ToplamUcret, OnaylandiMi = m.OnaylandiMi
        }).ToList();
    }

    private static PersonelDto MapToDto(Kullanici k) => new()
    {
        Id = k.Id, BayiId = k.BayiId, BayiAd = k.Bayi?.Ad, Ad = k.Ad, Soyad = k.Soyad,
        AdSoyad = k.AdSoyad, Email = k.Email, Telefon = k.Telefon, Rol = k.Rol, RolAd = k.Rol.ToString(),
        Maas = k.Maas, YillikIzinHakki = k.YillikIzinHakki, IseBaslamaTarihi = k.IseBaslamaTarihi, Aktif = k.Aktif
    };
}
