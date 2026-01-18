using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class Sale : BranchEntity
{
    public string SaleNumber { get; set; } = null!;
    public Guid? CustomerId { get; set; }
    public Guid UserId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    
    // Tutarlar
    public decimal SubTotal { get; set; }
    public decimal VatTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal GrandTotal { get; set; }
    
    // Ödeme
    public PaymentMethod PaymentMethod { get; set; }
    public bool IsPaid { get; set; } = true;
    
    public string? Notes { get; set; }

    // Navigation
    public virtual Customer? Customer { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    public virtual CashMovement? CashMovement { get; set; }
}

public class SaleItem : TenantEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public Guid SerialNumberId { get; set; }
    public string Barcode { get; set; } = null!;
    public string Serial { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation
    public virtual Sale Sale { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual SerialNumber SerialNumber { get; set; } = null!;
}
