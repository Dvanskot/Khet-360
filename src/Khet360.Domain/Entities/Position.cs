using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class Position : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Grade { get; set; }
}
