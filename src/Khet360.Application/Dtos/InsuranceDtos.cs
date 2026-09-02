using System;
using Khet360.Domain.Enums;

namespace Khet360.Application.Dtos;

public record PolicyMemberDto(
    Guid Id,
    Guid CustomerId,
    MemberRole Role,
    DateTime JoinedAt);

public record PolicyMemberCreateDto(
    Guid CustomerId,
    MemberRole Role);

public record PolicyDto(
    Guid Id,
    string PolicyNumber,
    string ProviderName,
    decimal CoverageAmount,
    DateTime StartDate,
    DateTime? EndDate,
    PolicyStatus Status,
    Guid PolicyPlanId,
    IReadOnlyList<PolicyMemberDto> Members);

public record PolicyCreateDto(
    string PolicyNumber,
    string ProviderName,
    decimal CoverageAmount,
    DateTime StartDate,
    DateTime? EndDate,
    Guid PolicyPlanId,
    IReadOnlyList<PolicyMemberCreateDto> Members);

public record PolicyUpdateDto(
    string PolicyNumber,
    decimal CoverageAmount,
    DateTime? EndDate,
    PolicyStatus Status);

public record ClaimDto(
    Guid Id,
    string ClaimNumber,
    decimal ClaimAmount,
    ClaimStatus Status,
    DateTime SubmittedAt,
    DateTime? ProcessedAt,
    string? Notes,
    Guid PolicyId,
    Guid FuneralCaseId);

public record ClaimCreateDto(
    string ClaimNumber,
    decimal ClaimAmount,
    Guid PolicyId,
    Guid FuneralCaseId,
    string? Notes);

public record ClaimUpdateDto(
    ClaimStatus Status,
    DateTime? ProcessedAt,
    string? Notes);

public record ClaimPaymentDto(
    Guid Id,
    decimal Amount,
    DateTime PaymentDate,
    string TransactionReference,
    string? Notes,
    Guid ClaimId);

public record ClaimPaymentCreateDto(
    decimal Amount,
    DateTime PaymentDate,
    string TransactionReference,
    string? Notes,
    Guid ClaimId);
