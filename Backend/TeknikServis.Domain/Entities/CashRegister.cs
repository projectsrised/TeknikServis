using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class CashRegister : BranchEntity
{
    public string Name { get; set; } = null!;
    public decimal CurrentBalance { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<CashMovement> Movements { get; set; } = new List<CashMovement>();
}

public class CashMovement : TenantEntity
{
    public Guid CashRegisterId { get; set; }
    public Guid? BranchId { get; set; }
    public CashMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? Description { get; set; }

    // Navigation
    public virtual CashRegister CashRegister { get; set; } = null!;
    public virtual Branch? Branch { get; set; }
}
