using System;

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
