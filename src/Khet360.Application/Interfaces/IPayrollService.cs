using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IPayrollService
{
    Task<PayProfileDto> GetPayProfileAsync(Guid employeeId);
    Task<Guid> CreatePayProfileAsync(PayProfileCreateDto dto);

    Task<List<PayItemDto>> GetPayItemsAsync();
    Task<Guid> CreatePayItemAsync(PayItemCreateDto dto);

    Task<Guid> CreatePayrollRunAsync(PayrollRunCreateDto dto);
    Task<PayrollRunDto> GetPayrollRunAsync(Guid id);
    Task CalculatePayrollAsync(Guid runId);
    Task FinalizePayrollRunAsync(Guid runId, Guid approvedBy);

    Task<PayslipDto> GetPayslipAsync(Guid employeeId, Guid runId);
}
