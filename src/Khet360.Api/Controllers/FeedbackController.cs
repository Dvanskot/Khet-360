using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackCreateDto feedbackDto, [FromQuery] Guid userId)
    {
        await _feedbackService.SubmitFeedbackAsync(userId, feedbackDto);
        return Ok();
    }

    [HttpGet("tenant")]
    public async Task<ActionResult<List<FeedbackDto>>> GetFeedback()
    {
        var feedback = await _feedbackService.GetTenantFeedbackAsync();
        return Ok(feedback);
    }

    [HttpPost("resolve")]
    public async Task<IActionResult> ResolveFeedback([FromBody] ResolveFeedbackRequest request)
    {
        await _feedbackService.ResolveFeedbackAsync(request.FeedbackId, request.Resolution);
        return Ok();
    }
}

public record ResolveFeedbackRequest(Guid FeedbackId, string Resolution);
