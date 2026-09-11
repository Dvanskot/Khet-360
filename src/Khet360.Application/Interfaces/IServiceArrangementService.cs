using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IServiceArrangementService
{
    Task<Guid> CreateArrangementAsync(ServiceArrangementCreateDto dto, Guid branchId);
    Task<ServiceArrangementDto?> GetArrangementAsync(Guid id);
    Task UpdateArrangementAsync(Guid id, ServiceArrangementUpdateDto dto);
    Task DeleteArrangementAsync(Guid id);
    Task<IEnumerable<ServiceArrangementDto>> GetArrangementsByCaseAsync(Guid funeralCaseId);
}
