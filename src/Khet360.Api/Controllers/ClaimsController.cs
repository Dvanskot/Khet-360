using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;

    public ClaimsController(IClaimService claimService)
    {
        _claimService = claimService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClaimCreateDto dto, [FromQuery] Guid branchId)
    {
        var id = await _claimService.CreateClaimAsync(dto, branchId);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var claim = await _claimService.GetClaimAsync(id);
        if (claim == null) return NotFound();
        return Ok(claim);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] ClaimUpdateDto dto)
    {
        await _claimService.UpdateClaimStatusAsync(id, dto);
        return NoContent();
    }

    [HttpGet("policy/{policyId}")]
    public async Task<IActionResult> GetByPolicy(Guid policyId)
    {
        var claims = await _claimService.GetClaimsByPolicyAsync(policyId);
        return Ok(claims);
    }

    [HttpGet("case/{funeralCaseId}")]
    public async Task<IActionResult> GetByCase(Guid funeralCaseId)
    {
        var claims = await _claimService.GetClaimsByCaseAsync(funeralCaseId);
        return Ok(claims);
    }

    [HttpPost("payments")]
    public async Task<IActionResult> AddPayment([FromBody] ClaimPaymentCreateDto dto, [FromQuery] Guid branchId)
    {
        var id = await _claimService.AddPaymentAsync(dto, branchId);
        return Ok(id);
    }

    [HttpGet("claim/{claimId}/payments")]
    public async Task<IActionResult> GetPayments(Guid claimId)
    {
        var payments = await _claimService.GetPaymentsForClaimAsync(claimId);
        return Ok(payments);
    }
}
