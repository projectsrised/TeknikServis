using Microsoft.EntityFrameworkCore.Storage;
using TeknikServis.DataAccess.Context;
using TeknikServis.Domain.Entities;

namespace TeknikServis.DataAccess.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<Tenant> Tenants { get; }
    IRepository<Branch> Branches { get; }
    IRepository<User> Users { get; }
    IRepository<Category> Categories { get; }
    IRepository<Product> Products { get; }
    IRepository<SerialNumber> SerialNumbers { get; }
    IRepository<Warehouse> Warehouses { get; }
    IRepository<Invoice> Invoices { get; }
    IRepository<InvoiceItem> InvoiceItems { get; }
    IRepository<Sale> Sales { get; }
    IRepository<SaleItem> SaleItems { get; }
    IRepository<Customer> Customers { get; }
    IRepository<TechnicalService> TechnicalServices { get; }
    IRepository<TechnicalServicePart> TechnicalServiceParts { get; }
    IRepository<TechnicalServiceStatusHistory> TechnicalServiceStatusHistories { get; }
    IRepository<Transfer> Transfers { get; }
    IRepository<TransferItem> TransferItems { get; }
    IRepository<Return> Returns { get; }
    IRepository<ReturnItem> ReturnItems { get; }
    IRepository<StockMovement> StockMovements { get; }
    IRepository<CashRegister> CashRegisters { get; }
    IRepository<CashMovement> CashMovements { get; }
    IRepository<InventoryCount> InventoryCounts { get; }
    IRepository<InventoryCountItem> InventoryCountItems { get; }
    IRepository<Employee> Employees { get; }
    IRepository<EmployeeSalary> EmployeeSalaries { get; }
    IRepository<EmployeeAdvance> EmployeeAdvances { get; }
    IRepository<EmployeeLeave> EmployeeLeaves { get; }
    IRepository<EmployeeOvertime> EmployeeOvertimes { get; }

    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly TeknikServisDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy loaded repositories
    private IRepository<Tenant>? _tenants;
    private IRepository<Branch>? _branches;
    private IRepository<User>? _users;
    private IRepository<Category>? _categories;
    private IRepository<Product>? _products;
    private IRepository<SerialNumber>? _serialNumbers;
    private IRepository<Warehouse>? _warehouses;
    private IRepository<Invoice>? _invoices;
    private IRepository<InvoiceItem>? _invoiceItems;
    private IRepository<Sale>? _sales;
    private IRepository<SaleItem>? _saleItems;
    private IRepository<Customer>? _customers;
    private IRepository<TechnicalService>? _technicalServices;
    private IRepository<TechnicalServicePart>? _technicalServiceParts;
    private IRepository<TechnicalServiceStatusHistory>? _technicalServiceStatusHistories;
    private IRepository<Transfer>? _transfers;
    private IRepository<TransferItem>? _transferItems;
    private IRepository<Return>? _returns;
    private IRepository<ReturnItem>? _returnItems;
    private IRepository<StockMovement>? _stockMovements;
    private IRepository<CashRegister>? _cashRegisters;
    private IRepository<CashMovement>? _cashMovements;
    private IRepository<InventoryCount>? _inventoryCounts;
    private IRepository<InventoryCountItem>? _inventoryCountItems;
    private IRepository<Employee>? _employees;
    private IRepository<EmployeeSalary>? _employeeSalaries;
    private IRepository<EmployeeAdvance>? _employeeAdvances;
    private IRepository<EmployeeLeave>? _employeeLeaves;
    private IRepository<EmployeeOvertime>? _employeeOvertimes;

    public UnitOfWork(TeknikServisDbContext context)
    {
        _context = context;
    }

    public IRepository<Tenant> Tenants => _tenants ??= new Repository<Tenant>(_context);
    public IRepository<Branch> Branches => _branches ??= new Repository<Branch>(_context);
    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<Product> Products => _products ??= new Repository<Product>(_context);
    public IRepository<SerialNumber> SerialNumbers => _serialNumbers ??= new Repository<SerialNumber>(_context);
    public IRepository<Warehouse> Warehouses => _warehouses ??= new Repository<Warehouse>(_context);
    public IRepository<Invoice> Invoices => _invoices ??= new Repository<Invoice>(_context);
    public IRepository<InvoiceItem> InvoiceItems => _invoiceItems ??= new Repository<InvoiceItem>(_context);
    public IRepository<Sale> Sales => _sales ??= new Repository<Sale>(_context);
    public IRepository<SaleItem> SaleItems => _saleItems ??= new Repository<SaleItem>(_context);
    public IRepository<Customer> Customers => _customers ??= new Repository<Customer>(_context);
    public IRepository<TechnicalService> TechnicalServices => _technicalServices ??= new Repository<TechnicalService>(_context);
    public IRepository<TechnicalServicePart> TechnicalServiceParts => _technicalServiceParts ??= new Repository<TechnicalServicePart>(_context);
    public IRepository<TechnicalServiceStatusHistory> TechnicalServiceStatusHistories => _technicalServiceStatusHistories ??= new Repository<TechnicalServiceStatusHistory>(_context);
    public IRepository<Transfer> Transfers => _transfers ??= new Repository<Transfer>(_context);
    public IRepository<TransferItem> TransferItems => _transferItems ??= new Repository<TransferItem>(_context);
    public IRepository<Return> Returns => _returns ??= new Repository<Return>(_context);
    public IRepository<ReturnItem> ReturnItems => _returnItems ??= new Repository<ReturnItem>(_context);
    public IRepository<StockMovement> StockMovements => _stockMovements ??= new Repository<StockMovement>(_context);
    public IRepository<CashRegister> CashRegisters => _cashRegisters ??= new Repository<CashRegister>(_context);
    public IRepository<CashMovement> CashMovements => _cashMovements ??= new Repository<CashMovement>(_context);
    public IRepository<InventoryCount> InventoryCounts => _inventoryCounts ??= new Repository<InventoryCount>(_context);
    public IRepository<InventoryCountItem> InventoryCountItems => _inventoryCountItems ??= new Repository<InventoryCountItem>(_context);
    public IRepository<Employee> Employees => _employees ??= new Repository<Employee>(_context);
    public IRepository<EmployeeSalary> EmployeeSalaries => _employeeSalaries ??= new Repository<EmployeeSalary>(_context);
    public IRepository<EmployeeAdvance> EmployeeAdvances => _employeeAdvances ??= new Repository<EmployeeAdvance>(_context);
    public IRepository<EmployeeLeave> EmployeeLeaves => _employeeLeaves ??= new Repository<EmployeeLeave>(_context);
    public IRepository<EmployeeOvertime> EmployeeOvertimes => _employeeOvertimes ??= new Repository<EmployeeOvertime>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
