using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/stok")]
public class StokController : ControllerBase
{
    private readonly IStokService _stokService;

    public StokController(IStokService stokService)
    {
        _stokService = stokService;
    }

    [HttpGet("urun/{urunId}/durum")]
    public async Task<IActionResult> GetUrunStokDurumu(Guid urunId, [FromQuery] Guid? bayiId = null)
    {
        var result = await _stokService.GetUrunStokDurumuAsync(urunId, bayiId);
        return Ok(result);
    }

    [HttpGet("kritik")]
    public async Task<IActionResult> GetKritikStokUrunler([FromQuery] Guid? bayiId = null)
    {
        var result = await _stokService.GetKritikStokUrunlerAsync(bayiId);
        return Ok(result);
    }
[HttpGet("durum")]
public async Task<IActionResult> GetStokDurum([FromQuery] Guid? bayiId = null)
{
    var result = await _stokService.GetTumStokDurumuAsync(bayiId);
    return Ok(ApiResponseDto<List<StokDurumDto>>.Ok(result));
}
    [HttpGet("hareketler")]
    public async Task<IActionResult> GetHareketler([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? urunId = null, [FromQuery] Guid? depoId = null,
        [FromQuery] DateTime? baslangic = null, [FromQuery] DateTime? bitis = null)
    {
        var result = await _stokService.GetHareketlerAsync(page, pageSize, urunId, depoId, baslangic, bitis);
        return Ok(result);
    }

    [HttpGet("seri/{seriNo}")]
    public async Task<IActionResult> GetSeriNumarasiBilgi(string seriNo)
    {
        var result = await _stokService.GetSeriNumarasiBilgiAsync(seriNo);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost("giris")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> StokGirisi([FromBody] StokGirisDto dto)
    {
        var result = await _stokService.StokGirisiAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("cikis")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> StokCikisi([FromBody] StokCikisDto dto)
    {
        var result = await _stokService.StokCikisiAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpGet("tedarikciler")]
    public async Task<IActionResult> GetTedarikciler()
    {
        var result = await _stokService.GetTedarikciListAsync();
        return Ok(ApiResponseDto<List<MusteriDto>>.Ok(result));
    }

    [HttpPost("giris-fatura")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> StokGirisiWithFatura([FromBody] StokGirisFaturaDto dto)
    {
        var result = await _stokService.StokGirisiWithFaturaAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

[Authorize]
[ApiController]
[Route("api/kasa")]
public class KasaController : ControllerBase
{
    private readonly IKasaService _kasaService;

    public KasaController(IKasaService kasaService)
    {
        _kasaService = kasaService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var kasa = await _kasaService.GetByIdAsync(id);
        return kasa != null ? Ok(kasa) : NotFound();
    }

    [HttpGet("bayi/{bayiId}")]
    public async Task<IActionResult> GetByBayiId(Guid bayiId)
    {
        var kasa = await _kasaService.GetByBayiIdAsync(bayiId);
        return kasa != null ? Ok(kasa) : NotFound();
    }

    [HttpGet("{kasaId}/hareketler")]
    public async Task<IActionResult> GetHareketler(Guid kasaId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? baslangic = null, [FromQuery] DateTime? bitis = null)
    {
        var result = await _kasaService.GetHareketlerAsync(kasaId, page, pageSize, baslangic, bitis);
        return Ok(result);
    }

    [HttpGet("bayi/{bayiId}/gunluk-ciro")]
    public async Task<IActionResult> GetGunlukCiro(Guid bayiId, [FromQuery] DateTime? tarih = null)
    {
        var ciro = await _kasaService.GetGunlukCiroAsync(bayiId, tarih);
        return Ok(new { BayiId = bayiId, Ciro = ciro });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<IActionResult> Create([FromBody] KasaCreateDto dto)
    {
        var result = await _kasaService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("hareket")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> HareketEkle([FromBody] KasaHareketEkleDto dto)
    {
        var result = await _kasaService.HareketEkleAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}
