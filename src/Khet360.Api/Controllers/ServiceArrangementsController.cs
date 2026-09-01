using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceArrangementsController : ControllerBase
{
    private readonly IServiceArrangementService _serviceArrangementService;

    public ServiceArrangementsController(IServiceArrangementService serviceArrangementService)
    {
        _serviceArrangementService = serviceArrangementService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ServiceArrangementCreateDto dto, [FromQuery] Guid branchId)
    {
        var id = await _serviceArrangementService.CreateArrangementAsync(dto, branchId);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var arrangement = await _serviceArrangementService.GetArrangementAsync(id);
        if (arrangement == null) return NotFound();
        return Ok(arrangement);
    }

    [HttpGet("case/{funeralCaseId}")]
    public async Task<IActionResult> GetByCase(Guid funeralCaseId)
    {
        var arrangements = await _serviceArrangementService.GetArrangementsByCaseAsync(funeralCaseId);
        return Ok(arrangements);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ServiceArrangementUpdateDto dto)
    {
        await _serviceArrangementService.UpdateArrangementAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _serviceArrangementService.DeleteArrangementAsync(id);
        return NoContent();
    }
}
