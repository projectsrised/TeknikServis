using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var result = await _authService.LoginAsync(request);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _authService.ChangePasswordAsync(userId, request);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _authService.GetCurrentUserAsync(userId);
        return user != null ? Ok(user) : NotFound();
    }
[HttpGet("test-hash")]
[AllowAnonymous]
public IActionResult TestHash()
{
    var password = "Admin123!";
    var hash = BCrypt.Net.BCrypt.HashPassword(password);
    var verify = BCrypt.Net.BCrypt.Verify(password, hash);
    
    return Ok(new { 
        hash, 
        verify,
        message = "Bu hash'i veritabanına kopyala"
    });
}
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}
