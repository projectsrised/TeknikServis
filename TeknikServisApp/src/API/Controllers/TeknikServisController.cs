using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/teknik-servis")]
public class TeknikServisController : ControllerBase
{
    private readonly ITeknikServisService _teknikServisService;

    public TeknikServisController(ITeknikServisService teknikServisService)
    {
        _teknikServisService = teknikServisService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? bayiId = null, [FromQuery] string? durum = null)
    {
        var result = await _teknikServisService.GetListAsync(page, pageSize, bayiId, durum);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var servis = await _teknikServisService.GetByIdAsync(id);
        return servis != null ? Ok(servis) : NotFound();
    }

    [HttpGet("no/{servisNo}")]
    public async Task<IActionResult> GetByServisNo(string servisNo)
    {
        var servis = await _teknikServisService.GetByServisNoAsync(servisNo);
        return servis != null ? Ok(servis) : NotFound();
    }

    [HttpGet("telefon/{telefon}")]
    public async Task<IActionResult> GetByMusteriTelefon(string telefon)
    {
        var servisler = await _teknikServisService.GetByMusteriTelefonAsync(telefon);
        return Ok(servisler);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TeknikServisCreateDto dto)
    {
        var result = await _teknikServisService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}/durum")]
    public async Task<IActionResult> DurumGuncelle(Guid id, [FromBody] TeknikServisDurumGuncelleDto dto)
    {
        var result = await _teknikServisService.DurumGuncelleAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/parca")]
    public async Task<IActionResult> ParcaEkle(Guid id, [FromBody] TeknikServisParcaEkleDto dto)
    {
        var result = await _teknikServisService.ParcaEkleAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/teslim")]
    public async Task<IActionResult> TeslimEt(Guid id)
    {
        var result = await _teknikServisService.TeslimEtAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/odeme")]
    public async Task<IActionResult> OdemeAl(Guid id, [FromBody] TeknikServisOdemeAlDto dto)
    {
        var result = await _teknikServisService.OdemeAlAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/iptal")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Iptal(Guid id, [FromBody] IptalRequest? request = null)
    {
        var result = await _teknikServisService.IptalAsync(id, request?.Neden);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}
