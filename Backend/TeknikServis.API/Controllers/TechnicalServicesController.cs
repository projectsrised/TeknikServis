using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServis.Business.DTOs;
using TeknikServis.Business.Interfaces;
using TeknikServis.Domain.Enums;

namespace TeknikServis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TechnicalServicesController : ControllerBase
{
    private readonly ITechnicalServiceService _technicalServiceService;

    public TechnicalServicesController(ITechnicalServiceService technicalServiceService)
    {
        _technicalServiceService = technicalServiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request, [FromQuery] TechnicalServiceStatus? status = null)
    {
        var result = await _technicalServiceService.GetAllAsync(request, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _technicalServiceService.GetByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager,TechnicalStaff")]
    public async Task<IActionResult> Create([FromBody] CreateTechnicalServiceRequest request)
    {
        var result = await _technicalServiceService.CreateAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager,TechnicalStaff")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTechnicalServiceRequest request)
    {
        var result = await _technicalServiceService.UpdateAsync(id, request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/status")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager,TechnicalStaff")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeServiceStatusRequest request)
    {
        var result = await _technicalServiceService.ChangeStatusAsync(id, request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/customer-approve")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager,TechnicalStaff")]
    public async Task<IActionResult> CustomerApprove(Guid id, [FromBody] CustomerApproveServiceRequest request)
    {
        var result = await _technicalServiceService.CustomerApproveAsync(id, request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/payment")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager,TechnicalStaff,SalesStaff")]
    public async Task<IActionResult> CompletePayment(Guid id, [FromBody] CompleteServicePaymentRequest request)
    {
        var result = await _technicalServiceService.CompletePaymentAsync(id, request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}/print")]
    public async Task<IActionResult> GetPrintForm(Guid id)
    {
        var result = await _technicalServiceService.GetPrintFormAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
}
