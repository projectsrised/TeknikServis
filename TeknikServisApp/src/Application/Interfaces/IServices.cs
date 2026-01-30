using TeknikServisApp.Application.DTOs;

namespace TeknikServisApp.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginDto request);
    Task<ApiResponseDto<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto request);
    Task<ApiResponseDto<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto request);
    Task<KullaniciDto?> GetCurrentUserAsync(Guid userId);
    Task<ApiResponseDto<KullaniciDto>> RegisterAsync(RegisterDto request);
}

public interface IMarkaService
{
    Task<List<MarkaDto>> GetAllAsync();
    Task<List<MarkaModelDto>> GetAllWithModelsAsync();
    Task<MarkaDto?> GetByIdAsync(Guid id);
    Task<ApiResponseDto<MarkaDto>> CreateAsync(MarkaCreateDto dto);
    Task<ApiResponseDto<MarkaDto>> UpdateAsync(Guid id, MarkaUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(Guid id);
}

public interface IModelService
{
    Task<List<ModelDto>> GetAllAsync();
    Task<List<ModelDto>> GetByMarkaIdAsync(Guid markaId);
    Task<ModelDto?> GetByIdAsync(Guid id);
    Task<ApiResponseDto<ModelDto>> CreateAsync(ModelCreateDto dto);
    Task<ApiResponseDto<ModelDto>> UpdateAsync(Guid id, ModelUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(Guid id);
}

public interface IKategoriService
{
    Task<List<KategoriDto>> GetAllAsync();
    Task<List<KategoriTreeDto>> GetTreeAsync();
    Task<KategoriDto?> GetByIdAsync(Guid id);
    Task<ApiResponseDto<KategoriDto>> CreateAsync(KategoriCreateDto dto);
    Task<ApiResponseDto<KategoriDto>> UpdateAsync(Guid id, KategoriUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(Guid id);
}

public interface IUrunService
{
    Task<PagedResultDto<UrunDto>> GetListAsync(int page, int pageSize, Guid? kategoriId = null, bool? aktif = null, string? arama = null);
    Task<UrunDto?> GetByIdAsync(Guid id);
    Task<UrunDto?> GetByBarkodAsync(string barkod);
    Task<List<SeriNumarasiDto>> GetSeriNumaralariAsync(Guid urunId, Guid? depoId = null);
    Task<int> GetStokAsync(Guid urunId, Guid? depoId = null);
    Task<ApiResponseDto<UrunDto>> CreateAsync(UrunCreateDto dto);
    Task<ApiResponseDto<UrunDto>> UpdateAsync(Guid id, UrunUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(Guid id);
}

public interface IBayiService
{
    Task<List<BayiDto>> GetAllAsync();
    Task<PagedResultDto<BayiDto>> GetListAsync(int page, int pageSize, bool? aktif = null);
    Task<BayiDto?> GetByIdAsync(Guid id);
    Task<List<DepoDto>> GetDepolarAsync(Guid bayiId);
    Task<KasaDto?> GetKasaAsync(Guid bayiId);
    Task<ApiResponseDto<BayiDto>> CreateAsync(BayiCreateDto dto);
    Task<ApiResponseDto<BayiDto>> UpdateAsync(Guid id, BayiUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(Guid id);
}

public interface IDepoService
{
    Task<DepoDto?> GetByIdAsync(Guid id);
    Task<List<DepoDto>> GetByBayiIdAsync(Guid bayiId);
    Task<ApiResponseDto<DepoDto>> CreateAsync(DepoCreateDto dto);
    Task<ApiResponseDto<DepoDto>> UpdateAsync(Guid id, DepoUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(Guid id);
}

public interface IMusteriService
{
    Task<PagedResultDto<MusteriDto>> GetListAsync(int page, int pageSize, string? arama = null);
    Task<MusteriDto?> GetByIdAsync(Guid id);
    Task<MusteriDto?> GetByTelefonAsync(string telefon);
    Task<List<SatisDto>> GetSatislarAsync(Guid musteriId);
    Task<List<TeknikServisDto>> GetTeknikServislerAsync(Guid musteriId);
    Task<ApiResponseDto<MusteriDto>> CreateAsync(MusteriCreateDto dto);
    Task<ApiResponseDto<MusteriDto>> UpdateAsync(Guid id, MusteriUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(Guid id);
}

public interface IKasaService
{
    Task<KasaDto?> GetByIdAsync(Guid id);
    Task<KasaDto?> GetByBayiIdAsync(Guid bayiId);
    Task<PagedResultDto<KasaHareketDto>> GetHareketlerAsync(Guid kasaId, int page, int pageSize, DateTime? baslangic = null, DateTime? bitis = null);
    Task<ApiResponseDto<KasaHareketDto>> HareketEkleAsync(KasaHareketEkleDto dto);
    Task<decimal> GetGunlukCiroAsync(Guid bayiId, DateTime? tarih = null);
    Task<ApiResponseDto<KasaDto>> CreateAsync(KasaCreateDto dto);
}

public interface IPersonelService
{
    Task<PersonelDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<PersonelDto>> GetListAsync(int page, int pageSize, Guid? bayiId = null, bool? aktif = null);
    Task<ApiResponseDto<PersonelDto>> CreateAsync(PersonelCreateDto dto);
    Task<ApiResponseDto<PersonelDto>> UpdateAsync(Guid id, PersonelUpdateDto dto);
    Task<ApiResponseDto<bool>> DeleteAsync(Guid id);
    Task<MaasHesaplamaDto> MaasHesaplaAsync(Guid personelId, int yil, int ay);
    Task<ApiResponseDto<PersonelMaasOdemeDto>> MaasOdeAsync(Guid personelId, int yil, int ay);
    Task<List<PersonelMaasOdemeDto>> GetMaasOdemeleriAsync(Guid personelId);
    Task<ApiResponseDto<PersonelAvansDto>> AvansTalepEtAsync(Guid personelId, decimal tutar);
    Task<ApiResponseDto<PersonelAvansDto>> AvansOnaylaAsync(Guid avansId, bool onayla);
    Task<List<PersonelAvansDto>> GetAvanslarAsync(Guid personelId);
    Task<ApiResponseDto<PersonelIzinDto>> IzinTalepEtAsync(PersonelIzinCreateDto dto);
    Task<ApiResponseDto<PersonelIzinDto>> IzinOnaylaAsync(Guid izinId, bool onayla, string? neden = null);
    Task<List<PersonelIzinDto>> GetIzinlerAsync(Guid personelId);
    Task<int> KalanIzinGunuAsync(Guid personelId, int yil);
    Task<ApiResponseDto<PersonelMesaiDto>> MesaiKaydetAsync(PersonelMesaiCreateDto dto);
    Task<ApiResponseDto<PersonelMesaiDto>> MesaiOnaylaAsync(Guid mesaiId);
    Task<List<PersonelMesaiDto>> GetMesailerAsync(Guid personelId, int? yil = null, int? ay = null);
}

public interface ISatisService
{
    Task<SatisDto?> GetByIdAsync(Guid id);
    Task<SatisDto?> GetBySatisNoAsync(string satisNo);
    Task<PagedResultDto<SatisDto>> GetListAsync(int page, int pageSize, Guid? bayiId = null, DateTime? baslangic = null, DateTime? bitis = null);
    Task<ApiResponseDto<SatisDto>> CreateAsync(SatisCreateDto dto);
    Task<ApiResponseDto<SatisDto>> IptalAsync(Guid id, string? neden = null);
    Task<SatisValidasyonDto> ValidateSeriNumarasiAsync(string seriNo);
}

public interface ITeknikServisService
{
    Task<TeknikServisDto?> GetByIdAsync(Guid id);
    Task<TeknikServisDto?> GetByServisNoAsync(string servisNo);
    Task<List<TeknikServisDto>> GetByMusteriTelefonAsync(string telefon);
    Task<PagedResultDto<TeknikServisDto>> GetListAsync(int page, int pageSize, Guid? bayiId = null, string? durum = null);
    Task<ApiResponseDto<TeknikServisDto>> CreateAsync(TeknikServisCreateDto dto);
    Task<ApiResponseDto<TeknikServisDto>> DurumGuncelleAsync(Guid id, TeknikServisDurumGuncelleDto dto);
    Task<ApiResponseDto<TeknikServisDto>> ParcaEkleAsync(Guid id, TeknikServisParcaEkleDto dto);
    Task<ApiResponseDto<TeknikServisDto>> TeslimEtAsync(Guid id);
    Task<ApiResponseDto<TeknikServisDto>> OdemeAlAsync(Guid id, TeknikServisOdemeAlDto dto);
    Task<ApiResponseDto<TeknikServisDto>> IptalAsync(Guid id, string? neden = null);
}

public interface ITransferService
{
    Task<TransferDto?> GetByIdAsync(Guid id);
    Task<TransferDto?> GetByTransferNoAsync(string transferNo);
    Task<PagedResultDto<TransferDto>> GetListAsync(int page, int pageSize, Guid? kaynakDepoId = null, Guid? hedefDepoId = null, string? durum = null);
    Task<ApiResponseDto<TransferDto>> CreateAsync(TransferCreateDto dto);
    Task<ApiResponseDto<TransferDto>> OnaylaAsync(Guid id);
    Task<ApiResponseDto<TransferDto>> TamamlaAsync(Guid id);
    Task<ApiResponseDto<TransferKalemDto>> KalemTeslimAlAsync(Guid transferId, Guid kalemId);
    Task<ApiResponseDto<TransferDto>> IptalAsync(Guid id, string? neden = null);
}

public interface IIadeService
{
    Task<IadeDto?> GetByIdAsync(Guid id);
    Task<IadeDto?> GetByIadeNoAsync(string iadeNo);
    Task<PagedResultDto<IadeDto>> GetListAsync(int page, int pageSize, Guid? bayiId = null, string? durum = null);
    Task<ApiResponseDto<IadeDto>> CreateAsync(IadeCreateDto dto);
    Task<ApiResponseDto<IadeDto>> OnaylaAsync(Guid id);
    Task<ApiResponseDto<IadeDto>> TamamlaAsync(Guid id);
    Task<ApiResponseDto<IadeDto>> ReddetAsync(Guid id, string? neden = null);
}

public interface IFaturaService
{
    Task<FaturaDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<FaturaDto>> GetListAsync(int page, int pageSize, string? tip = null, bool? onaylandi = null);
    Task<ApiResponseDto<FaturaDto>> CreateAsync(FaturaCreateDto dto);
    Task<ApiResponseDto<FaturaDto>> OnaylaAsync(Guid id);
    Task<ApiResponseDto<bool>> IptalAsync(Guid id, string? neden = null);
}

public interface ISayimService
{
    Task<SayimDto?> GetByIdAsync(Guid id);
    Task<SayimDto?> GetBySayimNoAsync(string sayimNo);
    Task<PagedResultDto<SayimDto>> GetListAsync(int page, int pageSize, Guid? depoId = null, string? durum = null);
    Task<ApiResponseDto<SayimDto>> BaslatAsync(SayimBaslatDto dto);
    Task<ApiResponseDto<SayimSeriTaramaResultDto>> SeriTaraAsync(Guid sayimId, string seriNo);
    Task<ApiResponseDto<SayimRaporDto>> TamamlaAsync(Guid id);
    Task<SayimRaporDto?> GetRaporAsync(Guid id);
    Task<ApiResponseDto<SayimDto>> IptalAsync(Guid id);
}

public interface IStokService
{
    Task<List<StokDurumDto>> GetTumStokDurumuAsync(Guid? bayiId = null);
    Task<StokDurumDto> GetUrunStokDurumuAsync(Guid urunId, Guid? bayiId = null);
    Task<List<UrunStokDto>> GetKritikStokUrunlerAsync(Guid? bayiId = null);
    Task<PagedResultDto<StokHareketDto>> GetHareketlerAsync(int page, int pageSize, Guid? urunId = null, Guid? depoId = null, DateTime? baslangic = null, DateTime? bitis = null);
    Task<SeriNumarasiDetayDto?> GetSeriNumarasiBilgiAsync(string seriNo);
    Task<ApiResponseDto<List<SeriNumarasiDto>>> StokGirisiAsync(StokGirisDto dto);
    Task<ApiResponseDto<bool>> StokCikisiAsync(StokCikisDto dto);
    Task<ApiResponseDto<StokGirisSonucDto>> StokGirisiWithFaturaAsync(StokGirisFaturaDto dto);
    Task<List<MusteriDto>> GetTedarikciListAsync();
}

public interface IRaporService
{
    Task<OzetRaporDto> GetOzetRaporAsync(Guid? bayiId = null);
    Task<List<GunlukSatisRaporDto>> GetSatisRaporuAsync(DateTime baslangic, DateTime bitis, Guid? bayiId = null);
    Task<List<BayiPerformansDto>> GetBayiPerformansAsync(DateTime baslangic, DateTime bitis);
    Task<List<PersonelPerformansDto>> GetPersonelPerformansAsync(DateTime baslangic, DateTime bitis, Guid? bayiId = null);
    Task<List<UrunStokRaporDto>> GetStokRaporuAsync(Guid? bayiId = null);
}
