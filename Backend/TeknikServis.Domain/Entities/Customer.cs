namespace TeknikServis.Domain.Entities;

public class Customer : TenantEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TcNo { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<TechnicalService> TechnicalServices { get; set; } = new List<TechnicalService>();
    public virtual ICollection<SerialNumber> PurchasedSerials { get; set; } = new List<SerialNumber>();

    public string FullName => $"{FirstName} {LastName}";
}
