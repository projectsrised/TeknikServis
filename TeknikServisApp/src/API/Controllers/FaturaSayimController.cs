using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/fatura")]
public class FaturaController : ControllerBase
{
    private readonly IFaturaService _faturaService;

    public FaturaController(IFaturaService faturaService)
    {
        _faturaService = faturaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? tip = null, [FromQuery] bool? onaylandi = null)
    {
        var result = await _faturaService.GetListAsync(page, pageSize, tip, onaylandi);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var fatura = await _faturaService.GetByIdAsync(id);
        return fatura != null ? Ok(fatura) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FaturaCreateDto dto)
    {
        var result = await _faturaService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/onayla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Onayla(Guid id)
    {
        var result = await _faturaService.OnaylaAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/iptal")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Iptal(Guid id, [FromBody] IptalRequest? request = null)
    {
        var result = await _faturaService.IptalAsync(id, request?.Neden);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

[Authorize]
[ApiController]
[Route("api/sayim")]
public class SayimController : ControllerBase
{
    private readonly ISayimService _sayimService;

    public SayimController(ISayimService sayimService)
    {
        _sayimService = sayimService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? depoId = null, [FromQuery] string? durum = null)
    {
        var result = await _sayimService.GetListAsync(page, pageSize, depoId, durum);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sayim = await _sayimService.GetByIdAsync(id);
        return sayim != null ? Ok(sayim) : NotFound();
    }

    [HttpGet("no/{sayimNo}")]
    public async Task<IActionResult> GetBySayimNo(string sayimNo)
    {
        var sayim = await _sayimService.GetBySayimNoAsync(sayimNo);
        return sayim != null ? Ok(sayim) : NotFound();
    }

    [HttpGet("{id}/rapor")]
    public async Task<IActionResult> GetRapor(Guid id)
    {
        var rapor = await _sayimService.GetRaporAsync(id);
        return rapor != null ? Ok(rapor) : NotFound();
    }

    [HttpPost("baslat")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Baslat([FromBody] SayimBaslatDto dto)
    {
        var result = await _sayimService.BaslatAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/seri-tara")]
    public async Task<IActionResult> SeriTara(Guid id, [FromBody] SeriTaraRequest request)
    {
        var result = await _sayimService.SeriTaraAsync(id, request.SeriNo);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/tamamla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Tamamla(Guid id)
    {
        var result = await _sayimService.TamamlaAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/iptal")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Iptal(Guid id)
    {
        var result = await _sayimService.IptalAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

public class SeriTaraRequest
{
    public string SeriNo { get; set; } = null!;
}
