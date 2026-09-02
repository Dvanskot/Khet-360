using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Common;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid UserId { get; set; }
    public string Category { get; set; } = "General"; // e.g., Bug, FeatureRequest, UX
    public string Message { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5
    public string? Resolution { get; set; }
    public FeedbackStatus Status { get; set; } = FeedbackStatus.Submitted;
    public Guid? ReviewerId { get; set; }
    public ResolutionHelpfulness? ResolutionHelpfulness { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
