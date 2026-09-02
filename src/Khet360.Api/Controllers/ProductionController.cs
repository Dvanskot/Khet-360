using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly IProductionService _productionService;

    public ProductionController(IProductionService productionService)
    {
        _productionService = productionService;
    }

    [HttpPost("orders")]
    public async Task<ActionResult<Guid>> CreateOrder([FromBody] Guid memorialId)
    {
        var id = await _productionService.CreateProductionOrderAsync(memorialId);
        return Ok(id);
    }

    [HttpGet("orders/{id}")]
    public async Task<ActionResult<ProductionOrderDto>> GetOrder(Guid id)
    {
        try
        {
            return Ok(await _productionService.GetProductionOrderAsync(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("orders/active")]
    public async Task<ActionResult<List<ProductionOrderDto>>> GetActiveOrders()
    {
        return Ok(await _productionService.GetActiveOrdersAsync());
    }

    [HttpPost("orders/{id}/advance")]
    public async Task<IActionResult> AdvanceStage(Guid id, [FromBody] Guid artisanId)
    {
        try
        {
            await _productionService.AdvanceStageAsync(id, artisanId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("orders/{id}/log-time")]
    public async Task<IActionResult> LogTime(Guid id, [FromBody] LogTimeRequest request)
    {
        try
        {
            await _productionService.LogTimeAsync(id, request.ArtisanId, request.Hours, request.Notes);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("orders/{id}/quality-check")]
    public async Task<IActionResult> QualityCheck(Guid id, [FromBody] QualityCheckRequest request)
    {
        try
        {
            await _productionService.PerformQualityCheckAsync(id, request.InspectorId, request.Passed, request.Comments);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public record LogTimeRequest(Guid ArtisanId, double Hours, string Notes);
public record QualityCheckRequest(Guid InspectorId, bool Passed, string Comments);
