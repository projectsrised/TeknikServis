using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

public class TechnicalServiceDto
{
    public Guid Id { get; set; }
    public string ServiceNumber { get; set; } = null!;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    
    // Cihaz Bilgileri
    public string DeviceBrand { get; set; } = null!;
    public string DeviceModel { get; set; } = null!;
    public string? DeviceIMEI { get; set; }
    public string? DeviceSerialNumber { get; set; }
    public string? DevicePassword { get; set; }
    
    // Arıza Bilgileri
    public string FaultDescription { get; set; } = null!;
    public string? DiagnosisNotes { get; set; }
    
    // Durum
    public TechnicalServiceStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public DateTime ReceivedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    
    // Fiyatlandırma
    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }
    public decimal TotalCost { get; set; }
    public bool CustomerApproved { get; set; }
    public DateTime? CustomerApprovedAt { get; set; }
    
    // Ödeme
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentMethodName { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    
    public string? Notes { get; set; }
    public List<TechnicalServicePartDto> Parts { get; set; } = new();
    public List<TechnicalServiceStatusHistoryDto> StatusHistory { get; set; } = new();
}

public class TechnicalServicePartDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public Guid? SerialNumberId { get; set; }
    public string? Serial { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsUsed { get; set; }
}

public class TechnicalServiceStatusHistoryDto
{
    public Guid Id { get; set; }
    public TechnicalServiceStatus FromStatus { get; set; }
    public string FromStatusName { get; set; } = null!;
    public TechnicalServiceStatus ToStatus { get; set; }
    public string ToStatusName { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByName { get; set; } = null!;
}

public class CreateTechnicalServiceRequest
{
    // Müşteri bilgileri
    public Guid? CustomerId { get; set; }
    public CustomerInfo? NewCustomer { get; set; }
    
    // Cihaz bilgileri
    public string DeviceBrand { get; set; } = null!;
    public string DeviceModel { get; set; } = null!;
    public string? DeviceIMEI { get; set; }
    public string? DeviceSerialNumber { get; set; }
    public string? DevicePassword { get; set; }
    
    // Arıza bilgileri
    public string FaultDescription { get; set; } = null!;
    public string? Notes { get; set; }
}

public class UpdateTechnicalServiceRequest
{
    public string? DiagnosisNotes { get; set; }
    public decimal LaborCost { get; set; }
    public List<AddTechnicalServicePartRequest> Parts { get; set; } = new();
    public string? Notes { get; set; }
}

public class AddTechnicalServicePartRequest
{
    public Guid ProductId { get; set; }
    public Guid? SerialNumberId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}

public class ChangeServiceStatusRequest
{
    public TechnicalServiceStatus NewStatus { get; set; }
    public string? Notes { get; set; }
}

public class CustomerApproveServiceRequest
{
    public bool Approved { get; set; }
    public string? Notes { get; set; }
}

public class CompleteServicePaymentRequest
{
    public PaymentMethod PaymentMethod { get; set; }
}

public class TechnicalServiceListDto
{
    public Guid Id { get; set; }
    public string ServiceNumber { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string DeviceBrand { get; set; } = null!;
    public string DeviceModel { get; set; } = null!;
    public TechnicalServiceStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public decimal TotalCost { get; set; }
    public bool IsPaid { get; set; }
    public DateTime ReceivedAt { get; set; }
}

// Yazdırma için form
public class TechnicalServiceFormDto
{
    public string ServiceNumber { get; set; } = null!;
    public string TenantName { get; set; } = null!;
    public string BranchName { get; set; } = null!;
    public string BranchAddress { get; set; } = null!;
    public string BranchPhone { get; set; } = null!;
    
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string? CustomerAddress { get; set; }
    
    public string DeviceBrand { get; set; } = null!;
    public string DeviceModel { get; set; } = null!;
    public string? DeviceIMEI { get; set; }
    public string? DeviceSerialNumber { get; set; }
    
    public string FaultDescription { get; set; } = null!;
    public string? DiagnosisNotes { get; set; }
    
    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }
    public decimal TotalCost { get; set; }
    
    public List<TechnicalServicePartDto> Parts { get; set; } = new();
    
    public DateTime ReceivedAt { get; set; }
    public string ReceivedByName { get; set; } = null!;
}
