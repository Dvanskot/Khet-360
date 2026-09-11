using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;

namespace Khet360.Application.Dtos;

public enum CommandActionType
{
    Navigate,
    ExecuteApi,
    OpenModal
}

public record CommandActionDto(
    string Id,
    string Label,
    string Description,
    string Category,
    CommandActionType ActionType,
    string Target,
    string? Icon = null);
