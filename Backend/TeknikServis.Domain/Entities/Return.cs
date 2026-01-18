using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class Return : BranchEntity
{
    public string ReturnNumber { get; set; } = null!;
    public Guid SaleId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    public ReturnReason Reason { get; set; }
    public string? ReasonNotes { get; set; }
    
    // Tutarlar
    public decimal RefundAmount { get; set; }
    public PaymentMethod RefundMethod { get; set; }
    public bool IsRefunded { get; set; } = false;
    public DateTime? RefundedAt { get; set; }
    
    public string? Notes { get; set; }

    // Navigation
    public virtual Sale Sale { get; set; } = null!;
    public virtual Customer? Customer { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<ReturnItem> Items { get; set; } = new List<ReturnItem>();
    public virtual CashMovement? CashMovement { get; set; }
}

public class ReturnItem : TenantEntity
{
    public Guid ReturnId { get; set; }
    public Guid SaleItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid SerialNumberId { get; set; }
    public string Serial { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal RefundPrice { get; set; }
    public ReturnCondition Condition { get; set; }
    public string? ConditionNotes { get; set; }

    // Navigation
    public virtual Return Return { get; set; } = null!;
    public virtual SaleItem SaleItem { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual SerialNumber SerialNumber { get; set; } = null!;
}
