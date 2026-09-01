using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record FamilyCaseViewDto(
    Guid CaseId,
    string DeceasedName,
    string CurrentStatus,
    DateTime ScheduledDate,
    List<CaseMilestoneDto> Progress,
    List<ArrangementItemDto> SelectedItems,
    List<DocumentDto> UploadedDocuments,
    List<DocumentRequestDto> PendingDocuments,
    List<InvoiceDto> OutstandingInvoices);

public record CaseMilestoneDto(
    string MilestoneName,
    bool IsCompleted,
    DateTime? CompletionDate);

public record DocumentDto(
    Guid Id,
    string FileName,
    DateTime UploadedAt,
    string PresignedUrl);

public record DocumentRequestDto(
    Guid Id,
    string DocumentName,
    string Description,
    bool IsMandatory);

public record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    decimal Amount,
    DateTime DueDate,
    string Status);

public record TokenResponseDto(
    string Token,
    DateTime ExpiryDate);
