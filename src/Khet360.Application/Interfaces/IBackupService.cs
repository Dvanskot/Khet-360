using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Domain.Entities;

namespace Khet360.Application.Interfaces;

public interface IBackupService
{
    Task<Guid> RequestBackupAsync(Guid tenantId);
    Task<Guid> RequestRestoreAsync(Guid tenantId, Guid backupJobId);
    Task<PlatformBackupJob> GetBackupStatusAsync(Guid backupJobId);
    Task<List<PlatformBackupJob>> GetBackupHistoryAsync(Guid tenantId);
    Task PerformBackupInternalAsync(Guid backupJobId);
}
