using Microsoft.EntityFrameworkCore;
using TeknikServis.Business.DTOs;
using TeknikServis.Business.Interfaces;
using TeknikServis.DataAccess.Repositories;
using TeknikServis.Domain.Entities;
using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.Services;

public class SaleService : ISaleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public SaleService(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<SaleDto>> GetByIdAsync(Guid id)
    {
        var sale = await _unitOfWork.Sales.Query()
            .Include(s => s.Branch)
            .Include(s => s.Customer)
            .Include(s => s.User)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
            return ApiResponse<SaleDto>.Fail("Satış bulunamadı");

        if (!_currentUser.IsTenantAdmin && sale.BranchId != _currentUser.BranchId)
            return ApiResponse<SaleDto>.Fail("Bu satışa erişim yetkiniz yok");

        if (!_currentUser.CanViewAllSales && sale.UserId != _currentUser.UserId)
            return ApiResponse<SaleDto>.Fail("Bu satışa erişim yetkiniz yok");

        return ApiResponse<SaleDto>.Ok(MapToDto(sale));
    }

    public async Task<ApiResponse<PagedResponse<SaleDto>>> GetAllAsync(PagedRequest request)
    {
        var query = _unitOfWork.Sales.Query()
            .Include(s => s.Branch)
            .Include(s => s.Customer)
            .Include(s => s.User)
            .AsQueryable();

        if (!_currentUser.IsTenantAdmin && _currentUser.BranchId.HasValue)
            query = query.Where(s => s.BranchId == _currentUser.BranchId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(s =>
                s.SaleNumber.Contains(request.Search) ||
                (s.Customer != null && (s.Customer.FirstName.Contains(request.Search) ||
                                        s.Customer.LastName.Contains(request.Search) ||
                                        s.Customer.Phone.Contains(request.Search))));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.SaleDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return ApiResponse<PagedResponse<SaleDto>>.Ok(new PagedResponse<SaleDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }

    public async Task<ApiResponse<PagedResponse<SaleDto>>> GetMyTransactionsAsync(PagedRequest request)
    {
        var query = _unitOfWork.Sales.Query()
            .Include(s => s.Branch)
            .Include(s => s.Customer)
            .Include(s => s.User)
            .Where(s => s.UserId == _currentUser.UserId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.SaleDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return ApiResponse<PagedResponse<SaleDto>>.Ok(new PagedResponse<SaleDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }

    public async Task<ApiResponse<ScanBarcodeResponse>> ScanBarcodeAsync(string serialOrBarcode)
    {
        var serialNumber = await _unitOfWork.SerialNumbers.Query()
            .Include(sn => sn.Product)
            .Include(sn => sn.CurrentWarehouse)
            .FirstOrDefaultAsync(sn => sn.Serial == serialOrBarcode || sn.Product.Barcode == serialOrBarcode);

        if (serialNumber == null)
        {
            return ApiResponse<ScanBarcodeResponse>.Ok(new ScanBarcodeResponse
            {
                Found = false,
                IsAvailable = false,
                Message = "Ürün bulunamadı"
            });
        }

        if (serialNumber.Status != SerialNumberStatus.InStock)
        {
            var statusMessage = serialNumber.Status switch
            {
                SerialNumberStatus.Sold => "Bu ürün daha önce satılmış",
                SerialNumberStatus.InTransfer => "Bu ürün transferde",
                SerialNumberStatus.UsedInService => "Bu ürün serviste kullanılmış",
                _ => "Bu ürün satışa uygun değil"
            };

            return ApiResponse<ScanBarcodeResponse>.Ok(new ScanBarcodeResponse
            {
                Found = true,
                IsAvailable = false,
                Message = statusMessage
            });
        }

        if (_currentUser.BranchId.HasValue)
        {
            var branchWarehouse = await _unitOfWork.Warehouses.Query()
                .FirstOrDefaultAsync(w => w.BranchId == _currentUser.BranchId.Value);

            if (branchWarehouse != null && serialNumber.CurrentWarehouseId != branchWarehouse.Id)
            {
                return ApiResponse<ScanBarcodeResponse>.Ok(new ScanBarcodeResponse
                {
                    Found = true,
                    IsAvailable = false,
                    Message = "Bu ürün sizin deponuzda değil"
                });
            }
        }

        return ApiResponse<ScanBarcodeResponse>.Ok(new ScanBarcodeResponse
        {
            Found = true,
            IsAvailable = true,
            SerialInfo = new SerialNumberInfoDto
            {
                SerialNumberId = serialNumber.Id,
                ProductId = serialNumber.ProductId,
                Serial = serialNumber.Serial,
                IMEI = serialNumber.IMEI,
                ProductName = serialNumber.Product.Name,
                Barcode = serialNumber.Product.Barcode,
                Brand = serialNumber.Product.Brand,
                Model = serialNumber.Product.Model,
                SalePrice = serialNumber.Product.SalePrice,
                DiscountedPrice = serialNumber.Product.DiscountedPrice,
                VatRate = serialNumber.Product.VatRate,
                Status = serialNumber.Status.ToString(),
                CurrentWarehouse = serialNumber.CurrentWarehouse?.Name
            }
        });
    }

    public async Task<ApiResponse<SaleDto>> CreateAsync(CreateSaleRequest request)
    {
        if (!_currentUser.IsSalesStaff && !_currentUser.IsBranchAdmin && !_currentUser.IsTenantAdmin)
            return ApiResponse<SaleDto>.Fail("Satış yapma yetkiniz yok");

        if (!_currentUser.BranchId.HasValue)
            return ApiResponse<SaleDto>.Fail("Bayi tanımlı değil");

        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            Guid? customerId = request.CustomerId;
            if (!customerId.HasValue && request.NewCustomer != null)
            {
                var customer = new Customer
                {
                    TenantId = _currentUser.TenantId!.Value,
                    FirstName = request.NewCustomer.FirstName,
                    LastName = request.NewCustomer.LastName,
                    Phone = request.NewCustomer.Phone,
                    Email = request.NewCustomer.Email,
                    Address = request.NewCustomer.Address,
                    TcNo = request.NewCustomer.TcNo,
                    CreatedBy = _currentUser.UserId!.Value
                };
                await _unitOfWork.Customers.AddAsync(customer);
                customerId = customer.Id;
            }

            var saleNumber = await GenerateSaleNumber();

            var sale = new Sale
            {
                TenantId = _currentUser.TenantId!.Value,
                BranchId = _currentUser.BranchId.Value,
                SaleNumber = saleNumber,
                CustomerId = customerId,
                UserId = _currentUser.UserId!.Value,
                SaleDate = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                IsPaid = true,
                Notes = request.Notes,
                CreatedBy = _currentUser.UserId.Value
            };

            decimal subTotal = 0, vatTotal = 0, discountTotal = 0;

            foreach (var itemRequest in request.Items)
            {
                var serialNumber = await _unitOfWork.SerialNumbers.Query()
                    .Include(sn => sn.Product)
                    .FirstOrDefaultAsync(sn => sn.Serial == itemRequest.SerialOrBarcode);

                if (serialNumber == null)
                    throw new Exception($"Seri numarası bulunamadı: {itemRequest.SerialOrBarcode}");

                if (serialNumber.Status != SerialNumberStatus.InStock)
                    throw new Exception($"Ürün satışa uygun değil: {itemRequest.SerialOrBarcode}");

                var product = serialNumber.Product;
                var unitPrice = product.DiscountedPrice ?? product.SalePrice;
                var discount = itemRequest.DiscountAmount ?? 0;
                var vatAmount = (unitPrice - discount) * product.VatRate / 100;
                var totalPrice = unitPrice - discount + vatAmount;

                var saleItem = new SaleItem
                {
                    TenantId = _currentUser.TenantId!.Value,
                    SaleId = sale.Id,
                    ProductId = product.Id,
                    SerialNumberId = serialNumber.Id,
                    Barcode = product.Barcode,
                    Serial = serialNumber.Serial,
                    ProductName = product.Name,
                    UnitPrice = unitPrice,
                    VatRate = product.VatRate,
                    VatAmount = vatAmount,
                    DiscountAmount = discount,
                    TotalPrice = totalPrice,
                    CreatedBy = _currentUser.UserId.Value
                };

                sale.Items.Add(saleItem);
                subTotal += unitPrice;
                vatTotal += vatAmount;
                discountTotal += discount;

                serialNumber.Status = SerialNumberStatus.Sold;
                serialNumber.SoldAt = DateTime.UtcNow;
                serialNumber.SoldToCustomerId = customerId;
                serialNumber.SaleItemId = saleItem.Id;
                serialNumber.CurrentWarehouseId = null;
                _unitOfWork.SerialNumbers.Update(serialNumber);

                var stockMovement = new StockMovement
                {
                    TenantId = _currentUser.TenantId!.Value,
                    ProductId = product.Id,
                    SerialNumberId = serialNumber.Id,
                    Type = StockMovementType.SaleOut,
                    Quantity = -1,
                    ReferenceNumber = saleNumber,
                    ReferenceId = sale.Id,
                    CreatedBy = _currentUser.UserId.Value
                };
                await _unitOfWork.StockMovements.AddAsync(stockMovement);
            }

            sale.SubTotal = subTotal;
            sale.VatTotal = vatTotal;
            sale.DiscountTotal = discountTotal;
            sale.GrandTotal = subTotal - discountTotal + vatTotal;

            await _unitOfWork.Sales.AddAsync(sale);

            var cashRegister = await _unitOfWork.CashRegisters.Query()
                .FirstOrDefaultAsync(cr => cr.BranchId == _currentUser.BranchId.Value);

            if (cashRegister != null)
            {
                cashRegister.CurrentBalance += sale.GrandTotal;

                var cashMovement = new CashMovement
                {
                    TenantId = _currentUser.TenantId!.Value,
                    CashRegisterId = cashRegister.Id,
                    BranchId = _currentUser.BranchId.Value,
                    Type = CashMovementType.SaleIncome,
                    Amount = sale.GrandTotal,
                    BalanceAfter = cashRegister.CurrentBalance,
                    PaymentMethod = request.PaymentMethod,
                    ReferenceNumber = saleNumber,
                    ReferenceId = sale.Id,
                    Description = $"Satış: {saleNumber}",
                    CreatedBy = _currentUser.UserId.Value
                };

                await _unitOfWork.CashMovements.AddAsync(cashMovement);
                _unitOfWork.CashRegisters.Update(cashRegister);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var createdSale = await _unitOfWork.Sales.Query()
                .Include(s => s.Branch)
                .Include(s => s.Customer)
                .Include(s => s.User)
                .Include(s => s.Items).ThenInclude(i => i.Product)
                .FirstAsync(s => s.Id == sale.Id);

            return ApiResponse<SaleDto>.Ok(MapToDto(createdSale), "Satış başarıyla oluşturuldu");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<SaleDto>.Fail($"Satış oluşturulurken hata: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SaleSummaryDto>>> GetDailySummaryAsync(DateTime startDate, DateTime endDate, Guid? branchId = null)
    {
        var query = _unitOfWork.Sales.Query()
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate);

        if (!_currentUser.IsTenantAdmin && _currentUser.BranchId.HasValue)
            query = query.Where(s => s.BranchId == _currentUser.BranchId.Value);
        else if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value);

        var sales = await query.ToListAsync();

        var summary = sales
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new SaleSummaryDto
            {
                Date = g.Key,
                TotalSales = g.Count(),
                TotalAmount = g.Sum(s => s.GrandTotal),
                CashAmount = g.Where(s => s.PaymentMethod == PaymentMethod.Cash).Sum(s => s.GrandTotal),
                CardAmount = g.Where(s => s.PaymentMethod == PaymentMethod.CreditCard).Sum(s => s.GrandTotal)
            })
            .OrderBy(s => s.Date)
            .ToList();

        return ApiResponse<List<SaleSummaryDto>>.Ok(summary);
    }

    public async Task<ApiResponse<List<UserSaleSummaryDto>>> GetUserSummaryAsync(DateTime startDate, DateTime endDate, Guid? branchId = null)
    {
        var query = _unitOfWork.Sales.Query()
            .Include(s => s.User)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate);

        if (!_currentUser.IsTenantAdmin && _currentUser.BranchId.HasValue)
            query = query.Where(s => s.BranchId == _currentUser.BranchId.Value);
        else if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value);

        var sales = await query.ToListAsync();

        var summary = sales
            .GroupBy(s => new { s.UserId, s.User.FirstName, s.User.LastName })
            .Select(g => new UserSaleSummaryDto
            {
                UserId = g.Key.UserId,
                UserName = $"{g.Key.FirstName} {g.Key.LastName}",
                TotalSales = g.Count(),
                TotalAmount = g.Sum(s => s.GrandTotal)
            })
            .OrderByDescending(s => s.TotalAmount)
            .ToList();

        return ApiResponse<List<UserSaleSummaryDto>>.Ok(summary);
    }

    private async Task<string> GenerateSaleNumber()
    {
        var today = DateTime.UtcNow;
        var prefix = $"SLS-{today:yyyyMMdd}";

        var lastSale = await _unitOfWork.Sales.Query()
            .Where(s => s.SaleNumber.StartsWith(prefix))
            .OrderByDescending(s => s.SaleNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastSale != null)
        {
            var lastNumber = lastSale.SaleNumber.Split('-').LastOrDefault();
            if (int.TryParse(lastNumber, out int num))
                sequence = num + 1;
        }

        return $"{prefix}-{sequence:D4}";
    }

    private static SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            BranchId = sale.BranchId,
            BranchName = sale.Branch?.Name ?? "",
            CustomerId = sale.CustomerId,
            CustomerName = sale.Customer?.FullName,
            CustomerPhone = sale.Customer?.Phone,
            UserId = sale.UserId,
            UserName = sale.User?.FullName ?? "",
            SaleDate = sale.SaleDate,
            SubTotal = sale.SubTotal,
            VatTotal = sale.VatTotal,
            DiscountTotal = sale.DiscountTotal,
            GrandTotal = sale.GrandTotal,
            PaymentMethod = sale.PaymentMethod,
            PaymentMethodName = sale.PaymentMethod.ToString(),
            IsPaid = sale.IsPaid,
            Notes = sale.Notes,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Barcode = i.Barcode,
                Serial = i.Serial,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                VatRate = i.VatRate,
                VatAmount = i.VatAmount,
                DiscountAmount = i.DiscountAmount,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
