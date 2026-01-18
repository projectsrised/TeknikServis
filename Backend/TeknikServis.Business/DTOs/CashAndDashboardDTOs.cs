using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

// Kasa
public class CashRegisterDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; }
}

public class CashMovementDto
{
    public Guid Id { get; set; }
    public Guid CashRegisterId { get; set; }
    public CashMovementType Type { get; set; }
    public string TypeName { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = null!;
    public string? ReferenceNumber { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCashMovementRequest
{
    public CashMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Description { get; set; }
}

public class CashSummaryDto
{
    public decimal TotalBalance { get; set; }
    public decimal TodayIncome { get; set; }
    public decimal TodayExpense { get; set; }
    public decimal TodayNet { get; set; }
    public List<CashMovementDto> RecentMovements { get; set; } = new();
}

// Dashboard
public class DashboardDto
{
    public DashboardSummaryDto Summary { get; set; } = new();
    public List<SaleSummaryDto> DailySales { get; set; } = new();
    public List<SaleSummaryDto> MonthlySales { get; set; } = new();
    public List<UserSaleSummaryDto> TopSellers { get; set; } = new();
    public List<TechnicalServiceSummaryDto> ServiceSummary { get; set; } = new();
    public List<LowStockProductDto> LowStockProducts { get; set; } = new();
}

public class DashboardSummaryDto
{
    // Bugünkü
    public int TodaySalesCount { get; set; }
    public decimal TodaySalesAmount { get; set; }
    public int TodayServiceCount { get; set; }
    public decimal TodayServiceAmount { get; set; }
    
    // Bu ayki
    public int MonthSalesCount { get; set; }
    public decimal MonthSalesAmount { get; set; }
    public int MonthServiceCount { get; set; }
    public decimal MonthServiceAmount { get; set; }
    
    // Bekleyen
    public int PendingServices { get; set; }
    public int PendingTransfers { get; set; }
    public int LowStockCount { get; set; }
    
    // Kasa
    public decimal CashBalance { get; set; }
}

public class TechnicalServiceSummaryDto
{
    public TechnicalServiceStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public int Count { get; set; }
}

public class LowStockProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public int CurrentStock { get; set; }
    public int MinStockLevel { get; set; }
}

// Bayi Dashboard
public class BranchDashboardDto
{
    public BranchSummaryDto Summary { get; set; } = new();
    public List<SaleSummaryDto> WeeklySales { get; set; } = new();
    public List<UserSaleSummaryDto> StaffPerformance { get; set; } = new();
    public CashSummaryDto CashSummary { get; set; } = new();
    public List<TechnicalServiceListDto> RecentServices { get; set; } = new();
}

public class BranchSummaryDto
{
    public int TodaySalesCount { get; set; }
    public decimal TodaySalesAmount { get; set; }
    public int ActiveServices { get; set; }
    public int TotalStock { get; set; }
    public decimal CashBalance { get; set; }
}

// Raporlar
public class SalesReportRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? UserId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}

public class SalesReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalDiscount { get; set; }
    public Dictionary<string, decimal> ByPaymentMethod { get; set; } = new();
    public List<SaleSummaryDto> DailyBreakdown { get; set; } = new();
    public List<BranchSalesDto> ByBranch { get; set; } = new();
}

public class BranchSalesDto
{
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public int SalesCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class StockReportDto
{
    public int TotalProducts { get; set; }
    public int TotalSerialNumbers { get; set; }
    public int InStockCount { get; set; }
    public int SoldCount { get; set; }
    public int InTransferCount { get; set; }
    public int UsedInServiceCount { get; set; }
    public decimal TotalStockValue { get; set; }
    public List<WarehouseStockSummaryDto> ByWarehouse { get; set; } = new();
    public List<CategoryStockSummaryDto> ByCategory { get; set; } = new();
}

public class WarehouseStockSummaryDto
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public string? BranchName { get; set; }
    public int TotalStock { get; set; }
    public decimal StockValue { get; set; }
}

public class CategoryStockSummaryDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public int TotalStock { get; set; }
    public decimal StockValue { get; set; }
}
