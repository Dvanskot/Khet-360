using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeDto> GetEmployeeByIdAsync(Guid id);
    Task<List<EmployeeDto>> GetEmployeesByBranchAsync(Guid branchId);
    Task<Guid> CreateEmployeeAsync(EmployeeCreateDto dto);
    Task UpdateEmployeeAsync(Guid id, EmployeeUpdateDto dto);
    Task TerminateEmployeeAsync(Guid id, DateTime terminationDate);

    Task<DepartmentDto> GetDepartmentByIdAsync(Guid id);
    Task<Guid> CreateDepartmentAsync(DepartmentCreateDto dto);

    Task<PositionDto> GetPositionByIdAsync(Guid id);
    Task<List<PositionDto>> GetPositionsAsync();
    Task<Guid> CreatePositionAsync(PositionCreateDto dto);
}
