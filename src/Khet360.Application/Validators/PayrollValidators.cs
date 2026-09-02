using FluentValidation;
using Khet360.Application.Dtos;

namespace Khet360.Application.Validators;

public class PayProfileCreateDtoValidator : AbstractValidator<PayProfileCreateDto>
{
    public PayProfileCreateDtoValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.BankName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AccountNumber).NotEmpty();
        RuleFor(x => x.BranchCode).NotEmpty();
        RuleFor(x => x.TaxNumber).NotEmpty();
    }
}

public class PayrollRunCreateDtoValidator : AbstractValidator<PayrollRunCreateDto>
{
    public PayrollRunCreateDtoValidator()
    {
        RuleFor(x => x.PeriodName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThan(x => x.StartDate);
    }
}
