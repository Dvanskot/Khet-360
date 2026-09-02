using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackupController : ControllerBase
{
    private readonly IBackupService _backupService;

    public BackupController(IBackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestBackup([FromQuery] Guid tenantId)
    {
        var jobId = await _backupService.RequestBackupAsync(tenantId);
        return Ok(new { BackupJobId = jobId });
    }

    [HttpGet("status/{jobId}")]
    public async Task<IActionResult> GetStatus(Guid jobId)
    {
        var job = await _backupService.GetBackupStatusAsync(jobId);
        return Ok(job);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] Guid tenantId)
    {
        var history = await _backupService.GetBackupHistoryAsync(tenantId);
        return Ok(history);
    }

    [HttpPost("restore")]
    public async Task<IActionResult> RequestRestore([FromBody] RestoreRequest request)
    {
        var jobId = await _backupService.RequestRestoreAsync(request.TenantId, request.BackupJobId);
        return Ok(new { RestoreJobId = jobId });
    }
}

public record RestoreRequest(Guid TenantId, Guid BackupJobId);
