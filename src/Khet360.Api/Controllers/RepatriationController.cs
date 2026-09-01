using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepatriationController : ControllerBase
{
    private readonly IRepatriationService _repatriationService;

    public RepatriationController(IRepatriationService repatriationService)
    {
        _repatriationService = repatriationService;
    }

    [HttpPost]
    public async Task<IActionResult> Request([FromBody] RepatriationCreateDto dto, [FromQuery] Guid branchId)
    {
        var id = await _repatriationService.RequestRepatriationAsync(dto, branchId);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var repatriation = await _repatriationService.GetRepatriationAsync(id);
        if (repatriation == null) return NotFound();
        return Ok(repatriation);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] RepatriationUpdateDto dto)
    {
        await _repatriationService.UpdateRepatriationStatusAsync(id, dto);
        return NoContent();
    }

    [HttpGet("case/{funeralCaseId}")]
    public async Task<IActionResult> GetByCase(Guid funeralCaseId)
    {
        var repatriations = await _repatriationService.GetRepatriationsByCaseAsync(funeralCaseId);
        return Ok(repatriations);
    }
}
