using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Domain.Entities;

namespace Khet360.Application.Interfaces;

public interface IInstallationService
{
    Task<Guid> ScheduleInstallationAsync(InstallationScheduleDto dto);
    Task<InstallationJobDto> GetInstallationJobAsync(Guid id);
    Task<List<InstallationJobDto>> GetJobsByBranchAsync(Guid branchId);
    Task UpdateStatusAsync(Guid id, InstallationStatus status, string? notes);
    Task VerifyChecklistItemAsync(Guid jobId, string requirement, Guid verifiedBy);
    Task SignOffInstallationAsync(Guid jobId, InstallationSignOffDto dto);
}

public record InstallationScheduleDto(
    Guid MemorialId,
    Guid BranchId,
    Guid? VehicleId,
    Guid? LeadArtisanId,
    DateTime ScheduledDate
);

public record InstallationJobDto(
    Guid Id,
    Guid MemorialId,
    Guid BranchId,
    Guid? VehicleId,
    Guid? LeadArtisanId,
    DateTime? ScheduledDate,
    DateTime? ActualInstallationDate,
    InstallationStatus Status,
    string? InstallationNotes,
    List<InstallationChecklistDto> Checklist,
    bool IsSignedOff
);

public record InstallationChecklistDto(Guid Id, string Requirement, bool IsVerified, DateTime? VerifiedAtUtc, Guid? VerifiedBy);
public record InstallationSignOffDto(string CustomerName, string SignatureData, string Comments, bool IsSatisfied);
