using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;

namespace Khet360.Application.Dtos;

public record ServiceArrangementDto(
    Guid Id,
    string ArrangementName,
    DateTime ScheduledDate,
    string Location,
    ArrangementType Type,
    string? Description,
    bool HasCatering,
    int ExpectedGuestCount,
    string? CateringNotes,
    CateringStatus CateringStatus,
    Guid FuneralCaseId,
    List<ArrangementItemDto> Items);

public record ServiceArrangementCreateDto(
    string ArrangementName,
    DateTime ScheduledDate,
    string Location,
    ArrangementType Type,
    string? Description,
    bool HasCatering,
    int ExpectedGuestCount,
    string? CateringNotes,
    Guid FuneralCaseId,
    List<ArrangementItemCreateDto> Items);

public record ServiceArrangementUpdateDto(
    string ArrangementName,
    DateTime ScheduledDate,
    string Location,
    ArrangementType Type,
    string? Description,
    bool HasCatering,
    int ExpectedGuestCount,
    string? CateringNotes,
    CateringStatus CateringStatus);

public record ArrangementItemDto(
    Guid Id,
    string ItemName,
    string? Description,
    decimal UnitPrice,
    int Quantity,
    bool IsProvidedByFamily);

public record ArrangementItemCreateDto(
    string ItemName,
    string? Description,
    decimal UnitPrice,
    int Quantity,
    bool IsProvidedByFamily);
