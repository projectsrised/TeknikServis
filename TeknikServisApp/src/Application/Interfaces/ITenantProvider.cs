using TeknikServisApp.Domain.Enums;

namespace TeknikServisApp.Application.Interfaces;

public interface ITenantProvider
{
    Guid TenantId { get; }
    Guid? BayiId { get; }
    Guid? KullaniciId { get; }
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid TenantId { get; }
    Guid? BayiId { get; }
    UserRole Rol { get; }
    bool IsSuperAdmin { get; }
}
