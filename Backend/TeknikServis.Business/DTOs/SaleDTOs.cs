using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

public class SaleDto
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = null!;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public DateTime SaleDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VatTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = null!;
    public bool IsPaid { get; set; }
    public string? Notes { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}

public class SaleItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Barcode { get; set; } = null!;
    public string Serial { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateSaleRequest
{
    public Guid? CustomerId { get; set; }
    public CustomerInfo? NewCustomer { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}

public class CreateSaleItemRequest
{
    public string SerialOrBarcode { get; set; } = null!; // Barkod veya seri numarası
    public decimal? DiscountAmount { get; set; }
}

public class CustomerInfo
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TcNo { get; set; }
}

public class ScanBarcodeResponse
{
    public bool Found { get; set; }
    public bool IsAvailable { get; set; }
    public string? Message { get; set; }
    public SerialNumberInfoDto? SerialInfo { get; set; }
}

public class SerialNumberInfoDto
{
    public Guid SerialNumberId { get; set; }
    public Guid ProductId { get; set; }
    public string Serial { get; set; } = null!;
    public string? IMEI { get; set; }
    public string ProductName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int VatRate { get; set; }
    public string Status { get; set; } = null!;
    public string? CurrentWarehouse { get; set; }
}

public class SaleSummaryDto
{
    public DateTime Date { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CashAmount { get; set; }
    public decimal CardAmount { get; set; }
}

public class UserSaleSummaryDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public int TotalSales { get; set; }
    public decimal TotalAmount { get; set; }
}
