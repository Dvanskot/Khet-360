using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Khet360.Api.Controllers.Platform;

[ApiController]
[Route("platform/api/public")]
[AllowAnonymous]
public class PublicSubscriptionController : ControllerBase
{
    private readonly PlatformDbContext _platformDb;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ITenantManagementService _tenantManagementService;
    private readonly IPlatformPaymentService _platformPaymentService;

    public PublicSubscriptionController(
        PlatformDbContext platformDb,
        ISubscriptionService subscriptionService,
        ITenantManagementService tenantManagementService,
        IPlatformPaymentService platformPaymentService)
    {
        _platformDb = platformDb;
        _subscriptionService = subscriptionService;
        _tenantManagementService = tenantManagementService;
        _platformPaymentService = platformPaymentService;
    }

    [HttpGet("plans")]
    public async Task<ActionResult<IEnumerable<object>>> GetPublicPlans()
    {
        var plans = await _platformDb.SubscriptionPlans
            .Where(p => p.IsActive)
            .Select(p => new {
                p.Id,
                p.Name,
                p.Description,
                p.Category,
                p.MonthlyPrice,
                p.AnnualPrice,
                p.TrialPeriodDays,
                Entitlements = p.Entitlements
                    .Where(e => e.IsActive)
                    .Select(e => new { e.Code, e.Description, e.LimitValue })
            })
            .ToListAsync();

        return Ok(plans);
    }

    [HttpPost("trial")]
    public async Task<ActionResult> StartFreeTrial([FromBody] TrialSignupDto dto)
    {
        var plan = await _platformDb.SubscriptionPlans.FindAsync(dto.SubscriptionPlanId);
        if (plan == null || !plan.IsActive) return BadRequest("Invalid or inactive subscription plan.");

        var tenant = await _tenantManagementService.CreateTenantAsync(
            dto.CompanyName,
            dto.Slug,
            dto.SubscriptionPlanId,
            IsolationTier.Isolated);

        return Ok(new {
            Message = "Free trial started successfully!",
            TenantId = tenant.Id,
            TrialEndDate = tenant.TrialEndDate
        });
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult> InitiateSubscription([FromBody] SubscribeDto dto)
    {
        var plan = await _platformDb.SubscriptionPlans.FindAsync(dto.SubscriptionPlanId);
        if (plan == null || !plan.IsActive) return BadRequest("Invalid or inactive subscription plan.");

        try
        {
            var paymentLink = await _platformPaymentService.CreateSubscriptionPaymentLinkAsync(
                dto.SubscriptionPlanId,
                dto.Email,
                dto.CompanyName);

            return Ok(new {
                Message = "Payment initiated. Please complete the payment to activate your account.",
                PaymentLink = paymentLink,
                Plan = plan.Name,
                Amount = plan.MonthlyPrice
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Payment initialization failed: {ex.Message}");
        }
    }

    [HttpPost("webhook/payment-success")]
    public async Task<IActionResult> HandlePaymentWebhook([FromBody] PaymentWebhookDto dto)
    {
        var isValid = await _platformPaymentService.VerifySubscriptionPaymentAsync(dto.TransactionReference, dto.Amount);
        if (!isValid) return BadRequest("Invalid payment verification.");

        var tenant = await _tenantManagementService.CreateTenantAsync(
            dto.CompanyName,
            dto.Slug,
            dto.SubscriptionPlanId,
            IsolationTier.Isolated);

        await _subscriptionService.ActivateSubscriptionAsync(tenant.Id, 1);

        return Ok(new { Message = "Tenant provisioned and subscription activated." });
    }
}

public record TrialSignupDto(string CompanyName, string Slug, Guid SubscriptionPlanId, string Email);
public record SubscribeDto(string CompanyName, string Slug, Guid SubscriptionPlanId, string Email);
public record PaymentWebhookDto(string TransactionReference, decimal Amount, string CompanyName, string Slug, Guid SubscriptionPlanId);
