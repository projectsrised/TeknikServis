using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;
using TeknikServisApp.Domain.Entities;

namespace TeknikServisApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
        _configuration = configuration;
    }

    public async Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginDto request)
    {
        var kullanici = await _unitOfWork.Repository<Kullanici>()
            .Query()
             .IgnoreQueryFilters()
            .Include(k => k.Tenant)
            .Include(k => k.Bayi)
            .FirstOrDefaultAsync(k => k.Email == request.Email && !k.Silindi);

        if (kullanici == null)
            return ApiResponseDto<LoginResponseDto>.Fail("Email veya şifre hatalı");

        if (!BCrypt.Net.BCrypt.Verify(request.Sifre, kullanici.SifreHash))
            return ApiResponseDto<LoginResponseDto>.Fail("Email veya şifre hatalı");

        if (!kullanici.Aktif)
            return ApiResponseDto<LoginResponseDto>.Fail("Hesabınız aktif değil");

        kullanici.SonGirisTarihi = DateTime.UtcNow;

        var accessToken = GenerateAccessToken(kullanici);
        var refreshToken = GenerateRefreshToken();

        kullanici.RefreshToken = refreshToken;
        kullanici.RefreshTokenSonKullanma = DateTime.UtcNow.AddDays(7);

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<LoginResponseDto>.Ok(new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Kullanici = MapToDto(kullanici)
        }, "Giriş başarılı");
    }

    public async Task<ApiResponseDto<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto request)
    {
        var kullanici = await _unitOfWork.Repository<Kullanici>()
            .Query()
             .IgnoreQueryFilters()
            .Include(k => k.Tenant)
            .Include(k => k.Bayi)
            .FirstOrDefaultAsync(k => k.RefreshToken == request.RefreshToken && !k.Silindi);

        if (kullanici == null || kullanici.RefreshTokenSonKullanma < DateTime.UtcNow)
            return ApiResponseDto<LoginResponseDto>.Fail("Geçersiz veya süresi dolmuş token");

        var accessToken = GenerateAccessToken(kullanici);
        var refreshToken = GenerateRefreshToken();

        kullanici.RefreshToken = refreshToken;
        kullanici.RefreshTokenSonKullanma = DateTime.UtcNow.AddDays(7);

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<LoginResponseDto>.Ok(new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Kullanici = MapToDto(kullanici)
        });
    }

    public async Task<ApiResponseDto<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto request)
    {
        var kullanici = await _unitOfWork.Repository<Kullanici>().GetByIdAsync(userId);
        if (kullanici == null)
            return ApiResponseDto<bool>.Fail("Kullanıcı bulunamadı");

        if (!BCrypt.Net.BCrypt.Verify(request.MevcutSifre, kullanici.SifreHash))
            return ApiResponseDto<bool>.Fail("Mevcut şifre hatalı");

        kullanici.SifreHash = BCrypt.Net.BCrypt.HashPassword(request.YeniSifre);
        kullanici.RefreshToken = null;
        kullanici.RefreshTokenSonKullanma = null;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<bool>.Ok(true, "Şifre değiştirildi");
    }

    public async Task<KullaniciDto?> GetCurrentUserAsync(Guid userId)
    {
        var kullanici = await _unitOfWork.Repository<Kullanici>()
            .Query()
            .Include(k => k.Tenant)
            .Include(k => k.Bayi)
            .FirstOrDefaultAsync(k => k.Id == userId && !k.Silindi);

        return kullanici == null ? null : MapToDto(kullanici);
    }

    public async Task<ApiResponseDto<KullaniciDto>> RegisterAsync(RegisterDto request)
    {
        var exists = await _unitOfWork.Repository<Kullanici>()
            .AnyAsync(k => k.Email == request.Email && !k.Silindi);

        if (exists)
            return ApiResponseDto<KullaniciDto>.Fail("Bu email zaten kayıtlı");

        var kullanici = new Kullanici
        {
            TenantId = _tenantProvider.TenantId,
            BayiId = request.BayiId,
            Ad = request.Ad,
            Soyad = request.Soyad,
            Email = request.Email,
            SifreHash = BCrypt.Net.BCrypt.HashPassword(request.Sifre),
            Telefon = request.Telefon,
            Rol = request.Rol
        };

        await _unitOfWork.Repository<Kullanici>().AddAsync(kullanici);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<KullaniciDto>.Ok(MapToDto(kullanici), "Kullanıcı oluşturuldu");
    }

    private string GenerateAccessToken(Kullanici kullanici)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
            new(ClaimTypes.Email, kullanici.Email),
            new(ClaimTypes.Name, kullanici.AdSoyad),
            new(ClaimTypes.Role, kullanici.Rol.ToString()),
            new("TenantId", kullanici.TenantId.ToString())
        };

        if (kullanici.BayiId.HasValue)
            claims.Add(new Claim("BayiId", kullanici.BayiId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static KullaniciDto MapToDto(Kullanici k) => new()
    {
        Id = k.Id,
        TenantId = k.TenantId,
        TenantAd = k.Tenant?.Ad,
        BayiId = k.BayiId,
        BayiAd = k.Bayi?.Ad,
        Ad = k.Ad,
        Soyad = k.Soyad,
        AdSoyad = k.AdSoyad,
        Email = k.Email,
        Telefon = k.Telefon,
        Rol = k.Rol,
        RolAd = k.Rol.ToString(),
        Aktif = k.Aktif
    };
}
