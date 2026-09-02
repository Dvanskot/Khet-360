using System;

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
