using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TeknikServis.Business.DTOs;
using TeknikServis.Business.Interfaces;
using TeknikServis.DataAccess.Repositories;
using TeknikServis.Domain.Entities;
using BC = BCrypt.Net.BCrypt;

namespace TeknikServis.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.Query()
            .Include(u => u.Tenant)
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);

        if (user == null)
            return ApiResponse<LoginResponse>.Fail("Geçersiz email veya şifre");

        if (!BC.Verify(request.Password, user.PasswordHash))
            return ApiResponse<LoginResponse>.Fail("Geçersiz email veya şifre");

        if (!user.IsActive)
            return ApiResponse<LoginResponse>.Fail("Kullanıcı hesabı aktif değil");

        if (!user.Tenant.IsActive)
            return ApiResponse<LoginResponse>.Fail("Firma hesabı aktif değil");

        if (user.Tenant.SubscriptionEndDate.HasValue && user.Tenant.SubscriptionEndDate < DateTime.UtcNow)
            return ApiResponse<LoginResponse>.Fail("Firma aboneliği sona ermiş");

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.LastLoginAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = MapToUserDto(user)
        });
    }

    public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _unitOfWork.Users.Query()
            .Include(u => u.Tenant)
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && !u.IsDeleted);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return ApiResponse<LoginResponse>.Fail("Geçersiz veya süresi dolmuş refresh token");

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = MapToUserDto(user)
        });
    }

    public async Task<ApiResponse<bool>> LogoutAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse<bool>.Fail("Kullanıcı bulunamadı");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Çıkış yapıldı");
    }

    public async Task<ApiResponse<UserDto>> GetCurrentUserAsync()
    {
        if (!_currentUser.UserId.HasValue)
            return ApiResponse<UserDto>.Fail("Kullanıcı oturumu bulunamadı");

        var user = await _unitOfWork.Users.Query()
            .Include(u => u.Tenant)
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId.Value);

        if (user == null)
            return ApiResponse<UserDto>.Fail("Kullanıcı bulunamadı");

        return ApiResponse<UserDto>.Ok(MapToUserDto(user));
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse<bool>.Fail("Kullanıcı bulunamadı");

        if (!BC.Verify(request.CurrentPassword, user.PasswordHash))
            return ApiResponse<bool>.Fail("Mevcut şifre hatalı");

        user.PasswordHash = BC.HashPassword(request.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Şifre başarıyla değiştirildi");
    }

    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("TenantId", user.TenantId.ToString()),
        };

        if (user.BranchId.HasValue)
            claims.Add(new Claim("BranchId", user.BranchId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
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

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            BranchId = user.BranchId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            RoleName = user.Role.ToString(),
            TenantName = user.Tenant?.Name,
            BranchName = user.Branch?.Name,
            IsActive = user.IsActive
        };
    }
}
