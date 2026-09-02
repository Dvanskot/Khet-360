using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record DashboardWidgetDto(
    string WidgetId,
    string Title,
    int Width,
    int Height,
    int X,
    int Y,
    bool IsEnabled
);

public record UserDashboardLayoutDto(
    Guid UserId,
    List<DashboardWidgetDto> Widgets
);
