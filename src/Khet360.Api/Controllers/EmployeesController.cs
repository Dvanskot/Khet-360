using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("branch/{branchId}")]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesByBranch(Guid branchId)
    {
        var employees = await _employeeService.GetEmployeesByBranchAsync(branchId);
        return Ok(employees);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateEmployee([FromBody] EmployeeCreateDto dto)
    {
        var id = await _employeeService.CreateEmployeeAsync(dto);
        return CreatedAtAction(nameof(GetEmployee), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeUpdateDto dto)
    {
        try
        {
            await _employeeService.UpdateEmployeeAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{id}/terminate")]
    public async Task<IActionResult> TerminateEmployee(Guid id, [FromBody] DateTime terminationDate)
    {
        try
        {
            await _employeeService.TerminateEmployeeAsync(id, terminationDate);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("departments/branch/{branchId}")]
    public async Task<ActionResult<List<DepartmentDto>>> GetDepartments(Guid branchId)
    {
        var departments = await _employeeService.GetDepartmentsByBranchAsync(branchId);
        return Ok(departments);
    }

    [HttpPost("departments")]
    public async Task<ActionResult<Guid>> CreateDepartment([FromBody] DepartmentCreateDto dto)
    {
        var id = await _employeeService.CreateDepartmentAsync(dto);
        return Ok(id);
    }

    [HttpGet("positions")]
    public async Task<ActionResult<List<PositionDto>>> GetPositions()
    {
        var positions = await _employeeService.GetPositionsAsync();
        return Ok(positions);
    }

    [HttpPost("positions")]
    public async Task<ActionResult<Guid>> CreatePosition([FromBody] PositionCreateDto dto)
    {
        var id = await _employeeService.CreatePositionAsync(dto);
        return Ok(id);
    }
}
