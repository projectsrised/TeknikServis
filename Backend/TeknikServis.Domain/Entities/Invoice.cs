using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class Invoice : TenantEntity
{
    public string InvoiceNumber { get; set; } = null!;
    public InvoiceType Type { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    
    // Tedarikçi/Müşteri bilgileri
    public string? SupplierName { get; set; }
    public string? SupplierTaxNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerTaxNumber { get; set; }
    
    // Tutarlar
    public decimal SubTotal { get; set; }
    public decimal VatTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal GrandTotal { get; set; }
    
    public string? Notes { get; set; }

    // Navigation
    public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}

public class InvoiceItem : TenantEntity
{
    public Guid InvoiceId { get; set; }
    public Guid ProductId { get; set; }
    public string Barcode { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation
    public virtual Invoice Invoice { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ICollection<SerialNumber> SerialNumbers { get; set; } = new List<SerialNumber>();
}
