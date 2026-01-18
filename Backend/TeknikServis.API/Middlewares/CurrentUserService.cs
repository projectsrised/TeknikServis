using System.Security.Claims;
using TeknikServis.Business.Interfaces;
using TeknikServis.Domain.Enums;

namespace TeknikServis.API.Middlewares;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
        }
    }

    public Guid? TenantId
    {
        get
        {
            var tenantId = User?.FindFirst("TenantId")?.Value;
            return string.IsNullOrEmpty(tenantId) ? null : Guid.Parse(tenantId);
        }
    }

    public Guid? BranchId
    {
        get
        {
            var branchId = User?.FindFirst("BranchId")?.Value;
            return string.IsNullOrEmpty(branchId) ? null : Guid.Parse(branchId);
        }
    }

    public UserRole? Role
    {
        get
        {
            var role = User?.FindFirst(ClaimTypes.Role)?.Value;
            return string.IsNullOrEmpty(role) ? null : Enum.Parse<UserRole>(role);
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsTenantAdmin => Role == UserRole.TenantAdmin;
    
    public bool IsBranchAdmin => Role == UserRole.BranchAdmin || Role == UserRole.BranchManager;
    
    public bool IsSalesStaff => Role == UserRole.SalesStaff;
    
    public bool IsTechnicalStaff => Role == UserRole.TechnicalStaff;

    public bool CanAccessBranch(Guid branchId)
    {
        if (IsTenantAdmin) return true;
        return BranchId.HasValue && BranchId.Value == branchId;
    }

    public bool CanManageUsers => IsTenantAdmin || IsBranchAdmin;
    
    public bool CanManageBranches => IsTenantAdmin;
    
    public bool CanViewAllSales => IsTenantAdmin || IsBranchAdmin;
    
    public bool CanViewAllServices => IsTenantAdmin || IsBranchAdmin;
}
