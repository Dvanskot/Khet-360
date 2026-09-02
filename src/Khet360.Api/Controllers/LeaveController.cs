using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpGet("types")]
    public async Task<ActionResult<List<LeaveTypeDto>>> GetLeaveTypes()
    {
        return Ok(await _leaveService.GetLeaveTypesAsync());
    }

    [HttpPost("types")]
    public async Task<ActionResult<Guid>> CreateLeaveType([FromBody] LeaveTypeCreateDto dto)
    {
        var id = await _leaveService.CreateLeaveTypeAsync(dto);
        return Ok(id);
    }

    [HttpGet("balances/{employeeId}")]
    public async Task<ActionResult<List<LeaveBalanceDto>>> GetBalances(Guid employeeId)
    {
        return Ok(await _leaveService.GetEmployeeBalancesAsync(employeeId));
    }

    [HttpPost("balances/adjust")]
    public async Task<IActionResult> AdjustBalance([FromBody] BalanceAdjustmentDto dto)
    {
        await _leaveService.AdjustBalanceAsync(dto.EmployeeId, dto.LeaveTypeId, dto.Adjustment);
        return NoContent();
    }

    [HttpPost("applications")]
    public async Task<ActionResult<Guid>> ApplyForLeave([FromBody] LeaveApplicationCreateDto dto)
    {
        var id = await _leaveService.ApplyForLeaveAsync(dto);
        return CreatedAtAction(nameof(GetApplication), new { id }, id);
    }

    [HttpGet("applications/{id}")]
    public async Task<ActionResult<LeaveApplicationDto>> GetApplication(Guid id)
    {
        try
        {
            var app = await _leaveService.GetLeaveApplicationAsync(id);
            return Ok(app);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("applications/employee/{employeeId}")]
    public async Task<ActionResult<List<LeaveApplicationDto>>> GetEmployeeApplications(Guid employeeId)
    {
        return Ok(await _leaveService.GetLeaveApplicationsByEmployeeAsync(employeeId));
    }

    [HttpGet("applications/pending")]
    public async Task<ActionResult<List<LeaveApplicationDto>>> GetPendingApplications()
    {
        return Ok(await _leaveService.GetPendingApplicationsAsync());
    }

    [HttpPost("applications/{id}/process")]
    public async Task<IActionResult> ProcessApplication(Guid id, [FromBody] LeaveApprovalDto approval)
    {
        try
        {
            await _leaveService.ProcessLeaveApplicationAsync(id, approval);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("applications/{id}/cancel")]
    public async Task<IActionResult> CancelApplication(Guid id)
    {
        try
        {
            await _leaveService.CancelLeaveApplicationAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public record BalanceAdjustmentDto(Guid EmployeeId, Guid LeaveTypeId, double Adjustment);
