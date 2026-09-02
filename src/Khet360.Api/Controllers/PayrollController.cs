using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpGet("pay-profiles/{employeeId}")]
    public async Task<ActionResult<PayProfileDto>> GetPayProfile(Guid employeeId)
    {
        try
        {
            return Ok(await _payrollService.GetPayProfileAsync(employeeId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("pay-profiles")]
    public async Task<ActionResult<Guid>> CreatePayProfile([FromBody] PayProfileCreateDto dto)
    {
        var id = await _payrollService.CreatePayProfileAsync(dto);
        return Ok(id);
    }

    [HttpGet("pay-items")]
    public async Task<ActionResult<List<PayItemDto>>> GetPayItems()
    {
        return Ok(await _payrollService.GetPayItemsAsync());
    }

    [HttpPost("pay-items")]
    public async Task<ActionResult<Guid>> CreatePayItem([FromBody] PayItemCreateDto dto)
    {
        var id = await _payrollService.CreatePayItemAsync(dto);
        return Ok(id);
    }

    [HttpPost("runs")]
    public async Task<ActionResult<Guid>> CreatePayrollRun([FromBody] PayrollRunCreateDto dto)
    {
        var id = await _payrollService.CreatePayrollRunAsync(dto);
        return Ok(id);
    }

    [HttpGet("runs/{id}")]
    public async Task<ActionResult<PayrollRunDto>> GetPayrollRun(Guid id)
    {
        try
        {
            return Ok(await _payrollService.GetPayrollRunAsync(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("runs/{id}/calculate")]
    public async Task<IActionResult> CalculatePayroll(Guid id)
    {
        try
        {
            await _payrollService.CalculatePayrollAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("runs/{id}/finalize")]
    public async Task<IActionResult> FinalizePayroll(Guid id, [FromBody] Guid approvedBy)
    {
        try
        {
            await _payrollService.FinalizePayrollRunAsync(id, approvedBy);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("payslips/{employeeId}/{runId}")]
    public async Task<ActionResult<PayslipDto>> GetPayslip(Guid employeeId, Guid runId)
    {
        try
        {
            return Ok(await _payrollService.GetPayslipAsync(employeeId, runId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
