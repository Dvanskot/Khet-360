using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
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
