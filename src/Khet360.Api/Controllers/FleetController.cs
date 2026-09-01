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

    [HttpPost("register")]
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

    [HttpGet("{id}/efficiency")]
    public async Task<IActionResult> GetEfficiency(Guid id)
    {
        var efficiency = await _fleetService.CalculateFuelEfficiencyAsync(id);
        return Ok(new { VehicleId = id, Efficiency = efficiency });
    }

    [HttpGet("maintenance-due")]
    public async Task<IActionResult> GetMaintenanceDue([FromQuery] Guid branchId)
    {
        var vehicles = await _fleetService.GetVehiclesRequiringMaintenanceAsync(branchId);
        return Ok(vehicles);
    }

    [HttpPost("assign-trip")]
    public async Task<IActionResult> AssignTrip([FromBody] TripAssignmentDto dto)
    {
        await _fleetService.AssignTripAsync(dto.VehicleId, dto.DriverId, dto.FuneralCaseId, dto.RouteDetails);
        return Ok("Trip assigned successfully.");
    }
}
