using System;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}
