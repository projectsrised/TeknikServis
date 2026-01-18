using TeknikServis.Domain.Enums;

namespace TeknikServis.Domain.Events;

public abstract class DomainEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid UserId { get; set; }
}

// Stok Events
public class StockReceivedEvent : DomainEvent
{
    public Guid ProductId { get; set; }
    public Guid SerialNumberId { get; set; }
    public string Serial { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public Guid InvoiceId { get; set; }
}

public class StockTransferredEvent : DomainEvent
{
    public Guid TransferId { get; set; }
    public Guid SerialNumberId { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
}

public class StockSoldEvent : DomainEvent
{
    public Guid SaleId { get; set; }
    public Guid SerialNumberId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal SalePrice { get; set; }
}

public class StockReturnedEvent : DomainEvent
{
    public Guid ReturnId { get; set; }
    public Guid SerialNumberId { get; set; }
    public ReturnCondition Condition { get; set; }
    public Guid WarehouseId { get; set; }
}

public class StockUsedInServiceEvent : DomainEvent
{
    public Guid TechnicalServiceId { get; set; }
    public Guid SerialNumberId { get; set; }
    public Guid ProductId { get; set; }
}

// Satış Events
public class SaleCreatedEvent : DomainEvent
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}

public class SaleRefundedEvent : DomainEvent
{
    public Guid SaleId { get; set; }
    public Guid ReturnId { get; set; }
    public decimal RefundAmount { get; set; }
}

// Teknik Servis Events
public class TechnicalServiceCreatedEvent : DomainEvent
{
    public Guid TechnicalServiceId { get; set; }
    public string ServiceNumber { get; set; } = null!;
    public Guid CustomerId { get; set; }
}

public class TechnicalServiceStatusChangedEvent : DomainEvent
{
    public Guid TechnicalServiceId { get; set; }
    public TechnicalServiceStatus FromStatus { get; set; }
    public TechnicalServiceStatus ToStatus { get; set; }
}

public class TechnicalServiceCompletedEvent : DomainEvent
{
    public Guid TechnicalServiceId { get; set; }
    public decimal TotalCost { get; set; }
    public List<Guid> UsedPartIds { get; set; } = new();
}

// Kasa Events
public class CashMovementCreatedEvent : DomainEvent
{
    public Guid CashRegisterId { get; set; }
    public CashMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
}

// Transfer Events
public class TransferCreatedEvent : DomainEvent
{
    public Guid TransferId { get; set; }
    public string TransferNumber { get; set; } = null!;
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public int ItemCount { get; set; }
}

public class TransferDeliveredEvent : DomainEvent
{
    public Guid TransferId { get; set; }
    public Guid ToWarehouseId { get; set; }
}
