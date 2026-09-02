using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public FeedbackService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task SubmitFeedbackAsync(Guid userId, FeedbackCreateDto feedbackDto)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = tenantId,
            Category = feedbackDto.Category,
            Message = feedbackDto.Message,
            Rating = feedbackDto.Rating,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Feedbacks.Add(feedback);
        await _db.SaveChangesAsync();
    }

    public async Task<List<FeedbackDto>> GetTenantFeedbackAsync()
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        return await _db.Feedbacks
            .Where(f => f.TenantId == tenantId)
            .OrderByDescending(f => f.CreatedAtUtc)
            .Select(f => new FeedbackDto(
                f.Id,
                f.Category,
                f.Message,
                f.Rating,
                f.IsResolved,
                f.Resolution,
                f.CreatedAtUtc))
            .ToListAsync();
    }

    public async Task ResolveFeedbackAsync(Guid feedbackId, string resolution)
    {
        var feedback = await _db.Feedbacks.FindAsync(feedbackId);
        if (feedback == null) throw new KeyNotFoundException("Feedback not found.");

        feedback.Resolution = resolution;
        feedback.IsResolved = true;

        await _db.SaveChangesAsync();
    }
}
