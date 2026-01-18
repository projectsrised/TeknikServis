namespace TeknikServis.Domain.Enums;

public enum UserRole
{
    TenantAdmin = 1,
    BranchAdmin = 2,
    BranchManager = 3,
    SalesStaff = 4,
    TechnicalStaff = 5
}

public enum InvoiceType
{
    Purchase = 1,    // Gelen Fatura
    Sales = 2        // Giden Fatura
}

public enum InvoiceStatus
{
    Draft = 1,
    Approved = 2,
    Cancelled = 3
}

public enum TechnicalServiceStatus
{
    Pending = 1,         // Beklemede
    Approved = 2,        // Onaylandı
    InProgress = 3,      // İşlemde
    Completed = 4,       // Tamamlandı
    PaymentReceived = 5, // Ödemesi Alındı
    Cancelled = 6        // İptal Edildi
}

public enum TransferStatus
{
    Pending = 1,
    Approved = 2,
    InTransit = 3,
    Delivered = 4,
    Cancelled = 5
}

public enum StockMovementType
{
    PurchaseIn = 1,      // Alış girişi
    SaleOut = 2,         // Satış çıkışı
    TransferIn = 3,      // Transfer girişi
    TransferOut = 4,     // Transfer çıkışı
    ServiceOut = 5,      // Teknik servis çıkışı
    ReturnIn = 6,        // İade girişi
    ReturnOut = 7,       // İade çıkışı (hasarlı)
    InventoryAdjust = 8  // Sayım düzeltme
}

public enum SerialNumberStatus
{
    InStock = 1,         // Stokta
    Sold = 2,            // Satıldı
    InTransfer = 3,      // Transferde
    UsedInService = 4,   // Serviste kullanıldı
    Returned = 5,        // İade edildi
    Damaged = 6          // Hasarlı
}

public enum CashMovementType
{
    SaleIncome = 1,      // Satış geliri
    ServiceIncome = 2,   // Servis geliri
    RefundExpense = 3,   // İade gideri
    SalaryExpense = 4,   // Maaş gideri
    AdvanceExpense = 5,  // Avans gideri
    OtherIncome = 6,
    OtherExpense = 7
}

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    BankTransfer = 3,
    Installment = 4
}

public enum ReturnReason
{
    Defective = 1,       // Arızalı
    WrongProduct = 2,    // Yanlış ürün
    CustomerRegret = 3,  // Müşteri vazgeçti
    Other = 4
}

public enum ReturnCondition
{
    Good = 1,            // Sağlam - tekrar satılabilir
    Damaged = 2          // Hasarlı - satılamaz
}

public enum InventoryCountStatus
{
    InProgress = 1,
    Completed = 2,
    Approved = 3
}

public enum LeaveType
{
    Annual = 1,          // Yıllık izin
    Sick = 2,            // Hastalık izni
    Unpaid = 3,          // Ücretsiz izin
    Maternity = 4,       // Doğum izni
    Marriage = 5         // Evlilik izni
}

public enum LeaveStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}

public enum CategoryType
{
    Product = 1,
    TechnicalService = 2,
    Accessory = 3,
    Phone = 4
}
