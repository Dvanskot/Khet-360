using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities.Platform;

public class PlatformAnnouncement
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementPriority Priority { get; set; }
    public AnnouncementScope Scope { get; set; }
    public Guid? SpecificTenantId { get; set; }
    public string? TargetPlanCategories { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum AnnouncementPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum AnnouncementScope
{
    All,
    PlanCategory,
    SpecificTenant
}
