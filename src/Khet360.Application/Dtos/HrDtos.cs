using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record EmployeeDto(
    Guid Id,
    Guid? UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string EmployeeCode,
    string Status,
    DateTime HireDate,
    Guid DepartmentId,
    string DepartmentName,
    Guid PositionId,
    string PositionTitle,
    Guid BranchId,
    Guid? ManagerId,
    string? ManagerName,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Qualifications,
    decimal Salary,
    string ContractType
);

public record EmployeeCreateDto(
    Guid? UserId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string EmployeeCode,
    Guid DepartmentId,
    Guid PositionId,
    Guid BranchId,
    Guid? ManagerId,
    DateTime HireDate,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Qualifications,
    decimal Salary,
    string ContractType
);

public record EmployeeUpdateDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    Guid DepartmentId,
    Guid PositionId,
    Guid BranchId,
    Guid? ManagerId,
    string Status,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Qualifications
);

public record DepartmentDto(Guid Id, string Name, string? Description, Guid BranchId);
public record DepartmentCreateDto(string Name, string? Description, Guid BranchId);
public record PositionDto(Guid Id, string Title, string? Description, string? Grade);
public record PositionCreateDto(string Title, string? Description, string? Grade);
