using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

public class ReturnDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = null!;
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = null!;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = null!;
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public DateTime ReturnDate { get; set; }
    public ReturnReason Reason { get; set; }
    public string ReasonName { get; set; } = null!;
    public string? ReasonNotes { get; set; }
    public decimal RefundAmount { get; set; }
    public PaymentMethod RefundMethod { get; set; }
    public string RefundMethodName { get; set; } = null!;
    public bool IsRefunded { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? Notes { get; set; }
    public List<ReturnItemDto> Items { get; set; } = new();
}

public class ReturnItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Serial { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal RefundPrice { get; set; }
    public ReturnCondition Condition { get; set; }
    public string ConditionName { get; set; } = null!;
    public string? ConditionNotes { get; set; }
}

public class CreateReturnRequest
{
    public Guid SaleId { get; set; }
    public ReturnReason Reason { get; set; }
    public string? ReasonNotes { get; set; }
    public PaymentMethod RefundMethod { get; set; }
    public string? Notes { get; set; }
    public List<CreateReturnItemRequest> Items { get; set; } = new();
}

public class CreateReturnItemRequest
{
    public string SerialNumber { get; set; } = null!;
    public ReturnCondition Condition { get; set; }
    public string? ConditionNotes { get; set; }
}

public class ReturnListDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = null!;
    public string SaleNumber { get; set; } = null!;
    public string? CustomerName { get; set; }
    public DateTime ReturnDate { get; set; }
    public ReturnReason Reason { get; set; }
    public string ReasonName { get; set; } = null!;
    public decimal RefundAmount { get; set; }
    public bool IsRefunded { get; set; }
    public int ItemCount { get; set; }
}

// Satış bilgisinden iade yapılacak ürünleri getir
public class SaleForReturnDto
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = null!;
    public DateTime SaleDate { get; set; }
    public string? CustomerName { get; set; }
    public List<SaleItemForReturnDto> Items { get; set; } = new();
}

public class SaleItemForReturnDto
{
    public Guid SaleItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid SerialNumberId { get; set; }
    public string Serial { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal SalePrice { get; set; }
    public bool AlreadyReturned { get; set; }
}
