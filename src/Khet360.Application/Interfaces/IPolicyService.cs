using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IPolicyService
{
    Task<Guid> CreatePolicyAsync(PolicyCreateDto dto, Guid branchId);
    Task<PolicyDto?> GetPolicyAsync(Guid id);
    Task<IEnumerable<PolicyDto>> GetPoliciesByCustomerAsync(Guid customerId);
    Task UpdatePolicyAsync(Guid id, PolicyUpdateDto dto);
    Task DeletePolicyAsync(Guid id);
}
