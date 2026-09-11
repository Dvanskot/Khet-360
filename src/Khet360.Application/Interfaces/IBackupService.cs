using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IBackupService
{
    Task<Guid> RequestBackupAsync(Guid tenantId);
    Task<Guid> RequestRestoreAsync(Guid tenantId, Guid backupJobId);
    Task<PlatformBackupJob> GetBackupStatusAsync(Guid backupJobId);
    Task<List<PlatformBackupJob>> GetBackupHistoryAsync(Guid tenantId);
    Task<PlatformRestoreJob> GetRestoreStatusAsync(Guid restoreJobId);
    Task<List<PlatformRestoreJob>> GetRestoreHistoryAsync(Guid tenantId);
    Task PerformBackupInternalAsync(Guid backupJobId);
    Task PerformRestoreInternalAsync(Guid restoreJobId);
}
