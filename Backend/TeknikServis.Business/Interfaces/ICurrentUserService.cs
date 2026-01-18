using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? BranchId { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
    bool IsTenantAdmin { get; }
    bool IsBranchAdmin { get; }
    bool IsSalesStaff { get; }
    bool IsTechnicalStaff { get; }
    bool CanAccessBranch(Guid branchId);
    bool CanManageUsers { get; }
    bool CanManageBranches { get; }
    bool CanViewAllSales { get; }
    bool CanViewAllServices { get; }
}
