using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string title, string message, NotificationPriority priority = NotificationPriority.Normal);
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Critical
}
