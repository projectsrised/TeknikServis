using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class Transfer : TenantEntity
{
    public string TransferNumber { get; set; } = null!;
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.Pending;
    public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public Guid? DeliveredBy { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual Warehouse FromWarehouse { get; set; } = null!;
    public virtual Warehouse ToWarehouse { get; set; } = null!;
    public virtual ICollection<TransferItem> Items { get; set; } = new List<TransferItem>();
}

public class TransferItem : TenantEntity
{
    public Guid TransferId { get; set; }
    public Guid ProductId { get; set; }
    public Guid SerialNumberId { get; set; }
    public string Barcode { get; set; } = null!;
    public string Serial { get; set; } = null!;
    public string ProductName { get; set; } = null!;

    // Navigation
    public virtual Transfer Transfer { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual SerialNumber SerialNumber { get; set; } = null!;
}
