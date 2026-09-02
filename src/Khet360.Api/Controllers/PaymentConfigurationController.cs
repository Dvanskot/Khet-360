using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khet360.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentConfigurationController : ControllerBase
{
    private readonly IPaymentConfigurationService _configService;

    public PaymentConfigurationController(IPaymentConfigurationService configService)
    {
        _configService = configService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var config = await _configService.GetConfigurationAsync();
        return Ok(config);
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] PaymentConfiguration config)
    {
        if (config == null) return BadRequest("Invalid configuration data.");

        await _configService.UpdateConfigurationAsync(config);
        return Ok(new { message = "Payment configuration updated successfully." });
    }

    [HttpPost("test-connection")]
    public async Task<IActionResult> TestConnection([FromBody] PaymentConfiguration config)
    {
        if (config == null) return BadRequest("Invalid configuration data.");

        var isWorking = await _configService.TestConnectionAsync(config);
        if (isWorking)
        {
            return Ok(new { status = "Success", message = "Connection to payment gateway is working." });
        }

        return BadRequest(new { status = "Failure", message = "Could not connect to the payment gateway. Please verify your API keys and Merchant ID." });
    }
}
