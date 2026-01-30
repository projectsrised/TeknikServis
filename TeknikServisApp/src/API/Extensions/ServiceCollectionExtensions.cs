using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Application.Services;
using TeknikServisApp.Infrastructure.Data;
using TeknikServisApp.Infrastructure.Repositories;

namespace TeknikServisApp.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMarkaService, MarkaService>();
        services.AddScoped<IModelService, ModelService>();
        services.AddScoped<IKategoriService, KategoriService>();
        services.AddScoped<IUrunService, UrunService>();
        services.AddScoped<IBayiService, BayiService>();
        services.AddScoped<IDepoService, DepoService>();
        services.AddScoped<IMusteriService, MusteriService>();
        services.AddScoped<IKasaService, KasaService>();
        services.AddScoped<IPersonelService, PersonelService>();
        services.AddScoped<ISatisService, SatisService>();
        services.AddScoped<ITeknikServisService, TeknikServisService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IIadeService, IadeService>();
        services.AddScoped<IFaturaService, FaturaService>();
        services.AddScoped<ISayimService, SayimService>();
        services.AddScoped<IStokService, StokService>();
        services.AddScoped<IRaporService, RaporService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<TenantProvider>();
        services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<TenantProvider>());
        services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<TenantProvider>());

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Teknik Servis API",
                Version = "v1",
                Description = "Multi-tenant phone repair and sales management API"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
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

        return services;
    }
}
