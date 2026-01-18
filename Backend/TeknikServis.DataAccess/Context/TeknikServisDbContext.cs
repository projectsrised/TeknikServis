using Microsoft.EntityFrameworkCore;
using TeknikServis.Domain.Entities;

namespace TeknikServis.DataAccess.Context;

public class TeknikServisDbContext : DbContext
{
    private readonly Guid? _tenantId;
    private readonly Guid? _branchId;

    public TeknikServisDbContext(DbContextOptions<TeknikServisDbContext> options) : base(options)
    {
    }

    public TeknikServisDbContext(DbContextOptions<TeknikServisDbContext> options, Guid? tenantId, Guid? branchId) 
        : base(options)
    {
        _tenantId = tenantId;
        _branchId = branchId;
    }

    // Multi-Tenant
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<User> Users => Set<User>();

    // Kategori & Ürün
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SerialNumber> SerialNumbers => Set<SerialNumber>();

    // Fatura
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    // Satış
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    // Müşteri
    public DbSet<Customer> Customers => Set<Customer>();

    // Teknik Servis
    public DbSet<TechnicalService> TechnicalServices => Set<TechnicalService>();
    public DbSet<TechnicalServicePart> TechnicalServiceParts => Set<TechnicalServicePart>();
    public DbSet<TechnicalServiceStatusHistory> TechnicalServiceStatusHistories => Set<TechnicalServiceStatusHistory>();

    // Transfer
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<TransferItem> TransferItems => Set<TransferItem>();

    // İade
    public DbSet<Return> Returns => Set<Return>();
    public DbSet<ReturnItem> ReturnItems => Set<ReturnItem>();

    // Depo & Stok
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    // Kasa
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CashMovement> CashMovements => Set<CashMovement>();

    // Sayım
    public DbSet<InventoryCount> InventoryCounts => Set<InventoryCount>();
    public DbSet<InventoryCountItem> InventoryCountItems => Set<InventoryCountItem>();

    // Personel
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeSalary> EmployeeSalaries => Set<EmployeeSalary>();
    public DbSet<EmployeeAdvance> EmployeeAdvances => Set<EmployeeAdvance>();
    public DbSet<EmployeeLeave> EmployeeLeaves => Set<EmployeeLeave>();
    public DbSet<EmployeeOvertime> EmployeeOvertimes => Set<EmployeeOvertime>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global Query Filters for Multi-Tenancy
        modelBuilder.Entity<Branch>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Category>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<SerialNumber>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Invoice>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Sale>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Customer>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<TechnicalService>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Transfer>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Return>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Warehouse>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<StockMovement>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<CashRegister>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<CashMovement>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<InventoryCount>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));
        modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted && (_tenantId == null || x.TenantId == _tenantId));

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TeknikServisDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Set TenantId for new tenant entities
        var tenantEntries = ChangeTracker.Entries<TenantEntity>();
        foreach (var entry in tenantEntries)
        {
            if (entry.State == EntityState.Added && _tenantId.HasValue)
            {
                entry.Entity.TenantId = _tenantId.Value;
            }
        }

        // Set BranchId for new branch entities
        var branchEntries = ChangeTracker.Entries<BranchEntity>();
        foreach (var entry in branchEntries)
        {
            if (entry.State == EntityState.Added && _branchId.HasValue)
            {
                entry.Entity.BranchId = _branchId.Value;
            }
        }
    }
}
