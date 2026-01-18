using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

public class TransferDto
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = null!;
    public Guid FromWarehouseId { get; set; }
    public string FromWarehouseName { get; set; } = null!;
    public string? FromBranchName { get; set; }
    public Guid ToWarehouseId { get; set; }
    public string ToWarehouseName { get; set; } = null!;
    public string? ToBranchName { get; set; }
    public TransferStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public DateTime TransferDate { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? DeliveredByName { get; set; }
    public string? Notes { get; set; }
    public int ItemCount { get; set; }
    public List<TransferItemDto> Items { get; set; } = new();
}

public class TransferItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Barcode { get; set; } = null!;
    public string Serial { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string? Brand { get; set; }
    public string? Model { get; set; }
}

public class CreateTransferRequest
{
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public string? Notes { get; set; }
    public List<CreateTransferItemRequest> Items { get; set; } = new();
}

public class CreateTransferItemRequest
{
    public string SerialOrBarcode { get; set; } = null!; // Seri numarası veya barkod
}

public class ApproveTransferRequest
{
    public Guid TransferId { get; set; }
    public string? Notes { get; set; }
}

public class DeliverTransferRequest
{
    public Guid TransferId { get; set; }
    public string? Notes { get; set; }
}

public class TransferListDto
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = null!;
    public string FromWarehouseName { get; set; } = null!;
    public string ToWarehouseName { get; set; } = null!;
    public TransferStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public DateTime TransferDate { get; set; }
    public int ItemCount { get; set; }
}

public class WarehouseDto
{
    public Guid Id { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsCentral { get; set; }
    public bool IsActive { get; set; }
    public int TotalStock { get; set; }
}

public class CreateWarehouseRequest
{
    public Guid? BranchId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsCentral { get; set; } = false;
}
