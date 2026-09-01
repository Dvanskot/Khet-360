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

public class FamilyPortalService : IFamilyPortalService
{
    private readonly TenantDbContext _db;
    private readonly IFileStorageService _storage;

    public FamilyPortalService(TenantDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<TokenResponseDto> GenerateCaseAccessTokenAsync(Guid caseId)
    {
        var caseEntity = await _db.FuneralCases.FindAsync(caseId);
        if (caseEntity == null) throw new KeyNotFoundException("Funeral case not found.");

        var token = Guid.NewGuid().ToString("N");
        var expiry = DateTime.UtcNow.AddDays(30);

        var accessToken = new CaseAccessToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            ExpiryDate = expiry,
            FuneralCaseId = caseId,
            TenantId = caseEntity.TenantId,
            BranchId = caseEntity.BranchId
        };

        _db.CaseAccessTokens.Add(accessToken);
        await _db.SaveChangesAsync();

        return new TokenResponseDto(token, expiry);
    }

    public async Task<FamilyCaseViewDto?> GetCaseViewByTokenAsync(string token)
    {
        var accessToken = await _db.CaseAccessTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.IsActive && t.ExpiryDate > DateTime.UtcNow);

        if (accessToken == null) return null;

        var funeralCase = await _db.FuneralCases
            .Include(c => c.Milestones)
            .FirstOrDefaultAsync(c => c.Id == accessToken.FuneralCaseId);

        if (funeralCase == null) return null;

        // Get arrangements associated with this case (simplified)
        // In a real app, we'd filter for "Family-Visible" arrangements
        var arrangements = await _db.ServiceArrangements
            .Include(s => s.Items)
            .Where(s => s.FuneralCaseId == funeralCase.Id)
            .ToListAsync();

        var selectedItems = arrangements.SelectMany(a => a.Items).ToList();

        // Get pending documents
        var pendingDocs = await _db.DocumentRequests
            .Where(d => d.FuneralCaseId == funeralCase.Id && !d.IsFulfilled)
            .Select(d => new DocumentRequestDto(d.Id, d.DocumentName, d.Description, d.IsMandatory))
            .ToListAsync();

        // Note: In a real app, we would track which physical files in MinIO
        // correspond to which DocumentRequest. For now, we return an empty list of uploaded docs.

        return new FamilyCaseViewDto(
            funeralCase.Id,
            funeralCase.DeceasedCustomer?.FullName ?? "Unknown Deceased",
            funeralCase.Status.ToString(),
            funeralCase.ScheduledDate ?? DateTime.MinValue,
            funeralCase.Milestones.Select(m => new CaseMilestoneDto(m.MilestoneStatus.ToString(), true, m.CompletedAt)).ToList(),
            selectedItems.Select(i => new ArrangementItemDto(i.Id, i.ItemName, i.Description, i.UnitPrice, i.Quantity, i.IsProvidedByFamily)).ToList(),
            new List<DocumentDto>(),
            pendingDocs);
    }

    public async Task<Guid> UploadDocumentAsync(string token, System.IO.Stream fileStream, string fileName, string contentType)
    {
        var accessToken = await _db.CaseAccessTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.IsActive && t.ExpiryDate > DateTime.UtcNow);

        if (accessToken == null) throw new UnauthorizedAccessException("Invalid or expired access token.");

        var fileKey = await _storage.UploadFileAsync(fileStream, fileName, contentType, $"family-portal/{accessToken.FuneralCaseId}");

        // In a production system, the family would select WHICH DocumentRequest they are fulfilling.
        // For this implementation, we just upload the file.
        // Future: add a DocumentRequestId to the upload call.

        return Guid.NewGuid(); // Returning a dummy ID for the upload confirmation
    }
}
