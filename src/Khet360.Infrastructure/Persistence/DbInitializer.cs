using Khet360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khet360.Infrastructure.Persistence;

public class DbInitializer
{
    public static async Task InitializeDatabase(PlatformDbContext context, ILogger logger)
    {
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
                        SubscriptionPlanId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Tenant
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenant Beta",
                        Slug = "tenantb",
                        ConnectionString = "Server=localhost;Database=Khet360_TenantB;Trusted_Connection=True; TrustServerCertificate=True;",
                        IsActive = true,
                        SubscriptionPlanId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
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
