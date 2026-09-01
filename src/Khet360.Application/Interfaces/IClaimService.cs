using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IClaimService
{
    Task<Guid> CreateClaimAsync(ClaimCreateDto dto, Guid branchId);
    Task<ClaimDto?> GetClaimAsync(Guid id);
    Task UpdateClaimStatusAsync(Guid id, ClaimUpdateDto dto);
    Task<IEnumerable<ClaimDto>> GetClaimsByPolicyAsync(Guid policyId);
    Task<IEnumerable<ClaimDto>> GetClaimsByCaseAsync(Guid funeralCaseId);
    Task<Guid> AddPaymentAsync(ClaimPaymentCreateDto dto, Guid branchId);
    Task<IEnumerable<ClaimPaymentDto>> GetPaymentsForClaimAsync(Guid claimId);
}
