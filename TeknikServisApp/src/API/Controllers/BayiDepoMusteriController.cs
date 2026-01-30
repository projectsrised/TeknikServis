using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/bayi")]
public class BayiController : ControllerBase
{
    private readonly IBayiService _bayiService;

    public BayiController(IBayiService bayiService)
    {
        _bayiService = bayiService;
    }

   [HttpGet]
public async Task<IActionResult> GetList([FromQuery] int? page = null, [FromQuery] int pageSize = 20, [FromQuery] bool? aktif = null)
{
    // Eğer page belirtilmemişse tüm listeyi döndür
    if (!page.HasValue)
    {
        var all = await _bayiService.GetAllAsync();
        return Ok(ApiResponseDto<List<BayiDto>>.Ok(all));
    }
    var result = await _bayiService.GetListAsync(page.Value, pageSize, aktif);
    return Ok(result);
}

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var bayi = await _bayiService.GetByIdAsync(id);
        return bayi != null
            ? Ok(new { basarili = true, data = bayi })
            : NotFound(new { basarili = false, mesaj = "Bayi bulunamadı" });
    }

    [HttpGet("{id}/depolar")]
    public async Task<IActionResult> GetDepolar(Guid id)
    {
        var depolar = await _bayiService.GetDepolarAsync(id);
        return Ok(new { basarili = true, data = depolar });
    }

    [HttpGet("{id}/kasa")]
    public async Task<IActionResult> GetKasa(Guid id)
    {
        var kasa = await _bayiService.GetKasaAsync(id);
        return kasa != null
            ? Ok(new { basarili = true, data = kasa })
            : NotFound(new { basarili = false, mesaj = "Kasa bulunamadı" });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<IActionResult> Create([FromBody] BayiCreateDto dto)
    {
        var result = await _bayiService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] BayiUpdateDto dto)
    {
        var result = await _bayiService.UpdateAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bayiService.DeleteAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

[Authorize]
[ApiController]
[Route("api/depo")]
public class DepoController : ControllerBase
{
    private readonly IDepoService _depoService;

    public DepoController(IDepoService depoService)
    {
        _depoService = depoService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var depo = await _depoService.GetByIdAsync(id);
        return depo != null
            ? Ok(new { basarili = true, data = depo })
            : NotFound(new { basarili = false, mesaj = "Depo bulunamadı" });
    }

    [HttpGet("bayi/{bayiId}")]
    public async Task<IActionResult> GetByBayiId(Guid bayiId)
    {
        var depolar = await _depoService.GetByBayiIdAsync(bayiId);
        return Ok(new { basarili = true, data = depolar });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Create([FromBody] DepoCreateDto dto)
    {
        var result = await _depoService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DepoUpdateDto dto)
    {
        var result = await _depoService.UpdateAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _depoService.DeleteAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

[Authorize]
[ApiController]
[Route("api/musteri")]
public class MusteriController : ControllerBase
{
    private readonly IMusteriService _musteriService;

    public MusteriController(IMusteriService musteriService)
    {
        _musteriService = musteriService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? arama = null)
    {
        var result = await _musteriService.GetListAsync(page, pageSize, arama);
        return Ok(new { basarili = true, data = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var musteri = await _musteriService.GetByIdAsync(id);
        return musteri != null
            ? Ok(new { basarili = true, data = musteri })
            : NotFound(new { basarili = false, mesaj = "Müşteri bulunamadı" });
    }

    [HttpGet("telefon/{telefon}")]
    public async Task<IActionResult> GetByTelefon(string telefon)
    {
        var musteri = await _musteriService.GetByTelefonAsync(telefon);
        return musteri != null
            ? Ok(new { basarili = true, data = musteri })
            : NotFound(new { basarili = false, mesaj = "Müşteri bulunamadı" });
    }

    [HttpGet("{id}/satislar")]
    public async Task<IActionResult> GetSatislar(Guid id)
    {
        var satislar = await _musteriService.GetSatislarAsync(id);
        return Ok(new { basarili = true, data = satislar });
    }

    [HttpGet("{id}/teknik-servisler")]
    public async Task<IActionResult> GetTeknikServisler(Guid id)
    {
        var servisler = await _musteriService.GetTeknikServislerAsync(id);
        return Ok(new { basarili = true, data = servisler });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MusteriCreateDto dto)
    {
        var result = await _musteriService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] MusteriUpdateDto dto)
    {
        var result = await _musteriService.UpdateAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _musteriService.DeleteAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}
