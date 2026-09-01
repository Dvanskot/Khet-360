using System;
using Khet360.Domain.Enums;

namespace Khet360.Application.Dtos;

public record RepatriationDto(
    Guid Id,
    string ReferenceNumber,
    RepatriationStatus Status,
    TransportMethod TransportMethod,
    string OriginCountry,
    string DestinationCountry,
    DateTime RequestedAt,
    DateTime? CompletedAt,
    string? Notes,
    Guid FuneralCaseId);

public record RepatriationCreateDto(
    string ReferenceNumber,
    TransportMethod TransportMethod,
    string OriginCountry,
    string DestinationCountry,
    Guid FuneralCaseId,
    string? Notes);

public record RepatriationUpdateDto(
    RepatriationStatus Status,
    DateTime? CompletedAt,
    string? Notes);
