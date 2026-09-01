using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/deal-boards")]
public class DealBoardController : ControllerBase
{
    private readonly IDealBoardService _boardService;

    public DealBoardController(IDealBoardService boardService)
    {
        _boardService = boardService;
    }

    [HttpGet("leads")]
    public async Task<ActionResult<DealBoardDto>> GetLeadBoard([FromQuery] Guid branchId)
    {
        var board = await _boardService.GetLeadBoardAsync(branchId);
        return Ok(board);
    }

    [HttpGet("opportunities")]
    public async Task<ActionResult<DealBoardDto>> GetOpportunityBoard([FromQuery] Guid branchId)
    {
        var board = await _boardService.GetOpportunityBoardAsync(branchId);
        return Ok(board);
    }

    [HttpPatch("leads/{id}/status")]
    public async Task<IActionResult> UpdateLeadStatus(Guid id, [FromBody] int newStatus)
    {
        await _boardService.UpdateLeadStatusAsync(id, newStatus);
        return Ok();
    }

    [HttpPatch("opportunities/{id}/stage")]
    public async Task<IActionResult> UpdateOpportunityStage(Guid id, [FromBody] int newStage)
    {
        await _boardService.UpdateOpportunityStageAsync(id, newStage);
        return Ok();
    }
}
