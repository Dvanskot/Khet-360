using System;
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
