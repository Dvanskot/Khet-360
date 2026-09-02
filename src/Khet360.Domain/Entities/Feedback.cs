using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid UserId { get; set; }
    public string Category { get; set; } = "General"; // e.g., Bug, FeatureRequest, UX
    public string Message { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5
    public string? Resolution { get; set; }
    public bool IsResolved { get; set; } = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
