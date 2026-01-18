using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class SerialNumber : TenantEntity
{
    public Guid ProductId { get; set; }
    public Guid? CurrentWarehouseId { get; set; }
    public Guid? InvoiceItemId { get; set; }
    public string Serial { get; set; } = null!;
    public string? IMEI { get; set; }
    public SerialNumberStatus Status { get; set; } = SerialNumberStatus.InStock;
    public DateTime? SoldAt { get; set; }
    public Guid? SoldToCustomerId { get; set; }
    public Guid? SaleItemId { get; set; }

    // Navigation
    public virtual Product Product { get; set; } = null!;
    public virtual Warehouse? CurrentWarehouse { get; set; }
    public virtual InvoiceItem? InvoiceItem { get; set; }
    public virtual Customer? SoldToCustomer { get; set; }
    public virtual SaleItem? SaleItem { get; set; }
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
