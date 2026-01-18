using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class User : TenantEntity
{
    public Guid? BranchId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public virtual Branch? Branch { get; set; }
    public virtual Employee? Employee { get; set; }
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<TechnicalService> TechnicalServices { get; set; } = new List<TechnicalService>();

    public string FullName => $"{FirstName} {LastName}";
}
