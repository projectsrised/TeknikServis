namespace TeknikServis.Domain.Entities;

public class Warehouse : TenantEntity
{
    public Guid? BranchId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsCentral { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Branch? Branch { get; set; }
    public virtual ICollection<SerialNumber> SerialNumbers { get; set; } = new List<SerialNumber>();
    public virtual ICollection<StockMovement> StockMovementsIn { get; set; } = new List<StockMovement>();
    public virtual ICollection<StockMovement> StockMovementsOut { get; set; } = new List<StockMovement>();
    public virtual ICollection<Transfer> TransfersFrom { get; set; } = new List<Transfer>();
    public virtual ICollection<Transfer> TransfersTo { get; set; } = new List<Transfer>();
}
