using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyService _policyService;

    public PoliciesController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PolicyCreateDto dto, [FromQuery] Guid branchId)
    {
        var id = await _policyService.CreatePolicyAsync(dto, branchId);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var policy = await _policyService.GetPolicyAsync(id);
        if (policy == null) return NotFound();
        return Ok(policy);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var policies = await _policyService.GetPoliciesByCustomerAsync(customerId);
        return Ok(policies);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PolicyUpdateDto dto)
    {
        await _policyService.UpdatePolicyAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _policyService.DeletePolicyAsync(id);
        return NoContent();
    }
}
