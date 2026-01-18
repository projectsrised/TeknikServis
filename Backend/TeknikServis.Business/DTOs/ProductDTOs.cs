namespace TeknikServis.Business.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int VatRate { get; set; }
    public bool TrackSerialNumber { get; set; }
    public int MinStockLevel { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public int TotalStock { get; set; }
    public int AvailableStock { get; set; }
}

public class CreateProductRequest
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int VatRate { get; set; } = 20;
    public bool TrackSerialNumber { get; set; } = true;
    public int MinStockLevel { get; set; } = 5;
    public string? ImageUrl { get; set; }
}

public class UpdateProductRequest
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int VatRate { get; set; }
    public int MinStockLevel { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
}

public class ProductStockDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public int TotalStock { get; set; }
    public int AvailableStock { get; set; }
    public int SoldStock { get; set; }
    public int InTransferStock { get; set; }
    public List<WarehouseStockDto> WarehouseStocks { get; set; } = new();
}

public class WarehouseStockDto
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public string? BranchName { get; set; }
    public int Quantity { get; set; }
}
