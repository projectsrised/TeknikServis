using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServis.Business.DTOs;
using TeknikServis.Business.Interfaces;

namespace TeknikServis.API.Controllers;

[ApiController]
[Route("api/inventory-counts")]
[Authorize]
public class InventoryCountsController : ControllerBase
{
    private readonly IInventoryCountService _inventoryCountService;

    public InventoryCountsController(IInventoryCountService inventoryCountService)
    {
        _inventoryCountService = inventoryCountService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _inventoryCountService.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _inventoryCountService.GetByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        var result = await _inventoryCountService.GetSummaryAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager")]
    public async Task<IActionResult> Create([FromBody] CreateInventoryCountRequest request)
    {
        var result = await _inventoryCountService.CreateAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPost("scan")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager,SalesStaff")]
    public async Task<IActionResult> ScanItem([FromBody] ScanCountItemRequest request)
    {
        var result = await _inventoryCountService.ScanItemAsync(request);
        return Ok(result);
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteInventoryCountRequest request)
    {
        request.InventoryCountId = id;
        var result = await _inventoryCountService.CompleteAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveInventoryCountRequest request)
    {
        request.InventoryCountId = id;
        var result = await _inventoryCountService.ApproveAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
}
