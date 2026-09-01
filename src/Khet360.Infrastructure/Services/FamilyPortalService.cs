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

        var arrangements = await _db.ServiceArrangements
            .Include(s => s.Items)
            .Where(s => s.FuneralCaseId == funeralCase.Id)
            .ToListAsync();

        var selectedItems = arrangements.SelectMany(a => a.Items).ToList();

        var pendingDocs = await _db.DocumentRequests
            .Where(d => d.FuneralCaseId == funeralCase.Id && !d.IsFulfilled)
            .Select(d => new DocumentRequestDto(d.Id, d.DocumentName, d.Description, d.IsMandatory))
            .ToListAsync();

        var fulfilledDocs = await _db.DocumentRequests
            .Where(d => d.FuneralCaseId == funeralCase.Id && d.IsFulfilled && d.FileKey != null)
            .Select(d => new DocumentDto(
                d.Id,
                d.DocumentName,
                DateTime.UtcNow,
                _storage.GetPresignedUrl(d.FileKey!)))
            .ToListAsync();

        var invoices = await _db.Invoices
            .Where(i => i.FuneralCaseId == funeralCase.Id && i.Status != InvoiceStatus.Paid)
            .Select(i => new InvoiceDto(
                i.Id,
                i.InvoiceNumber,
                i.TotalAmount,
                i.DueDate,
                i.Status.ToString()))
            .ToListAsync();

        return new FamilyCaseViewDto(
            funeralCase.Id,
            funeralCase.DeceasedCustomer?.FullName ?? "Unknown Deceased",
            funeralCase.Status.ToString(),
            funeralCase.ScheduledDate ?? DateTime.MinValue,
            funeralCase.Milestones.Select(m => new CaseMilestoneDto(m.MilestoneStatus.ToString(), true, m.CompletedAt)).ToList(),
            selectedItems.Select(i => new ArrangementItemDto(i.Id, i.ItemName, i.Description, i.UnitPrice, i.Quantity, i.IsProvidedByFamily)).ToList(),
            fulfilledDocs,
            pendingDocs,
            invoices);
    }

    public async Task<Guid> UploadDocumentAsync(string token, System.IO.Stream fileStream, string fileName, string contentType, Guid documentRequestId)
    {
        var accessToken = await _db.CaseAccessTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.IsActive && t.ExpiryDate > DateTime.UtcNow);

        if (accessToken == null) throw new UnauthorizedAccessException("Invalid or expired access token.");

        var fileKey = await _storage.UploadFileAsync(fileStream, fileName, contentType, $"family-portal/{accessToken.FuneralCaseId}");

        var request = await _db.DocumentRequests.FindAsync(documentRequestId);
        if (request == null) throw new KeyNotFoundException("Document request not found.");

        request.FileKey = fileKey;
        request.IsFulfilled = true;
        await _db.SaveChangesAsync();

        return request.Id;
    }
}
