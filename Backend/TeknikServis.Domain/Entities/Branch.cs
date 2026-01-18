namespace TeknikServis.Domain.Entities;

public class Branch : TenantEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsCentralBranch { get; set; } = false;

    // Navigation
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual Warehouse? Warehouse { get; set; }
    public virtual CashRegister? CashRegister { get; set; }
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<TechnicalService> TechnicalServices { get; set; } = new List<TechnicalService>();
}
