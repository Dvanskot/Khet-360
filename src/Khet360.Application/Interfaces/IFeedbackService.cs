using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface IFeedbackService
{
    Task SubmitFeedbackAsync(Guid userId, FeedbackCreateDto feedbackDto);
    Task<List<FeedbackDto>> GetTenantFeedbackAsync();
    Task ResolveFeedbackAsync(Guid feedbackId, string resolution);
}
