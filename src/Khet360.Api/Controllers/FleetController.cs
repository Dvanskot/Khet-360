using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FleetController : ControllerBase
{
    private readonly IFleetService _fleetService;

    public FleetController(IFleetService fleetService)
    {
        _fleetService = fleetService;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] VehicleCreateDto dto, [FromQuery] Guid branchId)
    {
        var id = await _fleetService.RegisterVehicleAsync(dto, branchId);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var vehicle = await _fleetService.GetVehicleAsync(id);
        if (vehicle == null) return NotFound();
        return Ok(vehicle);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] VehicleUpdateDto dto)
    {
        await _fleetService.UpdateVehicleStatusAsync(id, dto);
        return NoContent();
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] Guid branchId)
    {
        var vehicles = await _fleetService.GetAvailableVehiclesAsync(branchId);
        return Ok(vehicles);
    }
}
