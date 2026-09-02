using System;
using Khet360.Domain.Common;

namespace Khet360.Domain.Entities;

public class FuneralProduct : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DefaultPrice { get; set; }
    public bool IsManufacturable { get; set; } = false;
}
