using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace Khet360.Tests;

public class ProductionReadinessTests
{
    private PlatformDbContext GetPlatformDb()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new PlatformDbContext(options);
    }

    [Fact]
    public async Task BackupService_Should_Create_Pending_Job()
    {
        // Arrange
        var db = GetPlatformDb();
        var mockStorage = new Mock<IFileStorageService>();
        var mockLogger = new Mock<ILogger<BackupService>>();
        var service = new BackupService(db, mockStorage.Object, mockLogger.Object);
        var tenantId = Guid.NewGuid();

        db.Tenants.Add(new Tenant { Id = tenantId, Slug = "test-tenant" });
        await db.SaveChangesAsync();

        // Act
        var jobId = await service.RequestBackupAsync(tenantId);

        // Assert
        var job = await db.BackupJobs.FindAsync(jobId);
        job.Should().NotBeNull();
        job.Status.Should().Be(BackupStatus.Pending);
        job.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task BackupService_PerformInternal_Should_Upload_And_Complete()
    {
        // Arrange
        var db = GetPlatformDb();
        var mockStorage = new Mock<IFileStorageService>();
        var mockLogger = new Mock<ILogger<BackupService>>();
        var service = new BackupService(db, mockStorage.Object, mockLogger.Object);

        var tenantId = Guid.NewGuid();
        var tenant = new Tenant { Id = tenantId, Slug = "test-tenant" };
        db.Tenants.Add(tenant);

        var job = new PlatformBackupJob
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Status = BackupStatus.Pending
        };
        db.BackupJobs.Add(job);
        await db.SaveChangesAsync();

        // Act
        await service.PerformBackupInternalAsync(job.Id);

        // Assert
        var updatedJob = await db.BackupJobs.FindAsync(job.Id);
        updatedJob.Should().NotBeNull();
        updatedJob!.Status.Should().Be(BackupStatus.Completed);
        updatedJob.BackupFileKey.Should().NotBeNullOrEmpty();
        mockStorage.Verify(s => s.UploadFileAsync(
            It.IsAny<Stream>(),
            It.Is<string>(f => f.Contains("test-tenant")),
            "application/octet-stream",
            It.Is<string>(f => f.Contains("backups/test-tenant"))),
            Times.Once);
    }

    [Fact]
    public async Task MigrationService_Should_Create_Migration_Job()
    {
        // Arrange
        var db = GetPlatformDb();
        var mockBackup = new Mock<IBackupService>();
        var mockLogger = new Mock<ILogger<MigrationService>>();
        var service = new MigrationService(db, mockBackup.Object, mockLogger.Object);
        var tenantId = Guid.NewGuid();

        db.Tenants.Add(new Tenant { Id = tenantId, Slug = "test-tenant", Tier = IsolationTier.Isolated });
        await db.SaveChangesAsync();

        // Act
        var result = await service.MigrateTenantAsync(tenantId, "Dedicated Server");

        // Assert
        result.Should().BeTrue();
        var job = await db.MigrationJobs.FirstOrDefaultAsync(j => j.TenantId == tenantId);
        job.Should().NotBeNull();
        job.TargetTier.Should().Be(IsolationTier.Dedicated);
        job.Status.Should().Be(MigrationStatus.Pending);
    }

    [Fact]
    public async Task MigrationService_CompleteTransfer_Should_Update_Tenant_Tier()
    {
        // Arrange
        var db = GetPlatformDb();
        var mockBackup = new Mock<IBackupService>();
        var mockLogger = new Mock<ILogger<MigrationService>>();
        var service = new MigrationService(db, mockBackup.Object, mockLogger.Object);

        var tenantId = Guid.NewGuid();
        var tenant = new Tenant { Id = tenantId, Slug = "test-tenant", Tier = IsolationTier.Isolated };
        db.Tenants.Add(tenant);

        var backupJobId = Guid.NewGuid();
        var backupJob = new PlatformBackupJob
        {
            Id = backupJobId,
            Status = BackupStatus.Completed
        };
        db.BackupJobs.Add(backupJob);

        var migrationJob = new PlatformMigrationJob
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            BackupJobId = backupJobId,
            SourceTier = IsolationTier.Isolated,
            TargetTier = IsolationTier.Dedicated,
            Status = MigrationStatus.InProgress
        };
        db.MigrationJobs.Add(migrationJob);
        await db.SaveChangesAsync();

        // Act
        await service.CompleteTransferAsync(migrationJob.Id);

        // Assert
        var updatedTenant = await db.Tenants.FindAsync(tenantId);
        updatedTenant.Should().NotBeNull();
        updatedTenant!.Tier.Should().Be(IsolationTier.Dedicated);
        updatedTenant.ConnectionString.Should().Contain("DedicatedServer");

        var updatedJob = await db.MigrationJobs.FindAsync(migrationJob.Id);
        updatedJob.Should().NotBeNull();
        updatedJob!.Status.Should().Be(MigrationStatus.Completed);
    }
}
