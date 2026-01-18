using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class StockMovement : TenantEntity
{
    public Guid? WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? SerialNumberId { get; set; }
    public StockMovementType Type { get; set; }
    public int Quantity { get; set; }
    public string? ReferenceNumber { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual Warehouse? Warehouse { get; set; }
    public virtual Product Product { get; set; } = null!;
    public virtual SerialNumber? SerialNumber { get; set; }
}
