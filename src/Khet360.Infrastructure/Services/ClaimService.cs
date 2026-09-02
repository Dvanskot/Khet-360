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
    private readonly IInventoryService _inventoryService;
    private readonly ITenantUserContext _userContext;
    private readonly IProductionService _productionService;

    public ClaimService(TenantDbContext db, ITenantService tenantService, IInventoryService inventoryService, ITenantUserContext userContext, IProductionService productionService)
    {
        _db = db;
        _tenantService = tenantService;
        _inventoryService = inventoryService;
        _userContext = userContext;
        _productionService = productionService;
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

        // State Transition Validation
        if (claim.Status != dto.Status)
        {
            bool isValid = (claim.Status, dto.Status) switch
            {
                (ClaimStatus.Submitted, ClaimStatus.UnderReview) => true,
                (ClaimStatus.UnderReview, ClaimStatus.Approved) => true,
                (ClaimStatus.Approved, ClaimStatus.Paid) => true,
                (_, ClaimStatus.Rejected) => true,
                _ => false
            };

            if (!isValid)
            {
                throw new InvalidOperationException($"Invalid status transition from {claim.Status} to {dto.Status}.");
            }
        }

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

    public async Task ExecutePayoutAsync(Guid claimId)
    {
        var claim = await _db.InsuranceClaims.FindAsync(claimId);

        if (claim == null) throw new KeyNotFoundException("Claim not found.");

        var policy = await _db.InsurancePolicies.FindAsync(claim.PolicyId);
        if (policy == null) throw new KeyNotFoundException("Policy not found for this claim.");

        var plan = await _db.InsurancePolicyPlans.FindAsync(policy.PolicyPlanId);
        if (plan == null) throw new InvalidOperationException("Policy plan not configured for this claim.");

        await _db.Entry(policy).Collection(p => p.Members).LoadAsync();

        // Load benefits and items for the plan
        await _db.Entry(plan).Collection(pl => pl.Benefits).LoadAsync();
        foreach (var benefit in plan.Benefits)
        {
            await _db.Entry(benefit).Collection(b => b.BenefitItems).LoadAsync();
        }

        var funeralCase = await _db.FuneralCases.FindAsync(claim.FuneralCaseId);
        if (funeralCase == null) throw new KeyNotFoundException("Associated funeral case not found.");

        var deceasedCustomerId = funeralCase.DeceasedCustomerId
            ?? throw new InvalidOperationException("Deceased customer must be identified for insurance payout.");

        var member = policy.Members.FirstOrDefault(m => m.CustomerId == deceasedCustomerId);
        if (member == null) throw new InvalidOperationException("The deceased customer is not a registered member of the insurance policy.");

        var deceasedCustomer = await _db.Customers.FindAsync(deceasedCustomerId) as IndividualCustomer;
        int ageAtDeath = 0;
        if (deceasedCustomer?.DateOfBirth != null)
        {
            var today = DateTime.UtcNow;
            ageAtDeath = today.Year - deceasedCustomer.DateOfBirth.Value.Year;
            if (deceasedCustomer.DateOfBirth.Value.Date > today.AddYears(-ageAtDeath)) ageAtDeath--;
        }

        // 1. Resolve Fixed Benefits (apply regardless of age)
        var applicableBenefits = plan.Benefits
            .Where(b => b.Role == member.Role && b.IsFixed)
            .ToList();

        // 2. Resolve Age-Banded Benefit (apply based on age)
        var ageBandedBenefit = plan.Benefits
            .Where(b => b.Role == member.Role && !b.IsFixed && ageAtDeath >= b.MinAge && ageAtDeath <= b.MaxAge)
            .OrderByDescending(b => b.CoverAmount)
            .FirstOrDefault();

        if (ageBandedBenefit != null)
        {
            applicableBenefits.Add(ageBandedBenefit);
        }

        if (!applicableBenefits.Any())
        {
            throw new InvalidOperationException($"No valid benefits (fixed or age-banded) found for role {member.Role} and age {ageAtDeath} in plan {plan.Name}.");
        }

        // Aggregate total cover amount and all benefit items
        decimal totalCoverAmount = applicableBenefits.Sum(b => b.CoverAmount);
        var allBenefitItems = applicableBenefits.SelectMany(b => b.BenefitItems).ToList();

        switch (plan.CoverType)
        {
            case InsuranceCoverType.Burial:
                // Handle Burial Payout: Create ArrangementItems in the FuneralCase
                var arrangement = await _db.ServiceArrangements
                    .Where(a => a.FuneralCaseId == funeralCase.Id)
                    .FirstOrDefaultAsync();

                if (arrangement == null)
                {
                    arrangement = new ServiceArrangement
                    {
                        Id = Guid.NewGuid(),
                        FuneralCaseId = funeralCase.Id,
                        ArrangementName = "Insurance Plan Provision",
                        ScheduledDate = DateTime.UtcNow,
                        Location = "TBD",
                        Type = ArrangementType.Burial,
                        Description = $"Items provided by insurance plan: {plan.Name} for role {member.Role}",
                        BranchId = claim.BranchId
                    };
                    _db.ServiceArrangements.Add(arrangement);
                    await _db.SaveChangesAsync();
                }

                foreach (var benefitItem in allBenefitItems)
                {
                    var product = await _db.FuneralProducts.FindAsync(benefitItem.FuneralProductId);
                    if (product == null) continue;

                    var item = new ArrangementItem
                    {
                        Id = Guid.NewGuid(),
                        ItemName = product.Name,
                        Description = $"Provided by Insurance Plan {plan.Name} ({member.Role}): {product.Description}",
                        UnitPrice = product.DefaultPrice,
                        Quantity = benefitItem.Quantity,
                        IsProvidedByFamily = false,
                        ServiceArrangementId = arrangement.Id,
                        BranchId = claim.BranchId
                    };
                    _db.ArrangementItems.Add(item);

                    if (product.IsManufacturable)
                    {
                        var memorial = await _db.Memorials
                            .FirstOrDefaultAsync(m => m.FuneralCaseId == funeralCase.Id);

                        if (memorial != null)
                        {
                            await _productionService.CreateProductionOrderAsync(memorial.Id);
                        }
                    }
                    else
                    {
                        await _inventoryService.UpdateStockAsync(
                            benefitItem.FuneralProductId,
                            claim.BranchId,
                            -benefitItem.Quantity,
                            InventoryTransactionType.Adjustment,
                            _userContext.UserId ?? throw new UnauthorizedAccessException("User identity not found."),
                            $"CLAIM-{claim.ClaimNumber}",
                            benefitItem.FuneralProductId.ToString(),
                            $"Provisioned via insurance claim {claim.ClaimNumber}");
                    }
                }
                await _db.SaveChangesAsync();
                break;

            case InsuranceCoverType.Cash:
                // Handle Cash Payout: Record to Financial Ledger
                var transaction = new FinancialTransaction
                {
                    Id = Guid.NewGuid(),
                    Description = $"Insurance Payout for Claim {claim.ClaimNumber} (Member: {member.Role})",
                    TransactionDate = DateTime.UtcNow,
                    SourceEntityId = claim.Id,
                    SourceEntityType = "InsuranceClaim"
                };

                _db.FinancialTransactions.Add(transaction);

                decimal payoutAmount = totalCoverAmount;

                _db.FinancialEntries.Add(new FinancialEntry
                {
                    Id = Guid.NewGuid(),
                    FinancialTransactionId = transaction.Id,
                    AccountCode = "INS-EXP",
                    Debit = payoutAmount,
                    Credit = 0
                });

                _db.FinancialEntries.Add(new FinancialEntry
                {
                    Id = Guid.NewGuid(),
                    FinancialTransactionId = transaction.Id,
                    AccountCode = "CASH-BANK",
                    Debit = 0,
                    Credit = payoutAmount
                });
                break;

            default:
                throw new InvalidOperationException($"Unsupported cover type: {plan.CoverType}");
        }

        claim.Status = ClaimStatus.Paid;
        claim.ProcessedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
