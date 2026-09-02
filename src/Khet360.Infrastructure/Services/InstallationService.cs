using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class InstallationService : IInstallationService
{
    private readonly TenantDbContext _db;

    public InstallationService(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> ScheduleInstallationAsync(InstallationScheduleDto dto)
    {
        var job = new InstallationJob
        {
            Id = Guid.NewGuid(),
            MemorialId = dto.MemorialId,
            BranchId = dto.BranchId,
            VehicleId = dto.VehicleId,
            LeadArtisanId = dto.LeadArtisanId,
            ScheduledDate = dto.ScheduledDate,
            Status = InstallationStatus.Scheduled
        };

        // Seed a default checklist
        var checklist = new List<InstallationChecklist>
        {
            new() { Id = Guid.NewGuid(), InstallationJobId = job.Id, Requirement = "Site Readiness Verified", IsVerified = false },
            new() { Id = Guid.NewGuid(), InstallationJobId = job.Id, Requirement = "Cemetery Plot Confirmed", IsVerified = false },
            new() { Id = Guid.NewGuid(), InstallationJobId = job.Id, Requirement = "Equipment Loaded", IsVerified = false }
        };

        _db.InstallationJobs.Add(job);
        _db.InstallationChecklists.AddRange(checklist);
        await _db.SaveChangesAsync();

        return job.Id;
    }

    public async Task<InstallationJobDto> GetInstallationJobAsync(Guid id)
    {
        var job = await _db.InstallationJobs
            .Include(j => j.Checklist)
            .Include(j => j.SignOff)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null) throw new KeyNotFoundException("Installation job not found.");

        return new InstallationJobDto(
            job.Id,
            job.MemorialId,
            job.BranchId,
            job.VehicleId,
            job.LeadArtisanId,
            job.ScheduledDate,
            job.ActualInstallationDate,
            job.Status,
            job.InstallationNotes,
            job.Checklist.Select(c => new InstallationChecklistDto(c.Id, c.Requirement, c.IsVerified, c.VerifiedAtUtc, c.VerifiedBy)).ToList(),
            job.SignOff != null
        );
    }

    public async Task<List<InstallationJobDto>> GetJobsByBranchAsync(Guid branchId)
    {
        var jobs = await _db.InstallationJobs
            .Where(j => j.BranchId == branchId)
            .Include(j => j.Checklist)
            .Include(j => j.SignOff)
            .ToListAsync();

        return jobs.Select(j => new InstallationJobDto(
            j.Id,
            j.MemorialId,
            j.BranchId,
            j.VehicleId,
            j.LeadArtisanId,
            j.ScheduledDate,
            j.ActualInstallationDate,
            j.Status,
            j.InstallationNotes,
            j.Checklist.Select(c => new InstallationChecklistDto(c.Id, c.Requirement, c.IsVerified, c.VerifiedAtUtc, c.VerifiedBy)).ToList(),
            j.SignOff != null
        )).ToList();
    }

    public async Task UpdateStatusAsync(Guid id, InstallationStatus status, string? notes)
    {
        var job = await _db.InstallationJobs.FindAsync(id);
        if (job == null) throw new KeyNotFoundException("Installation job not found.");

        job.Status = status;
        job.InstallationNotes = notes;

        if (status == InstallationStatus.Completed)
        {
            job.ActualInstallationDate = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task VerifyChecklistItemAsync(Guid jobId, string requirement, Guid verifiedBy)
    {
        var item = await _db.InstallationChecklists
            .FirstOrDefaultAsync(c => c.InstallationJobId == jobId && c.Requirement == requirement);

        if (item == null) throw new KeyNotFoundException("Checklist item not found.");

        item.IsVerified = true;
        item.VerifiedAtUtc = DateTime.UtcNow;
        item.VerifiedBy = verifiedBy;

        await _db.SaveChangesAsync();
    }

    public async Task SignOffInstallationAsync(Guid jobId, InstallationSignOffDto dto)
    {
        var job = await _db.InstallationJobs.FindAsync(jobId);
        if (job == null) throw new KeyNotFoundException("Installation job not found.");

        var signOff = new InstallationSignOff
        {
            Id = Guid.NewGuid(),
            InstallationJobId = jobId,
            CustomerName = dto.CustomerName,
            SignatureData = dto.SignatureData,
            Comments = dto.Comments,
            IsSatisfied = dto.IsSatisfied,
            SignedAtUtc = DateTime.UtcNow
        };

        _db.InstallationSignOffs.Add(signOff);
        job.Status = InstallationStatus.Completed;

        await _db.SaveChangesAsync();
    }
}
