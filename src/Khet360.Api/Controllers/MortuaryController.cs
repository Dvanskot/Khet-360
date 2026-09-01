using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MortuaryController : ControllerBase
{
    private readonly IMortuaryService _mortuaryService;

    public MortuaryController(IMortuaryService mortuaryService)
    {
        _mortuaryService = mortuaryService;
    }

    [HttpPost("slots")]
    public async Task<IActionResult> CreateSlot([FromBody] SlotCreateRequest request)
    {
        var id = await _mortuaryService.CreateSlotAsync(request.SlotNumber, request.IsColdStorage, request.BranchId);
        return Ok(id);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] Guid branchId, [FromQuery] bool requireColdStorage = false)
    {
        var slots = await _mortuaryService.GetAvailableSlotsAsync(branchId, requireColdStorage);
        return Ok(slots);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign([FromBody] SlotAssignmentDto dto)
    {
        await _mortuaryService.AssignSlotAsync(dto);
        return NoContent();
    }

    [HttpPost("release")]
    public async Task<IActionResult> Release([FromBody] SlotReleaseDto dto)
    {
        await _mortuaryService.ReleaseSlotAsync(dto);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var slot = await _mortuaryService.GetSlotAsync(id);
        if (slot == null) return NotFound();
        return Ok(slot);
    }
}

public record SlotCreateRequest(string SlotNumber, bool IsColdStorage, Guid BranchId);
