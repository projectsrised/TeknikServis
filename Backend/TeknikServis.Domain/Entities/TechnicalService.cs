using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Entities;

public class TechnicalService : BranchEntity
{
    public string ServiceNumber { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    
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
    public TechnicalServiceStatus Status { get; set; } = TechnicalServiceStatus.Pending;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    
    // Fiyatlandırma
    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }
    public decimal TotalCost { get; set; }
    public bool CustomerApproved { get; set; } = false;
    public DateTime? CustomerApprovedAt { get; set; }
    
    // Ödeme
    public PaymentMethod? PaymentMethod { get; set; }
    public bool IsPaid { get; set; } = false;
    public DateTime? PaidAt { get; set; }
    
    public string? Notes { get; set; }

    // Navigation
    public virtual Customer Customer { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<TechnicalServicePart> Parts { get; set; } = new List<TechnicalServicePart>();
    public virtual ICollection<TechnicalServiceStatusHistory> StatusHistory { get; set; } = new List<TechnicalServiceStatusHistory>();
}

public class TechnicalServicePart : TenantEntity
{
    public Guid TechnicalServiceId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? SerialNumberId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsUsed { get; set; } = false;

    // Navigation
    public virtual TechnicalService TechnicalService { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual SerialNumber? SerialNumber { get; set; }
}

public class TechnicalServiceStatusHistory : TenantEntity
{
    public Guid TechnicalServiceId { get; set; }
    public TechnicalServiceStatus FromStatus { get; set; }
    public TechnicalServiceStatus ToStatus { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public virtual TechnicalService TechnicalService { get; set; } = null!;
}
