using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class InventoryCount : BranchEntity
{
    public string CountNumber { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CountDate { get; set; } = DateTime.UtcNow;
    public InventoryCountStatus Status { get; set; } = InventoryCountStatus.InProgress;
    public DateTime? CompletedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? Notes { get; set; }
    
    // Özet bilgiler
    public int TotalSystemStock { get; set; }
    public int TotalPhysicalStock { get; set; }
    public int TotalSoldItems { get; set; }
    public int Difference { get; set; }

    // Navigation
    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<InventoryCountItem> Items { get; set; } = new List<InventoryCountItem>();
}

public class InventoryCountItem : TenantEntity
{
    public Guid InventoryCountId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? SerialNumberId { get; set; }
    public string? Serial { get; set; }
    public string Barcode { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public bool IsInSystem { get; set; }
    public bool IsPhysicallyPresent { get; set; }
    public bool IsSold { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual InventoryCount InventoryCount { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual SerialNumber? SerialNumber { get; set; }
}
