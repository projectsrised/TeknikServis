using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServis.Business.DTOs;
using TeknikServis.Business.Interfaces;

namespace TeknikServis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _saleService.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _saleService.GetByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet("my-transactions")]
    public async Task<IActionResult> GetMyTransactions([FromQuery] PagedRequest request)
    {
        var result = await _saleService.GetMyTransactionsAsync(request);
        return Ok(result);
    }

    [HttpGet("scan/{serialOrBarcode}")]
    public async Task<IActionResult> ScanBarcode(string serialOrBarcode)
    {
        var result = await _saleService.ScanBarcodeAsync(serialOrBarcode);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request)
    {
        var result = await _saleService.CreateAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpGet("daily-summary")]
    public async Task<IActionResult> GetDailySummary(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? branchId = null)
    {
        var result = await _saleService.GetDailySummaryAsync(startDate, endDate, branchId);
        return Ok(result);
    }

    [HttpGet("user-summary")]
    public async Task<IActionResult> GetUserSummary(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? branchId = null)
    {
        var result = await _saleService.GetUserSummaryAsync(startDate, endDate, branchId);
        return Ok(result);
    }
}
