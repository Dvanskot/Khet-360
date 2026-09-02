using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class UserDashboardConfig : BaseEntity
{
    public Guid UserId { get; set; }
    public string ConfigJson { get; set; } = "{}"; // Stores widget layout and selection
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}
