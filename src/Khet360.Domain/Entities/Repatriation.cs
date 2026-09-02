using System;
using Khet360.Domain.Enums;

namespace Khet360.Domain.Entities;

public class Repatriation : IBranchScoped
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public RepatriationStatus Status { get; set; }
    public TransportMethod TransportMethod { get; set; }
    public string OriginCountry { get; set; } = string.Empty;
    public string DestinationCountry { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }

    public Guid FuneralCaseId { get; set; }
    public virtual FuneralCase FuneralCase { get; set; } = null!;

    public Guid BranchId { get; set; }
}
