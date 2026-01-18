using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServis.Business.DTOs;
using TeknikServis.Business.Interfaces;

namespace TeknikServis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ISalaryService _salaryService;
    private readonly IAdvanceService _advanceService;
    private readonly ILeaveService _leaveService;

    public EmployeesController(
        IEmployeeService employeeService,
        ISalaryService salaryService,
        IAdvanceService advanceService,
        ILeaveService leaveService)
    {
        _employeeService = employeeService;
        _salaryService = salaryService;
        _advanceService = advanceService;
        _leaveService = leaveService;
    }

    // Employee CRUD
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        var result = await _employeeService.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _employeeService.GetByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var result = await _employeeService.CreateAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
    {
        var result = await _employeeService.UpdateAsync(id, request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/terminate")]
    public async Task<IActionResult> Terminate(Guid id, [FromQuery] DateTime terminationDate)
    {
        var result = await _employeeService.TerminateAsync(id, terminationDate);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    // Salary
    [HttpGet("{employeeId:guid}/salaries")]
    public async Task<IActionResult> GetSalaries(Guid employeeId)
    {
        var result = await _salaryService.GetByEmployeeAsync(employeeId);
        return Ok(result);
    }

    [HttpGet("salaries/period")]
    public async Task<IActionResult> GetSalariesByPeriod([FromQuery] int year, [FromQuery] int month)
    {
        var result = await _salaryService.GetByPeriodAsync(year, month);
        return Ok(result);
    }

    [HttpPost("salaries")]
    public async Task<IActionResult> CreateSalary([FromBody] CreateSalaryRequest request)
    {
        var result = await _salaryService.CreateAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("salaries/{id:guid}/pay")]
    public async Task<IActionResult> PaySalary(Guid id, [FromBody] PaySalaryRequest request)
    {
        request.SalaryId = id;
        var result = await _salaryService.PayAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    // Advances
    [HttpGet("advances/pending")]
    public async Task<IActionResult> GetPendingAdvances()
    {
        var result = await _advanceService.GetPendingAsync();
        return Ok(result);
    }

    [HttpGet("{employeeId:guid}/advances")]
    public async Task<IActionResult> GetAdvances(Guid employeeId)
    {
        var result = await _advanceService.GetByEmployeeAsync(employeeId);
        return Ok(result);
    }

    [HttpPost("advances")]
    public async Task<IActionResult> CreateAdvance([FromBody] CreateAdvanceRequest request)
    {
        var result = await _advanceService.CreateAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("advances/{id:guid}/approve")]
    public async Task<IActionResult> ApproveAdvance(Guid id, [FromBody] ApproveAdvanceRequest request)
    {
        request.AdvanceId = id;
        var result = await _advanceService.ApproveAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("advances/{id:guid}/pay")]
    public async Task<IActionResult> PayAdvance(Guid id, [FromBody] PayAdvanceRequest request)
    {
        request.AdvanceId = id;
        var result = await _advanceService.PayAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    // Leaves
    [HttpGet("leaves/pending")]
    public async Task<IActionResult> GetPendingLeaves()
    {
        var result = await _leaveService.GetPendingAsync();
        return Ok(result);
    }

    [HttpGet("{employeeId:guid}/leaves")]
    public async Task<IActionResult> GetLeaves(Guid employeeId)
    {
        var result = await _leaveService.GetByEmployeeAsync(employeeId);
        return Ok(result);
    }

    [HttpPost("leaves")]
    public async Task<IActionResult> CreateLeave([FromBody] CreateLeaveRequest request)
    {
        var result = await _leaveService.CreateAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("leaves/{id:guid}/approve")]
    public async Task<IActionResult> ApproveLeave(Guid id, [FromBody] ApproveLeaveRequest request)
    {
        request.LeaveId = id;
        var result = await _leaveService.ApproveAsync(request);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("leaves/{id:guid}/cancel")]
    public async Task<IActionResult> CancelLeave(Guid id)
    {
        var result = await _leaveService.CancelAsync(id);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
}
