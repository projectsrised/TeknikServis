using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/rapor")]
public class RaporController : ControllerBase
{
    private readonly IRaporService _raporService;

    public RaporController(IRaporService raporService)
    {
        _raporService = raporService;
    }

    [HttpGet("ozet")]
    public async Task<IActionResult> GetOzetRapor([FromQuery] Guid? bayiId = null)
    {
        var result = await _raporService.GetOzetRaporAsync(bayiId);
        return Ok(result);
    }

    [HttpGet("satis")]
    public async Task<IActionResult> GetSatisRaporu([FromQuery] DateTime baslangic, [FromQuery] DateTime bitis, [FromQuery] Guid? bayiId = null)
    {
        var result = await _raporService.GetSatisRaporuAsync(baslangic, bitis, bayiId);
        return Ok(result);
    }

    [HttpGet("bayi-performans")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<IActionResult> GetBayiPerformans([FromQuery] DateTime baslangic, [FromQuery] DateTime bitis)
    {
        var result = await _raporService.GetBayiPerformansAsync(baslangic, bitis);
        return Ok(result);
    }

    [HttpGet("personel-performans")]
    public async Task<IActionResult> GetPersonelPerformans([FromQuery] DateTime baslangic, [FromQuery] DateTime bitis, [FromQuery] Guid? bayiId = null)
    {
        var result = await _raporService.GetPersonelPerformansAsync(baslangic, bitis, bayiId);
        return Ok(result);
    }

    [HttpGet("stok")]
    public async Task<IActionResult> GetStokRaporu([FromQuery] Guid? bayiId = null)
    {
        var result = await _raporService.GetStokRaporuAsync(bayiId);
        return Ok(result);
    }
}
