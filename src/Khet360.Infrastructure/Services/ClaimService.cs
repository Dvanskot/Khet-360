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

namespace Khet360.Infrastructure.Services;

public class ClaimService : IClaimService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public ClaimService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Guid> CreateClaimAsync(ClaimCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var claim = new InsuranceClaim
        {
            Id = Guid.NewGuid(),
            ClaimNumber = dto.ClaimNumber,
            ClaimAmount = dto.ClaimAmount,
            Status = ClaimStatus.Submitted,
            SubmittedAt = DateTime.UtcNow,
            Notes = dto.Notes,
            PolicyId = dto.PolicyId,
            FuneralCaseId = dto.FuneralCaseId,
            TenantId = tenantId,
            BranchId = branchId
        };

        _db.InsuranceClaims.Add(claim);
        await _db.SaveChangesAsync();

        return claim.Id;
    }

    public async Task<ClaimDto?> GetClaimAsync(Guid id)
    {
        var claim = await _db.InsuranceClaims.FindAsync(id);
        if (claim == null) return null;

        return new ClaimDto(
            claim.Id,
            claim.ClaimNumber,
            claim.ClaimAmount,
            claim.Status,
            claim.SubmittedAt,
            claim.ProcessedAt,
            claim.Notes,
            claim.PolicyId,
            claim.FuneralCaseId);
    }

    public async Task UpdateClaimStatusAsync(Guid id, ClaimUpdateDto dto)
    {
        var claim = await _db.InsuranceClaims.FindAsync(id);
        if (claim == null) throw new KeyNotFoundException("Claim not found.");

        claim.Status = dto.Status;
        claim.ProcessedAt = dto.ProcessedAt ?? claim.ProcessedAt;
        claim.Notes = dto.Notes ?? claim.Notes;

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ClaimDto>> GetClaimsByPolicyAsync(Guid policyId)
    {
        return await _db.InsuranceClaims
            .Where(c => c.PolicyId == policyId)
            .Select(c => new ClaimDto(
                c.Id,
                c.ClaimNumber,
                c.ClaimAmount,
                c.Status,
                c.SubmittedAt,
                c.ProcessedAt,
                c.Notes,
                c.PolicyId,
                c.FuneralCaseId))
            .ToListAsync();
    }

    public async Task<IEnumerable<ClaimDto>> GetClaimsByCaseAsync(Guid funeralCaseId)
    {
        return await _db.InsuranceClaims
            .Where(c => c.FuneralCaseId == funeralCaseId)
            .Select(c => new ClaimDto(
                c.Id,
                c.ClaimNumber,
                c.ClaimAmount,
                c.Status,
                c.SubmittedAt,
                c.ProcessedAt,
                c.Notes,
                c.PolicyId,
                c.FuneralCaseId))
            .ToListAsync();
    }

    public async Task<Guid> AddPaymentAsync(ClaimPaymentCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var payment = new ClaimPayment
        {
            Id = Guid.NewGuid(),
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            TransactionReference = dto.TransactionReference,
            Notes = dto.Notes,
            ClaimId = dto.ClaimId,
            TenantId = tenantId,
            BranchId = branchId
        };

        _db.ClaimPayments.Add(payment);
        await _db.SaveChangesAsync();

        // Automatically move claim to 'Paid' if total payments >= claim amount
        var totalPaid = await _db.ClaimPayments
            .Where(p => p.ClaimId == dto.ClaimId)
            .SumAsync(p => p.Amount);

        var claim = await _db.InsuranceClaims.FindAsync(dto.ClaimId);
        if (claim != null && totalPaid >= claim.ClaimAmount)
        {
            claim.Status = ClaimStatus.Paid;
            await _db.SaveChangesAsync();
        }

        return payment.Id;
    }

    public async Task<IEnumerable<ClaimPaymentDto>> GetPaymentsForClaimAsync(Guid claimId)
    {
        return await _db.ClaimPayments
            .Where(p => p.ClaimId == claimId)
            .Select(p => new ClaimPaymentDto(
                p.Id,
                p.Amount,
                p.PaymentDate,
                p.TransactionReference,
                p.Notes,
                p.ClaimId))
            .ToListAsync();
    }
}
