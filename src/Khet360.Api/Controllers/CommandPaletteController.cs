using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using System.Threading.Tasks;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/command-palette")]
public class CommandPaletteController : ControllerBase
{
    private readonly ICommandPaletteService _paletteService;

    public CommandPaletteController(ICommandPaletteService paletteService)
    {
        _paletteService = paletteService;
    }

    [HttpGet("actions")]
    public async Task<ActionResult<List<CommandActionDto>>> GetActions([FromQuery] string? context = null)
    {
        var actions = await _paletteService.GetAvailableCommandsAsync(context);
        return Ok(actions);
    }
}
