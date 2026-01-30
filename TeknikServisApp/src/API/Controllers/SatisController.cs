using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/satis")]
public class SatisController : ControllerBase
{
    private readonly ISatisService _satisService;

    public SatisController(ISatisService satisService)
    {
        _satisService = satisService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? bayiId = null, [FromQuery] DateTime? baslangic = null, [FromQuery] DateTime? bitis = null)
    {
        var result = await _satisService.GetListAsync(page, pageSize, bayiId, baslangic, bitis);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var satis = await _satisService.GetByIdAsync(id);
        return satis != null ? Ok(satis) : NotFound();
    }

    [HttpGet("no/{satisNo}")]
    public async Task<IActionResult> GetBySatisNo(string satisNo)
    {
        var satis = await _satisService.GetBySatisNoAsync(satisNo);
        return satis != null ? Ok(satis) : NotFound();
    }

    [HttpPost("validate-seri")]
    public async Task<IActionResult> ValidateSeriNumarasi([FromBody] ValidateSeriRequest request)
    {
        var result = await _satisService.ValidateSeriNumarasiAsync(request.SeriNo);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SatisCreateDto dto)
    {
        var result = await _satisService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/iptal")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Iptal(Guid id, [FromBody] IptalRequest? request = null)
    {
        var result = await _satisService.IptalAsync(id, request?.Neden);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

public class ValidateSeriRequest
{
    public string SeriNo { get; set; } = null!;
}

public class IptalRequest
{
    public string? Neden { get; set; }
}
