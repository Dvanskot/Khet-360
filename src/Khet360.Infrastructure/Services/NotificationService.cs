using System;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendNotificationAsync(Guid userId, string title, string message, NotificationPriority priority = NotificationPriority.Normal)
    {
        // In a real system, this would write to a Notification table or send a Push/Email/SMS
        _logger.LogInformation("Notification sent to User {UserId} [{Priority}]: {Title} - {Message}", userId, priority, title, message);
        await Task.CompletedTask;
    }
}
