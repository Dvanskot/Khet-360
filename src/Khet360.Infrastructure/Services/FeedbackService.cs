using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Khet360.Infrastructure.BackgroundServices;

namespace Khet360.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;
    private readonly ILogger<FeedbackService> _logger;

    public FeedbackService(TenantDbContext db, ITenantService tenantService, ILogger<FeedbackService> logger)
    {
        _db = db;
        _tenantService = tenantService;
        _logger = logger;
    }

    public async Task SubmitFeedbackAsync(Guid userId, FeedbackCreateDto feedbackDto)
    {
        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Category = feedbackDto.Category,
            Message = feedbackDto.Message,
            Rating = feedbackDto.Rating,
            Status = FeedbackStatus.Submitted,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Feedbacks.Add(feedback);
        await _db.SaveChangesAsync();
    }

    public async Task<List<FeedbackDto>> GetTenantFeedbackAsync()
    {
        return await _db.Feedbacks
            .OrderByDescending(f => f.CreatedAtUtc)
            .Select(f => new FeedbackDto(
                f.Id,
                f.Category,
                f.Message,
                f.Rating,
                f.Status == FeedbackStatus.Resolved || f.Status == FeedbackStatus.Closed,
                f.Resolution,
                f.CreatedAtUtc))
            .ToListAsync();
    }

    public async Task ClaimFeedbackAsync(Guid feedbackId, Guid reviewerId)
    {
        var feedback = await _db.Feedbacks.FindAsync(feedbackId);
        if (feedback == null) throw new KeyNotFoundException("Feedback not found.");

        if (feedback.Status != FeedbackStatus.Submitted)
            throw new InvalidOperationException("Only submitted feedback can be claimed.");

        feedback.Status = FeedbackStatus.UnderReview;
        feedback.ReviewerId = reviewerId;

        await _db.SaveChangesAsync();
    }

    public async Task ResolveFeedbackAsync(Guid feedbackId, string resolution)
    {
        var feedback = await _db.Feedbacks.FindAsync(feedbackId);
        if (feedback == null) throw new KeyNotFoundException("Feedback not found.");

        feedback.Resolution = resolution;
        feedback.Status = FeedbackStatus.Resolved;

        await _db.SaveChangesAsync();

        // In a real app, we would publish a FeedbackResolvedEvent here to RabbitMQ
        _logger.LogInformation("Feedback {FeedbackId} resolved. Notification triggered.", feedbackId);
    }

    public async Task RateResolutionAsync(Guid feedbackId, ResolutionHelpfulness helpfulness)
    {
        var feedback = await _db.Feedbacks.FindAsync(feedbackId);
        if (feedback == null) throw new KeyNotFoundException("Feedback not found.");

        if (feedback.Status != FeedbackStatus.Resolved)
            throw new InvalidOperationException("Only resolved feedback can be rated.");

        feedback.ResolutionHelpfulness = helpfulness;
        feedback.Status = FeedbackStatus.Closed;

        await _db.SaveChangesAsync();
    }
}
