using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IRepatriationService
{
    Task<Guid> RequestRepatriationAsync(RepatriationCreateDto dto, Guid branchId);
    Task<RepatriationDto?> GetRepatriationAsync(Guid id);
    Task UpdateRepatriationStatusAsync(Guid id, RepatriationUpdateDto dto);
    Task<IEnumerable<RepatriationDto>> GetRepatriationsByCaseAsync(Guid funeralCaseId);
}
