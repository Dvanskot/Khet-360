using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly TenantDbContext _db;
    private readonly PlatformDbContext _platformDb;

    public EmployeeService(TenantDbContext db, PlatformDbContext platformDb)
    {
        _db = db;
        _platformDb = platformDb;
    }

    public async Task<EmployeeDto> GetEmployeeByIdAsync(Guid id)
    {
        var employee = await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Branch)
            .Include(e => e.Manager)
            .Include(e => e.Contract)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) throw new KeyNotFoundException("Employee not found.");

        var position = await _platformDb.Positions.FindAsync(employee.PositionId);
        return MapToDto(employee, position?.Title ?? "Unknown");
    }

    public async Task<List<EmployeeDto>> GetEmployeesByBranchAsync(Guid branchId)
    {
        var employees = await _db.Employees
            .Where(e => e.BranchId == branchId)
            .Include(e => e.Department)
            .Include(e => e.Branch)
            .Include(e => e.Manager)
            .Include(e => e.Contract)
            .ToListAsync();

        var positions = await _platformDb.Positions.ToListAsync();
        var positionMap = positions.ToDictionary(p => p.Id, p => p.Title);

        return employees.Select(e => MapToDto(e, positionMap.GetValueOrDefault(e.PositionId, "Unknown"))).ToList();
    }

    public async Task<Guid> CreateEmployeeAsync(EmployeeCreateDto dto)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            EmployeeCode = dto.EmployeeCode,
            DepartmentId = dto.DepartmentId,
            PositionId = dto.PositionId,
            BranchId = dto.BranchId,
            ManagerId = dto.ManagerId,
            HireDate = dto.HireDate,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone,
            Qualifications = dto.Qualifications,
            Status = EmployeeStatus.Active
        };

        var contract = new EmploymentContract
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Salary = dto.Salary,
            Type = Enum.Parse<ContractType>(dto.ContractType),
            StartDate = employee.HireDate,
            Frequency = PaymentFrequency.Monthly // Default
        };

        _db.Employees.Add(employee);
        _db.EmploymentContracts.Add(contract);
        await _db.SaveChangesAsync();

        return employee.Id;
    }

    public async Task UpdateEmployeeAsync(Guid id, EmployeeUpdateDto dto)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) throw new KeyNotFoundException("Employee not found.");

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.PhoneNumber = dto.PhoneNumber;
        employee.DepartmentId = dto.DepartmentId;
        employee.PositionId = dto.PositionId;
        employee.BranchId = dto.BranchId;
        employee.ManagerId = dto.ManagerId;
        employee.Status = Enum.Parse<EmployeeStatus>(dto.Status);
        employee.EmergencyContactName = dto.EmergencyContactName;
        employee.EmergencyContactPhone = dto.EmergencyContactPhone;
        employee.Qualifications = dto.Qualifications;

        await _db.SaveChangesAsync();
    }

    public async Task TerminateEmployeeAsync(Guid id, DateTime terminationDate)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) throw new KeyNotFoundException("Employee not found.");

        employee.Status = EmployeeStatus.Terminated;
        employee.TerminationDate = terminationDate;

        await _db.SaveChangesAsync();
    }

    public async Task<DepartmentDto> GetDepartmentByIdAsync(Guid id)
    {
        var dept = await _db.Departments.FindAsync(id);
        if (dept == null) throw new KeyNotFoundException("Department not found.");
        return new DepartmentDto(dept.Id, dept.Name, dept.Description);
    }

    public async Task<Guid> CreateDepartmentAsync(DepartmentCreateDto dto)
    {
        var dept = new Department
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };
        _db.Departments.Add(dept);
        await _db.SaveChangesAsync();
        return dept.Id;
    }

    public async Task<PositionDto> GetPositionByIdAsync(Guid id)
    {
        var pos = await _platformDb.Positions.FindAsync(id);
        if (pos == null) throw new KeyNotFoundException("Position not found.");
        return new PositionDto(pos.Id, pos.Title, pos.Description, pos.Grade);
    }

    public async Task<List<PositionDto>> GetPositionsAsync()
    {
        return await _platformDb.Positions
            .Select(p => new PositionDto(p.Id, p.Title, p.Description, p.Grade))
            .ToListAsync();
    }

    public async Task<Guid> CreatePositionAsync(PositionCreateDto dto)
    {
        var pos = new Position
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Grade = dto.Grade
        };
        _platformDb.Positions.Add(pos);
        await _platformDb.SaveChangesAsync();
        return pos.Id;
    }

    private static EmployeeDto MapToDto(Employee e, string positionTitle)
    {
        return new EmployeeDto(
            e.Id,
            e.UserId,
            e.FirstName,
            e.LastName,
            e.Email,
            e.PhoneNumber,
            e.EmployeeCode,
            e.Status.ToString(),
            e.HireDate,
            e.DepartmentId,
            e.Department?.Name ?? "Unknown",
            e.PositionId,
            positionTitle,
            e.BranchId,
            e.ManagerId,
            e.Manager?.FirstName + " " + e.Manager?.LastName,
            e.EmergencyContactName,
            e.EmergencyContactPhone,
            e.Qualifications,
            e.Contract?.Salary ?? 0,
            e.Contract?.Type.ToString() ?? "Unknown"
        );
    }
}
