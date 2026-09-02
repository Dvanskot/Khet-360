using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using Khet360.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/arrangements/wizard")]
public class ArrangementWizardController : ControllerBase
{
    private readonly IArrangementWizardService _wizardService;

    public ArrangementWizardController(IArrangementWizardService wizardService)
    {
        _wizardService = wizardService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] Guid funeralCaseId, [FromQuery] Guid branchId)
    {
        var state = await _wizardService.StartWizardAsync(funeralCaseId, branchId);
        return Ok(state);
    }

    [HttpGet("{stateId}")]
    public async Task<IActionResult> GetState(Guid stateId)
    {
        var state = await _wizardService.GetStateAsync(stateId);
        if (state == null) return NotFound();
        return Ok(state);
    }

    [HttpPost("save-step")]
    public async Task<IActionResult> SaveStep([FromBody] SaveStepRequest request)
    {
        var result = await _wizardService.SaveStepAsync(request.StateId, request.Step, request.Data);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("finalize/{stateId}")]
    public async Task<IActionResult> Finalize(Guid stateId)
    {
        await _wizardService.FinalizeArrangementAsync(stateId);
        return Ok(new { Message = "Arrangement finalized successfully." });
    }
}

public record SaveStepRequest(
    Guid StateId,
    ArrangementWizardStep Step,
    Dictionary<string, string> Data);
