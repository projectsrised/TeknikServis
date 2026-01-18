using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServis.Business.DTOs;
using TeknikServis.Business.Interfaces;
using TeknikServis.Domain.Enums;

namespace TeknikServis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransfersController : ControllerBase
{
    private readonly ITransferService _transferService;

    public TransfersController(ITransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, [FromQuery] TransferStatus? status = null)
    {
        var result = await _transferService.GetAllAsync(request, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _transferService.GetByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager")]
    public async Task<IActionResult> Create([FromBody] CreateTransferRequest request)
    {
        var result = await _transferService.CreateAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveTransferRequest request)
    {
        request.TransferId = id;
        var result = await _transferService.ApproveAsync(id, request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/deliver")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager")]
    public async Task<IActionResult> Deliver(Guid id, [FromBody] DeliverTransferRequest request)
    {
        request.TransferId = id;
        var result = await _transferService.DeliverAsync(id, request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _transferService.CancelAsync(id);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
}
