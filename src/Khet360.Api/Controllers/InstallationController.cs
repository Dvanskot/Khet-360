using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstallationController : ControllerBase
{
    private readonly IInstallationService _installationService;

    public InstallationController(IInstallationService installationService)
    {
        _installationService = installationService;
    }

    [HttpPost("schedule")]
    public async Task<ActionResult<Guid>> Schedule([FromBody] InstallationScheduleDto dto)
    {
        var id = await _installationService.ScheduleInstallationAsync(dto);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InstallationJobDto>> GetJob(Guid id)
    {
        try
        {
            return Ok(await _installationService.GetInstallationJobAsync(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("branch/{branchId}")]
    public async Task<ActionResult<List<InstallationJobDto>>> GetJobsByBranch(Guid branchId)
    {
        return Ok(await _installationService.GetJobsByBranchAsync(branchId));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StatusUpdateRequest request)
    {
        try
        {
            await _installationService.UpdateStatusAsync(id, request.Status, request.Notes);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/verify")]
    public async Task<IActionResult> VerifyChecklist(Guid id, [FromBody] VerifyRequest request)
    {
        try
        {
            await _installationService.VerifyChecklistItemAsync(id, request.Requirement, request.VerifiedBy);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/sign-off")]
    public async Task<IActionResult> SignOff(Guid id, [FromBody] InstallationSignOffDto dto)
    {
        try
        {
            await _installationService.SignOffInstallationAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public record StatusUpdateRequest(InstallationStatus Status, string? Notes);
public record VerifyRequest(string Requirement, Guid VerifiedBy);
