using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public InvoiceType Type { get; set; }
    public string TypeName { get; set; } = null!;
    public InvoiceStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierTaxNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerTaxNumber { get; set; }
    public decimal SubTotal { get; set; }
    public decimal VatTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Barcode { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public List<string> SerialNumbers { get; set; } = new();
}

public class CreateInvoiceRequest
{
    public InvoiceType Type { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierTaxNumber { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerTaxNumber { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceItemRequest> Items { get; set; } = new();
}

public class CreateInvoiceItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public List<string>? SerialNumbers { get; set; } // Giden fatura için mevcut seri numaraları
}

public class ApproveInvoiceRequest
{
    public Guid InvoiceId { get; set; }
}

// Gelen fatura onaylandığında seri numaraları otomatik üretilir
public class GeneratedSerialNumberDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public List<string> SerialNumbers { get; set; } = new();
}
