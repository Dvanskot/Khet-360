using System;
using System.ComponentModel.DataAnnotations;
using Khet360.Domain.Enums;
using Khet360.Domain.Entities;

namespace Khet360.Domain.Entities;

/// <summary>
/// Represents a member associated with an insurance policy.
/// A policy can have multiple members (e.g., Main member, Spouse, Children).
/// Implements IBranchScoped to ensure membership records are isolated per branch.
/// </summary>
public class InsurancePolicyMember : IBranchScoped
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid PolicyId { get; set; }
    public virtual InsurancePolicy Policy { get; set; } = null!;

    [Required]
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    [Required]
    public MemberRole Role { get; set; }

    [Required]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid BranchId { get; set; }
}
