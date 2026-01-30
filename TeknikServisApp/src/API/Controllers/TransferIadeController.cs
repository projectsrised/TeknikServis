using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/transfer")]
public class TransferController : ControllerBase
{
    private readonly ITransferService _transferService;

    public TransferController(ITransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? kaynakDepoId = null, [FromQuery] Guid? hedefDepoId = null, [FromQuery] string? durum = null)
    {
        var result = await _transferService.GetListAsync(page, pageSize, kaynakDepoId, hedefDepoId, durum);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transfer = await _transferService.GetByIdAsync(id);
        return transfer != null ? Ok(transfer) : NotFound();
    }

    [HttpGet("no/{transferNo}")]
    public async Task<IActionResult> GetByTransferNo(string transferNo)
    {
        var transfer = await _transferService.GetByTransferNoAsync(transferNo);
        return transfer != null ? Ok(transfer) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransferCreateDto dto)
    {
        var result = await _transferService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/onayla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Onayla(Guid id)
    {
        var result = await _transferService.OnaylaAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/tamamla")]
    public async Task<IActionResult> Tamamla(Guid id)
    {
        var result = await _transferService.TamamlaAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{transferId}/kalem/{kalemId}/teslim-al")]
    public async Task<IActionResult> KalemTeslimAl(Guid transferId, Guid kalemId)
    {
        var result = await _transferService.KalemTeslimAlAsync(transferId, kalemId);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/iptal")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Iptal(Guid id, [FromBody] IptalRequest? request = null)
    {
        var result = await _transferService.IptalAsync(id, request?.Neden);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

[Authorize]
[ApiController]
[Route("api/iade")]
public class IadeController : ControllerBase
{
    private readonly IIadeService _iadeService;

    public IadeController(IIadeService iadeService)
    {
        _iadeService = iadeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? bayiId = null, [FromQuery] string? durum = null)
    {
        var result = await _iadeService.GetListAsync(page, pageSize, bayiId, durum);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var iade = await _iadeService.GetByIdAsync(id);
        return iade != null ? Ok(iade) : NotFound();
    }

    [HttpGet("no/{iadeNo}")]
    public async Task<IActionResult> GetByIadeNo(string iadeNo)
    {
        var iade = await _iadeService.GetByIadeNoAsync(iadeNo);
        return iade != null ? Ok(iade) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IadeCreateDto dto)
    {
        var result = await _iadeService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/onayla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Onayla(Guid id)
    {
        var result = await _iadeService.OnaylaAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/tamamla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Tamamla(Guid id)
    {
        var result = await _iadeService.TamamlaAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/reddet")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Reddet(Guid id, [FromBody] IptalRequest? request = null)
    {
        var result = await _iadeService.ReddetAsync(id, request?.Neden);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}
