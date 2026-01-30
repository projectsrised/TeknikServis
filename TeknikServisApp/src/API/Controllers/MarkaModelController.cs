using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeknikServisApp.Application.DTOs;
using TeknikServisApp.Application.Interfaces;

namespace TeknikServisApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/marka")]
public class MarkaController : ControllerBase
{
    private readonly IMarkaService _markaService;

    public MarkaController(IMarkaService markaService)
    {
        _markaService = markaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var markalar = await _markaService.GetAllAsync();
        return Ok(new { basarili = true, data = markalar });
    }

    [HttpGet("with-models")]
    public async Task<IActionResult> GetAllWithModels()
    {
        var markalar = await _markaService.GetAllWithModelsAsync();
        return Ok(new { basarili = true, data = markalar });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var marka = await _markaService.GetByIdAsync(id);
        return marka != null
            ? Ok(new { basarili = true, data = marka })
            : NotFound(new { basarili = false, mesaj = "Marka bulunamadı" });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Create([FromBody] MarkaCreateDto dto)
    {
        var result = await _markaService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] MarkaUpdateDto dto)
    {
        var result = await _markaService.UpdateAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _markaService.DeleteAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}

[Authorize]
[ApiController]
[Route("api/model")]
public class ModelController : ControllerBase
{
    private readonly IModelService _modelService;

    public ModelController(IModelService modelService)
    {
        _modelService = modelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var modeller = await _modelService.GetAllAsync();
        return Ok(new { basarili = true, data = modeller });
    }

    [HttpGet("marka/{markaId}")]
    public async Task<IActionResult> GetByMarkaId(Guid markaId)
    {
        var modeller = await _modelService.GetByMarkaIdAsync(markaId);
        return Ok(new { basarili = true, data = modeller });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var model = await _modelService.GetByIdAsync(id);
        return model != null
            ? Ok(new { basarili = true, data = model })
            : NotFound(new { basarili = false, mesaj = "Model bulunamadı" });
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Create([FromBody] ModelCreateDto dto)
    {
        var result = await _modelService.CreateAsync(dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ModelUpdateDto dto)
    {
        var result = await _modelService.UpdateAsync(id, dto);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin,BayiAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _modelService.DeleteAsync(id);
        return result.Basarili ? Ok(result) : BadRequest(result);
    }
}
