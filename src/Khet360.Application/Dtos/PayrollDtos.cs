using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record PayProfileDto(
    Guid Id,
    Guid EmployeeId,
    string BankName,
    string AccountNumber,
    string BranchCode,
    string TaxNumber,
    string TaxBracket
);

public record PayProfileCreateDto(
    Guid EmployeeId,
    string BankName,
    string AccountNumber,
    string BranchCode,
    string TaxNumber,
    string TaxBracket
);

public record PayItemDto(Guid Id, string Name, string Code, string Type, bool IsStatutory);
public record PayItemCreateDto(string Name, string Code, string Type, bool IsStatutory);

public record PayrollRunDto(
    Guid Id,
    string PeriodName,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    DateTime? FinalizedDate,
    Guid? ApprovedBy
);

public record PayrollRunCreateDto(string PeriodName, DateTime StartDate, DateTime EndDate);

public record PayrollEntryDto(
    Guid Id,
    Guid PayrollRunId,
    Guid EmployeeId,
    string EmployeeName,
    Guid PayItemId,
    string PayItemName,
    decimal Amount,
    double Quantity
);

public record PayslipDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    Guid PayrollRunId,
    string PeriodName,
    decimal GrossPay,
    decimal TotalDeductions,
    decimal NetPay,
    DateTime IssuedDate,
    List<PayrollEntryDto> LineItems
);
