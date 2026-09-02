using System;
using System.ComponentModel.DataAnnotations;

namespace Khet360.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}
