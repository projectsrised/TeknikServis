using TeknikServis.Business.DTOs;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ApiResponse<bool>> LogoutAsync(Guid userId);
    Task<ApiResponse<UserDto>> GetCurrentUserAsync();
    Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
}

public interface IUserService
{
    Task<ApiResponse<UserDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<UserDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<List<UserDto>>> GetByBranchAsync(Guid branchId);
    Task<ApiResponse<UserDto>> CreateAsync(CreateUserRequest request);
    Task<ApiResponse<UserDto>> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}

public interface ITenantService
{
    Task<ApiResponse<TenantDto>> GetCurrentTenantAsync();
    Task<ApiResponse<TenantDto>> UpdateAsync(UpdateTenantRequest request);
}

public interface IBranchService
{
    Task<ApiResponse<BranchDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<List<BranchDto>>> GetAllAsync();
    Task<ApiResponse<BranchDto>> CreateAsync(CreateBranchRequest request);
    Task<ApiResponse<BranchDto>> UpdateAsync(Guid id, UpdateBranchRequest request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}

public interface ICategoryService
{
    Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<List<CategoryDto>>> GetAllAsync();
    Task<ApiResponse<List<CategoryTreeDto>>> GetTreeAsync();
    Task<ApiResponse<List<CategoryDto>>> GetByTypeAsync(CategoryType type);
    Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryRequest request);
    Task<ApiResponse<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryRequest request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}

public interface IProductService
{
    Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<ProductDto>> GetByBarcodeAsync(string barcode);
    Task<ApiResponse<PagedResponse<ProductDto>>> GetAllAsync(PagedRequest request, Guid? categoryId = null);
    Task<ApiResponse<ProductStockDto>> GetStockInfoAsync(Guid productId);
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductRequest request);
    Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}

public interface IInvoiceService
{
    Task<ApiResponse<InvoiceDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<InvoiceDto>>> GetAllAsync(PagedRequest request, InvoiceType? type = null);
    Task<ApiResponse<InvoiceDto>> CreateAsync(CreateInvoiceRequest request);
    Task<ApiResponse<InvoiceDto>> ApproveAsync(Guid id);
    Task<ApiResponse<bool>> CancelAsync(Guid id);
    Task<ApiResponse<List<GeneratedSerialNumberDto>>> GetGeneratedSerialsAsync(Guid invoiceId);
}

public interface ISaleService
{
    Task<ApiResponse<SaleDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<SaleDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<PagedResponse<SaleDto>>> GetMyTransactionsAsync(PagedRequest request);
    Task<ApiResponse<ScanBarcodeResponse>> ScanBarcodeAsync(string serialOrBarcode);
    Task<ApiResponse<SaleDto>> CreateAsync(CreateSaleRequest request);
    Task<ApiResponse<List<SaleSummaryDto>>> GetDailySummaryAsync(DateTime startDate, DateTime endDate, Guid? branchId = null);
    Task<ApiResponse<List<UserSaleSummaryDto>>> GetUserSummaryAsync(DateTime startDate, DateTime endDate, Guid? branchId = null);
}

public interface ITechnicalServiceService
{
    Task<ApiResponse<TechnicalServiceDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<TechnicalServiceListDto>>> GetAllAsync(PagedRequest request, TechnicalServiceStatus? status = null);
    Task<ApiResponse<TechnicalServiceDto>> CreateAsync(CreateTechnicalServiceRequest request);
    Task<ApiResponse<TechnicalServiceDto>> UpdateAsync(Guid id, UpdateTechnicalServiceRequest request);
    Task<ApiResponse<TechnicalServiceDto>> ChangeStatusAsync(Guid id, ChangeServiceStatusRequest request);
    Task<ApiResponse<TechnicalServiceDto>> CustomerApproveAsync(Guid id, CustomerApproveServiceRequest request);
    Task<ApiResponse<TechnicalServiceDto>> CompletePaymentAsync(Guid id, CompleteServicePaymentRequest request);
    Task<ApiResponse<TechnicalServiceFormDto>> GetPrintFormAsync(Guid id);
}

public interface ITransferService
{
    Task<ApiResponse<TransferDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<TransferListDto>>> GetAllAsync(PagedRequest request, TransferStatus? status = null);
    Task<ApiResponse<TransferDto>> CreateAsync(CreateTransferRequest request);
    Task<ApiResponse<TransferDto>> ApproveAsync(Guid id, ApproveTransferRequest request);
    Task<ApiResponse<TransferDto>> DeliverAsync(Guid id, DeliverTransferRequest request);
    Task<ApiResponse<bool>> CancelAsync(Guid id);
}

public interface IWarehouseService
{
    Task<ApiResponse<WarehouseDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<List<WarehouseDto>>> GetAllAsync();
    Task<ApiResponse<WarehouseDto>> GetCentralWarehouseAsync();
    Task<ApiResponse<WarehouseDto>> CreateAsync(CreateWarehouseRequest request);
}

public interface IReturnService
{
    Task<ApiResponse<ReturnDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<ReturnListDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<SaleForReturnDto>> GetSaleForReturnAsync(string saleNumberOrSerial);
    Task<ApiResponse<ReturnDto>> CreateAsync(CreateReturnRequest request);
}

public interface IInventoryCountService
{
    Task<ApiResponse<InventoryCountDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<InventoryCountListDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<InventoryCountDto>> CreateAsync(CreateInventoryCountRequest request);
    Task<ApiResponse<ScanCountItemResponse>> ScanItemAsync(ScanCountItemRequest request);
    Task<ApiResponse<InventoryCountDto>> CompleteAsync(CompleteInventoryCountRequest request);
    Task<ApiResponse<InventoryCountDto>> ApproveAsync(ApproveInventoryCountRequest request);
    Task<ApiResponse<InventoryCountSummaryDto>> GetSummaryAsync(Guid countId);
}

public interface IEmployeeService
{
    Task<ApiResponse<EmployeeDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<EmployeeDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<EmployeeDto>> CreateAsync(CreateEmployeeRequest request);
    Task<ApiResponse<EmployeeDto>> UpdateAsync(Guid id, UpdateEmployeeRequest request);
    Task<ApiResponse<bool>> TerminateAsync(Guid id, DateTime terminationDate);
}

public interface ISalaryService
{
    Task<ApiResponse<EmployeeSalaryDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<List<EmployeeSalaryDto>>> GetByEmployeeAsync(Guid employeeId);
    Task<ApiResponse<List<EmployeeSalaryDto>>> GetByPeriodAsync(int year, int month);
    Task<ApiResponse<EmployeeSalaryDto>> CreateAsync(CreateSalaryRequest request);
    Task<ApiResponse<EmployeeSalaryDto>> PayAsync(PaySalaryRequest request);
}

public interface IAdvanceService
{
    Task<ApiResponse<EmployeeAdvanceDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<List<EmployeeAdvanceDto>>> GetByEmployeeAsync(Guid employeeId);
    Task<ApiResponse<List<EmployeeAdvanceDto>>> GetPendingAsync();
    Task<ApiResponse<EmployeeAdvanceDto>> CreateAsync(CreateAdvanceRequest request);
    Task<ApiResponse<EmployeeAdvanceDto>> ApproveAsync(ApproveAdvanceRequest request);
    Task<ApiResponse<EmployeeAdvanceDto>> PayAsync(PayAdvanceRequest request);
}

public interface ILeaveService
{
    Task<ApiResponse<EmployeeLeaveDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<List<EmployeeLeaveDto>>> GetByEmployeeAsync(Guid employeeId);
    Task<ApiResponse<List<EmployeeLeaveDto>>> GetPendingAsync();
    Task<ApiResponse<EmployeeLeaveDto>> CreateAsync(CreateLeaveRequest request);
    Task<ApiResponse<EmployeeLeaveDto>> ApproveAsync(ApproveLeaveRequest request);
    Task<ApiResponse<bool>> CancelAsync(Guid id);
}

public interface ICashService
{
    Task<ApiResponse<CashRegisterDto>> GetCurrentCashRegisterAsync();
    Task<ApiResponse<CashSummaryDto>> GetSummaryAsync(Guid? branchId = null);
    Task<ApiResponse<PagedResponse<CashMovementDto>>> GetMovementsAsync(PagedRequest request, Guid? branchId = null);
    Task<ApiResponse<CashMovementDto>> CreateMovementAsync(CreateCashMovementRequest request);
}

public interface IDashboardService
{
    Task<ApiResponse<DashboardDto>> GetTenantDashboardAsync();
    Task<ApiResponse<BranchDashboardDto>> GetBranchDashboardAsync(Guid? branchId = null);
}

public interface IReportService
{
    Task<ApiResponse<SalesReportDto>> GetSalesReportAsync(SalesReportRequest request);
    Task<ApiResponse<StockReportDto>> GetStockReportAsync();
}

public interface ICustomerService
{
    Task<ApiResponse<CustomerDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<PagedResponse<CustomerDto>>> GetAllAsync(PagedRequest request);
    Task<ApiResponse<CustomerDto>> GetByPhoneAsync(string phone);
    Task<ApiResponse<CustomerDto>> CreateAsync(CustomerInfo request);
    Task<ApiResponse<CustomerDto>> UpdateAsync(Guid id, CustomerInfo request);
}

// Tenant/Branch DTOs eklenmeli
public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Logo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public bool IsActive { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
}

public class UpdateTenantRequest
{
    public string Name { get; set; } = null!;
    public string? Logo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
}

public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public bool IsActive { get; set; }
    public bool IsCentralBranch { get; set; }
    public int UserCount { get; set; }
    public int StockCount { get; set; }
}

public class CreateBranchRequest
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public bool IsCentralBranch { get; set; } = false;
}

public class UpdateBranchRequest
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public bool IsActive { get; set; }
}

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TcNo { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public int TotalPurchases { get; set; }
    public int TotalServices { get; set; }
}
