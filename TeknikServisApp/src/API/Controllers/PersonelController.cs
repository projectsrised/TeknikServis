using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/personel")]
public class PersonelController : ControllerBase
{
    private readonly IPersonelService _personelService;

    public PersonelController(IPersonelService personelService)
    {
        _personelService = personelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? bayiId = null, [FromQuery] bool? aktif = null)
    {
        var result = await _personelService.GetListAsync(page, pageSize, bayiId, aktif);
        return Ok(new { basarili = true, data = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var personel = await _personelService.GetByIdAsync(id);
        return personel != null
            ? Ok(new { basarili = true, data = personel })
            : NotFound(new { basarili = false, mesaj = "Personel bulunamadı" });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Create([FromBody] PersonelCreateDto dto)
    {
        var result = await _personelService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PersonelUpdateDto dto)
    {
        var result = await _personelService.UpdateAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _personelService.DeleteAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    // Maaş işlemleri
    [HttpGet("{id}/maas-hesapla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> MaasHesapla(Guid id, [FromQuery] int yil, [FromQuery] int ay)
    {
        var result = await _personelService.MaasHesaplaAsync(id, yil, ay);
        return Ok(new { basarili = true, data = result });
    }

    [HttpPost("{id}/maas-ode")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> MaasOde(Guid id, [FromQuery] int yil, [FromQuery] int ay)
    {
        var result = await _personelService.MaasOdeAsync(id, yil, ay);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}/maas-odemeleri")]
    public async Task<IActionResult> GetMaasOdemeleri(Guid id)
    {
        var result = await _personelService.GetMaasOdemeleriAsync(id);
        return Ok(new { basarili = true, data = result });
    }

    // Avans işlemleri
    [HttpPost("{id}/avans-talep")]
    public async Task<IActionResult> AvansTalepEt(Guid id, [FromBody] AvansTalepRequest request)
    {
        var result = await _personelService.AvansTalepEtAsync(id, request.Tutar);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("avans/{avansId}/onayla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> AvansOnayla(Guid avansId, [FromQuery] bool onayla = true)
    {
        var result = await _personelService.AvansOnaylaAsync(avansId, onayla);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}/avanslar")]
    public async Task<IActionResult> GetAvanslar(Guid id)
    {
        var result = await _personelService.GetAvanslarAsync(id);
        return Ok(new { basarili = true, data = result });
    }

    // İzin işlemleri
    [HttpPost("izin-talep")]
    public async Task<IActionResult> IzinTalepEt([FromBody] PersonelIzinCreateDto dto)
    {
        var result = await _personelService.IzinTalepEtAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("izin/{izinId}/onayla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> IzinOnayla(Guid izinId, [FromQuery] bool onayla = true, [FromBody] IptalRequest? request = null)
    {
        var result = await _personelService.IzinOnaylaAsync(izinId, onayla, request?.Neden);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}/izinler")]
    public async Task<IActionResult> GetIzinler(Guid id)
    {
        var result = await _personelService.GetIzinlerAsync(id);
        return Ok(new { basarili = true, data = result });
    }

    [HttpGet("{id}/kalan-izin")]
    public async Task<IActionResult> KalanIzinGunu(Guid id, [FromQuery] int? yil = null)
    {
        var kalanGun = await _personelService.KalanIzinGunuAsync(id, yil ?? DateTime.UtcNow.Year);
        return Ok(new { basarili = true, data = new { PersonelId = id, KalanIzinGunu = kalanGun } });
    }

    // Mesai işlemleri
    [HttpPost("mesai")]
    public async Task<IActionResult> MesaiKaydet([FromBody] PersonelMesaiCreateDto dto)
    {
        var result = await _personelService.MesaiKaydetAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPost("mesai/{mesaiId}/onayla")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> MesaiOnayla(Guid mesaiId)
    {
        var result = await _personelService.MesaiOnaylaAsync(mesaiId);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}/mesailer")]
    public async Task<IActionResult> GetMesailer(Guid id, [FromQuery] int? yil = null, [FromQuery] int? ay = null)
    {
        var result = await _personelService.GetMesailerAsync(id, yil, ay);
        return Ok(new { basarili = true, data = result });
    }
}

public class AvansTalepRequest
{
    public decimal Tutar { get; set; }
}
