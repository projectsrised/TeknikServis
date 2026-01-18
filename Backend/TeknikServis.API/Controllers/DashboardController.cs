using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServis.Business.Interfaces;

namespace TeknikServis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IReportService _reportService;

    public DashboardController(IDashboardService dashboardService, IReportService reportService)
    {
        _dashboardService = dashboardService;
        _reportService = reportService;
    }

    [HttpGet("tenant")]
    [Authorize(Roles = "TenantAdmin")]
    public async Task<IActionResult> GetTenantDashboard()
    {
        var result = await _dashboardService.GetTenantDashboardAsync();
        return Ok(result);
    }

    [HttpGet("branch")]
    public async Task<IActionResult> GetBranchDashboard([FromQuery] Guid? branchId = null)
    {
        var result = await _dashboardService.GetBranchDashboardAsync(branchId);
        return Ok(result);
    }

    [HttpGet("reports/sales")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager")]
    public async Task<IActionResult> GetSalesReport([FromQuery] Business.DTOs.SalesReportRequest request)
    {
        var result = await _reportService.GetSalesReportAsync(request);
        return Ok(result);
    }

    [HttpGet("reports/stock")]
    [Authorize(Roles = "TenantAdmin,BranchAdmin,BranchManager")]
    public async Task<IActionResult> GetStockReport()
    {
        var result = await _reportService.GetStockReportAsync();
        return Ok(result);
    }
}
