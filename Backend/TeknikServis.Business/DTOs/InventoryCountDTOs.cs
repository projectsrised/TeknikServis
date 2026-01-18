using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

public class InventoryCountDto
{
    public Guid Id { get; set; }
    public string CountNumber { get; set; } = null!;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public DateTime CountDate { get; set; }
    public InventoryCountStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public DateTime? CompletedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedByName { get; set; }
    public string? Notes { get; set; }
    
    // Özet bilgiler
    public int TotalSystemStock { get; set; }
    public int TotalPhysicalStock { get; set; }
    public int TotalSoldItems { get; set; }
    public int Difference { get; set; }
    
    public List<InventoryCountItemDto> Items { get; set; } = new();
}

public class InventoryCountItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? SerialNumberId { get; set; }
    public string? Serial { get; set; }
    public string Barcode { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public bool IsInSystem { get; set; }
    public bool IsPhysicallyPresent { get; set; }
    public bool IsSold { get; set; }
    public string Status { get; set; } = null!; // Sistemde var, Fiziksel var, Satılmış, Eksik
    public string? Notes { get; set; }
}

public class CreateInventoryCountRequest
{
    public Guid WarehouseId { get; set; }
    public string? Notes { get; set; }
}

public class ScanCountItemRequest
{
    public Guid InventoryCountId { get; set; }
    public string SerialOrBarcode { get; set; } = null!;
}

public class ScanCountItemResponse
{
    public bool Found { get; set; }
    public string? Message { get; set; }
    public InventoryCountItemDto? Item { get; set; }
}

public class CompleteInventoryCountRequest
{
    public Guid InventoryCountId { get; set; }
    public string? Notes { get; set; }
}

public class ApproveInventoryCountRequest
{
    public Guid InventoryCountId { get; set; }
    public bool ApplyAdjustments { get; set; } // Farkları stoka uygula
    public string? Notes { get; set; }
}

public class InventoryCountListDto
{
    public Guid Id { get; set; }
    public string CountNumber { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTime CountDate { get; set; }
    public InventoryCountStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public int TotalSystemStock { get; set; }
    public int TotalPhysicalStock { get; set; }
    public int Difference { get; set; }
}

public class InventoryCountSummaryDto
{
    public int TotalSystemStock { get; set; }
    public int TotalPhysicalStock { get; set; }
    public int TotalSoldItems { get; set; }
    public int MissingItems { get; set; }
    public int ExtraItems { get; set; }
    public List<InventoryCountItemDto> MissingItemsList { get; set; } = new();
    public List<InventoryCountItemDto> ExtraItemsList { get; set; } = new();
}
