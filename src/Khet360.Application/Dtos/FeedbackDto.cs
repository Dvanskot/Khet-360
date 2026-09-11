using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Dtos;

public record FeedbackCreateDto(
    string Category,
    string Message,
    int Rating
);

public record FeedbackDto(
    Guid Id,
    string Category,
    string Message,
    int Rating,
    bool IsResolved,
    string? Resolution,
    DateTime CreatedAtUtc
);
