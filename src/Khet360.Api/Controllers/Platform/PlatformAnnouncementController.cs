using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using Khet360.Domain.Entities.Platform;

namespace Khet360.Api.Controllers.Platform;

[Authorize(AuthenticationSchemes = "PlatformJwt", Roles = "PlatformAdmin")]
[ApiController]
[Route("api/platform/announcements")]
public class PlatformAnnouncementController : ControllerBase
{
    private readonly IPlatformAnnouncementService _announcementService;
    private readonly ILogger<PlatformAnnouncementController> _logger;

    public PlatformAnnouncementController(IPlatformAnnouncementService announcementService, ILogger<PlatformAnnouncementController> logger)
    {
        _announcementService = announcementService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<AnnouncementDto>> CreateAnnouncement([FromBody] CreateAnnouncementRequest request)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());

            var announcement = await _announcementService.CreateAnnouncementAsync(
                request.Title,
                request.Content,
                request.Priority,
                request.Scope,
                request.SpecificTenantId,
                request.TargetPlanCategories ?? string.Empty,
                userId,
                username
            );

            return CreatedAtAction(nameof(GetActiveAnnouncements), new { id = announcement.Id }, announcement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create announcement");
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult> GetActiveAnnouncements([FromQuery] Guid? tenantId)
    {
        var announcements = await _announcementService.GetActiveAnnouncementsAsync(tenantId);
        return Ok(announcements);
    }

    [HttpDelete("{announcementId}")]
    public async Task<IActionResult> DeactivateAnnouncement(Guid announcementId)
    {
        var success = await _announcementService.DeactivateAnnouncementAsync(announcementId);
        if (!success) return NotFound(new { Message = "Announcement not found." });
        return NoContent();
    }
}

public record CreateAnnouncementRequest(
    string Title,
    string Content,
    AnnouncementPriority Priority,
    AnnouncementScope Scope,
    Guid? SpecificTenantId,
    string? TargetPlanCategories
);
