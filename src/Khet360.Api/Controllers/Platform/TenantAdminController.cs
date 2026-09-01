using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Khet360.Application.Tenants.Commands;

namespace Khet360.Api.Controllers.Platform;

[Authorize(Roles = "PlatformAdmin")]
[ApiController]
[Route("api/platform/[controller]")]
public class TenantAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
    {
        try
        {
            var tenantId = await _mediator.Send(command);
            return Ok(new { TenantId = tenantId, Message = "Tenant created successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
