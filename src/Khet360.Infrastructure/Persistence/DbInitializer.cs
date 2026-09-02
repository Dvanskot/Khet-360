using Khet360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Persistence;

public class DbInitializer
{
    public static async Task InitializeDatabase(PlatformDbContext context, ILogger logger)
    {
        Guid basicPlanId = Guid.Empty;
        Guid proPlanId = Guid.Empty;
        Guid enterprisePlanId = Guid.Empty;

        try
        {
            logger.LogInformation("Initializing Platform Database...");

            await context.Database.EnsureCreatedAsync();

            // Seed Tax Configuration
            if (!await context.TaxYears.AnyAsync())
            {
                logger.LogInformation("Seeding tax configuration...");
                var taxYear = new TaxYear
                {
                    Id = Guid.NewGuid(),
                    YearLabel = "2026/2027",
                    StartDate = new DateTime(2026, 3, 1),
                    EndDate = new DateTime(2027, 2, 28),
                    IsActive = true
                };
                context.TaxYears.Add(taxYear);

                // Seed Subscription Plans & Entitlements
                logger.LogInformation("Seeding subscription plans...");
                basicPlanId = Guid.NewGuid();
                proPlanId = Guid.NewGuid();
                enterprisePlanId = Guid.NewGuid();

                context.SubscriptionPlans.AddRange(
                    new SubscriptionPlan { Id = basicPlanId, Name = "Basic Plan", Description = "Essential tools for small operations", Category = PlanCategory.Basic, MonthlyPrice = 499m, AnnualPrice = 5000m, TrialPeriodDays = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new SubscriptionPlan { Id = proPlanId, Name = "Professional Plan", Description = "Advanced features for growing businesses", Category = PlanCategory.Professional, MonthlyPrice = 1499m, AnnualPrice = 15000m, TrialPeriodDays = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new SubscriptionPlan { Id = enterprisePlanId, Name = "Enterprise Plan", Description = "Unlimited scale and dedicated support", Category = PlanCategory.Enterprise, MonthlyPrice = 4999m, AnnualPrice = 50000m, TrialPeriodDays = 0, IsActive = true, CreatedAt = DateTime.UtcNow }
                );

                context.Entitlements.AddRange(
                    // Basic Limits
                    new Entitlement { Id = Guid.NewGuid(), SubscriptionPlanId = basicPlanId, Code = "MAX_BRANCHES", Description = "Maximum Branches", LimitValue = 2, IsActive = true },
                    new Entitlement { Id = Guid.NewGuid(), SubscriptionPlanId = basicPlanId, Code = "MAX_EMPLOYEES", Description = "Maximum Employees", LimitValue = 10, IsActive = true },
                    // Pro Limits
                    new Entitlement { Id = Guid.NewGuid(), SubscriptionPlanId = proPlanId, Code = "MAX_BRANCHES", Description = "Maximum Branches", LimitValue = 10, IsActive = true },
                    new Entitlement { Id = Guid.NewGuid(), SubscriptionPlanId = proPlanId, Code = "MAX_EMPLOYEES", Description = "Maximum Employees", LimitValue = 100, IsActive = true },
                    new Entitlement { Id = Guid.NewGuid(), SubscriptionPlanId = proPlanId, Code = "ADVANCED_REPORTING", Description = "Access to Advanced Reports", LimitValue = 1, IsActive = true },
                    // Enterprise Limits
                    new Entitlement { Id = Guid.NewGuid(), SubscriptionPlanId = enterprisePlanId, Code = "MAX_BRANCHES", Description = "Unlimited Branches", LimitValue = 9999, IsActive = true },
                    new Entitlement { Id = Guid.NewGuid(), SubscriptionPlanId = enterprisePlanId, Code = "MAX_EMPLOYEES", Description = "Unlimited Employees", LimitValue = 9999, IsActive = true },
                    new Entitlement { Id = Guid.NewGuid(), SubscriptionPlanId = enterprisePlanId, Code = "DEDICATED_SUPPORT", Description = "Dedicated Account Manager", LimitValue = 1, IsActive = true }
                );

                // Seed Platform Payment Config
                context.PlatformPaymentConfigs.Add(new PlatformPaymentConfig
                {
                    Id = Guid.NewGuid(),
                    ProviderName = "Stripe",
                    ApiKey = "sk_test_default",
                    SecretKey = "secret_test_default",
                    WebhookSecret = "whsec_default",
                    IsActive = true
                });

                context.TaxBrackets.AddRange(
                    new TaxBracket { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, LowerLimit = 0, UpperLimit = 237100, BaseAmount = 0, PercentageOverLowerLimit = 0.18m },
                    new TaxBracket { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, LowerLimit = 237101, UpperLimit = 370500, BaseAmount = 42678, PercentageOverLowerLimit = 0.26m },
                    new TaxBracket { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, LowerLimit = 370501, UpperLimit = 512800, BaseAmount = 77362, PercentageOverLowerLimit = 0.31m },
                    new TaxBracket { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, LowerLimit = 512801, UpperLimit = null, BaseAmount = 121475, PercentageOverLowerLimit = 0.36m }
                );

                context.TaxRebates.AddRange(
                    new TaxRebate { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, Type = RebateType.Primary, Amount = 17283, MinAge = 0 },
                    new TaxRebate { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, Type = RebateType.Secondary, Amount = 9452, MinAge = 65 },
                    new TaxRebate { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, Type = RebateType.Tertiary, Amount = 2650, MinAge = 75 }
                );

                context.StatutoryRates.AddRange(
                    new StatutoryRate { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, RateName = "UIF_Employee", Percentage = 0.01m },
                    new StatutoryRate { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, RateName = "UIF_Employer", Percentage = 0.01m },
                    new StatutoryRate { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, RateName = "SDL_Employer", Percentage = 0.01m },
                    new StatutoryRate { Id = Guid.NewGuid(), TaxYearId = taxYear.Id, RateName = "UIF_CappingLimit", CappingLimit = 17712m }
                );

                // Seed Standard Positions
                context.Positions.AddRange(
                    new Position { Id = Guid.NewGuid(), Title = "General Manager", Description = "Overall business management" },
                    new Position { Id = Guid.NewGuid(), Title = "HR Manager", Description = "Human resources and payroll management" },
                    new Position { Id = Guid.NewGuid(), Title = "Accountant", Description = "Financial records and accounting" },
                    new Position { Id = Guid.NewGuid(), Title = "Funeral Director", Description = "Coordination of funeral services" },
                    new Position { Id = Guid.NewGuid(), Title = "Driver", Description = "Transportation services" }
                );

                // Seed Standard Leave Types
                context.LeaveTypes.AddRange(
                    new LeaveType { Id = Guid.NewGuid(), Name = "Annual Leave", Code = "AL", IsPaid = true, AnnualAccrualRate = 21 },
                    new LeaveType { Id = Guid.NewGuid(), Name = "Sick Leave", Code = "SL", IsPaid = true, AnnualAccrualRate = 10 },
                    new LeaveType { Id = Guid.NewGuid(), Name = "Maternity Leave", Code = "MAT", IsPaid = true, AnnualAccrualRate = 0 },
                    new LeaveType { Id = Guid.NewGuid(), Name = "Family Responsibility Leave", Code = "FRL", IsPaid = true, AnnualAccrualRate = 3 }
                );

                await context.SaveChangesAsync();
            }

            if (!await context.Tenants.AnyAsync())
            {
                logger.LogInformation("Seeding initial tenants...");


                var seedTenants = new List<Tenant>
                {
                    new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant Alpha",
                        Slug = "tenanta",
                        ConnectionString = "Server=localhost;Database=Khet360_TenantA;Trusted_Connection=True; TrustServerCertificate=True;",
                        IsActive = true,
                        SubscriptionPlanId = basicPlanId,
                        SubscriptionStatus = SubscriptionStatus.Active,
                        SubscriptionStartDate = DateTime.UtcNow.AddMonths(-1),
                        SubscriptionEndDate = DateTime.UtcNow.AddMonths(11),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant Beta",
                        Slug = "tenantb",
                        ConnectionString = "Server=localhost;Database=Khet360_TenantB;Trusted_Connection=True; TrustServerCertificate=True;",
                        IsActive = true,
                        SubscriptionPlanId = proPlanId,
                        SubscriptionStatus = SubscriptionStatus.Active,
                        SubscriptionStartDate = DateTime.UtcNow.AddMonths(-2),
                        SubscriptionEndDate = DateTime.UtcNow.AddMonths(10),
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Tenants.AddRangeAsync(seedTenants);
                await context.SaveChangesAsync();

                logger.LogInformation("Successfully seeded {Count} tenants.", seedTenants.Count);
            }
            else
            {
                logger.LogInformation("Platform database already contains tenants. Skipping seed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the Platform Database.");
            throw;
        }
    }
}
