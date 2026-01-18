using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class Product : TenantEntity
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int VatRate { get; set; } = 20;
    public bool TrackSerialNumber { get; set; } = true;
    public int MinStockLevel { get; set; } = 5;
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }

    // Navigation
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<SerialNumber> SerialNumbers { get; set; } = new List<SerialNumber>();
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
