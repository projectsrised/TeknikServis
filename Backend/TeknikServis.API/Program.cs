using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TeknikServis.API.Middlewares;
using TeknikServis.Business.Interfaces;
using TeknikServis.Business.Services;
using TeknikServis.DataAccess.Context;
using TeknikServis.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<TeknikServisDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// DbContext Factory for multi-tenant
builder.Services.AddScoped<Func<Guid?, Guid?, TeknikServisDbContext>>(provider =>
{
    return (tenantId, branchId) =>
    {
        var options = new DbContextOptionsBuilder<TeknikServisDbContext>()
            .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .Options;
        return new TeknikServisDbContext(options, tenantId, branchId);
    };
});

// Repository & UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Current User Service
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Business Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISaleService, SaleService>();
// builder.Services.AddScoped<ICategoryService, CategoryService>();
// builder.Services.AddScoped<IProductService, ProductService>();
// builder.Services.AddScoped<IInvoiceService, InvoiceService>();
// builder.Services.AddScoped<ITechnicalServiceService, TechnicalServiceService>();
// builder.Services.AddScoped<ITransferService, TransferService>();
// builder.Services.AddScoped<IWarehouseService, WarehouseService>();
// builder.Services.AddScoped<IReturnService, ReturnService>();
// builder.Services.AddScoped<IInventoryCountService, InventoryCountService>();
// builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// builder.Services.AddScoped<ISalaryService, SalaryService>();
// builder.Services.AddScoped<IAdvanceService, AdvanceService>();
// builder.Services.AddScoped<ILeaveService, LeaveService>();
// builder.Services.AddScoped<ICashService, CashService>();
// builder.Services.AddScoped<IDashboardService, DashboardService>();
// builder.Services.AddScoped<IReportService, ReportService>();
// builder.Services.AddScoped<ICustomerService, CustomerService>();
// builder.Services.AddScoped<ITenantService, TenantService>();
// builder.Services.AddScoped<IBranchService, BranchService>();
// builder.Services.AddScoped<IUserService, UserService>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Teknik Servis API", 
        Version = "v1",
        Description = "Telefon Teknik Servis, Aksesuar ve Satış Uygulaması API"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TeknikServisDbContext>();
    db.Database.Migrate();
}

app.Run();
