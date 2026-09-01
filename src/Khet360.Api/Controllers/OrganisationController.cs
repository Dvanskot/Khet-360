using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Api.Attributes;

namespace Khet360.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrganisationController : ControllerBase
{
    private readonly IOrganisationService _organisationService;

    public OrganisationController(IOrganisationService organisationService)
    {
        _organisationService = organisationService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var config = await _organisationService.GetConfigAsync();
        if (config == null)
        {
            return NotFound(new { Message = "Organisation configuration not found." });
        }
        return Ok(config);
    }

    [HttpPost("update")]
    [HasPermission("Organisation.Update")]
    public async Task<IActionResult> Update([FromBody] OrganisationConfig config)
    {
        await _organisationService.UpdateConfigAsync(config);
        return Ok(new { Message = "Organisation configuration updated successfully." });
    }
}
