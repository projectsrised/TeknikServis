using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/kategori")]
public class KategoriController : ControllerBase
{
    private readonly IKategoriService _kategoriService;

    public KategoriController(IKategoriService kategoriService)
    {
        _kategoriService = kategoriService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var kategoriler = await _kategoriService.GetAllAsync();
        return Ok(new { basarili = true, data = kategoriler });
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var tree = await _kategoriService.GetTreeAsync();
        return Ok(new { basarili = true, data = tree });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var kategori = await _kategoriService.GetByIdAsync(id);
        return kategori != null
            ? Ok(new { basarili = true, data = kategori })
            : NotFound(new { basarili = false, mesaj = "Kategori bulunamadı" });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Create([FromBody] KategoriCreateDto dto)
    {
        var result = await _kategoriService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] KategoriUpdateDto dto)
    {
        var result = await _kategoriService.UpdateAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _kategoriService.DeleteAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

[Authorize]
[ApiController]
[Route("api/urun")]
public class UrunController : ControllerBase
{
    private readonly IUrunService _urunService;

    public UrunController(IUrunService urunService)
    {
        _urunService = urunService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? kategoriId = null, [FromQuery] bool? aktif = null, [FromQuery] string? arama = null)
    {
        var result = await _urunService.GetListAsync(page, pageSize, kategoriId, aktif, arama);
        return Ok(new { basarili = true, data = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var urun = await _urunService.GetByIdAsync(id);
        return urun != null
            ? Ok(new { basarili = true, data = urun })
            : NotFound(new { basarili = false, mesaj = "Ürün bulunamadı" });
    }

    [HttpGet("barkod/{barkod}")]
    public async Task<IActionResult> GetByBarkod(string barkod)
    {
        var urun = await _urunService.GetByBarkodAsync(barkod);
        return urun != null
            ? Ok(new { basarili = true, data = urun })
            : NotFound(new { basarili = false, mesaj = "Ürün bulunamadı" });
    }

    [HttpGet("{id}/seri-numaralari")]
    public async Task<IActionResult> GetSeriNumaralari(Guid id, [FromQuery] Guid? depoId = null)
    {
        var seriler = await _urunService.GetSeriNumaralariAsync(id, depoId);
        return Ok(new { basarili = true, data = seriler });
    }

    [HttpGet("{id}/stok")]
    public async Task<IActionResult> GetStok(Guid id, [FromQuery] Guid? depoId = null)
    {
        var stok = await _urunService.GetStokAsync(id, depoId);
        return Ok(new { basarili = true, data = new { UrunId = id, Stok = stok } });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Create([FromBody] UrunCreateDto dto)
    {
        var result = await _urunService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UrunUpdateDto dto)
    {
        var result = await _urunService.UpdateAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _urunService.DeleteAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}
