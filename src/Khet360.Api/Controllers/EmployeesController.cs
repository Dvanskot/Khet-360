using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(Guid id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(ApiResponse.Ok(employee, "Employee retrieved successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse.Fail<EmployeeDto>("Employee not found"));
        }
    }

    [HttpGet("branch/{branchId}")]
    public async Task<ActionResult<ApiResponse<List<EmployeeDto>>>> GetEmployeesByBranch(Guid branchId)
    {
        var employees = await _employeeService.GetEmployeesByBranchAsync(branchId);
        return Ok(ApiResponse.Ok(employees, "Employees retrieved successfully"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateEmployee([FromBody] EmployeeCreateDto dto)
    {
        var id = await _employeeService.CreateEmployeeAsync(dto);
        return CreatedAtAction(nameof(GetEmployee), new { id }, ApiResponse.Ok(id, "Employee created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateEmployee(Guid id, [FromBody] EmployeeUpdateDto dto)
    {
        try
        {
            await _employeeService.UpdateEmployeeAsync(id, dto);
            return Ok(ApiResponse.Ok(new { success = true }, "Employee updated successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse.Fail<object>("Employee not found"));
        }
    }

    [HttpPatch("{id}/terminate")]
    public async Task<ActionResult<ApiResponse<object>>> TerminateEmployee(Guid id, [FromBody] DateTime terminationDate)
    {
        try
        {
            await _employeeService.TerminateEmployeeAsync(id, terminationDate);
            return Ok(ApiResponse.Ok(new { success = true }, "Employee terminated successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse.Fail<object>("Employee not found"));
        }
    }

    [HttpPost("departments")]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateDepartment([FromBody] DepartmentCreateDto dto)
    {
        var id = await _employeeService.CreateDepartmentAsync(dto);
        return Ok(ApiResponse.Ok(id, "Department created successfully"));
    }

    [HttpGet("positions")]
    public async Task<ActionResult<ApiResponse<List<PositionDto>>>> GetPositions()
    {
        var positions = await _employeeService.GetPositionsAsync();
        return Ok(ApiResponse.Ok(positions, "Positions retrieved successfully"));
    }

    [HttpPost("positions")]
    public async Task<ActionResult<ApiResponse<Guid>>> CreatePosition([FromBody] PositionCreateDto dto)
    {
        var id = await _employeeService.CreatePositionAsync(dto);
        return Ok(ApiResponse.Ok(id, "Position created successfully"));
    }
}
