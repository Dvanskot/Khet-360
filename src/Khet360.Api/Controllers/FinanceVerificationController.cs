using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Infrastructure.Services;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceVerificationController : ControllerBase
{
    private readonly IFinanceVerificationService _verificationService;

    public FinanceVerificationController(IFinanceVerificationService verificationService)
    {
        _verificationService = verificationService;
    }

    [HttpGet("verify")]
    public async Task<IActionResult> VerifyInvariants()
    {
        try
        {
            var result = await _verificationService.VerifyInvariantsAsync();

            if (result.IsBalanced)
            {
                return Ok(new {
                    Status = "Balanced",
                    Message = "All financial transactions are balanced."
                });
            }

            return BadRequest(new {
                Status = "Unbalanced",
                Message = "Financial invariant violations detected.",
                Violations = result.Violations
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred during verification: {ex.Message}" });
        }
    }
}
