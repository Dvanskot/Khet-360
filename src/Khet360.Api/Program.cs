using Khet360.Api.Middleware;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Khet360.Infrastructure.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Khet360.Api.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// MediatR Configuration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Khet360.Application.Interfaces.ITenantService).Assembly));

// Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("PlatformJwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:PlatformKey"] ?? "DefaultPlatformKey_MustChangeInProduction_12345!"))
        };
    })
    .AddJwtBearer("TenantJwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:TenantKey"] ?? "DefaultTenantKey_MustChangeInProduction_56789!"))
        };
    });

builder.Services.AddAuthorization();

// Platform Database Configuration
var connectionString = builder.Configuration.GetConnectionString("PlatformConnection");
builder.Services.AddDbContext<PlatformDbContext>(options =>
    options.UseSqlServer(connectionString));

// Tenant Service - Must be Scoped to persist tenant for the duration of the request
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IEntitlementService, EntitlementService>();
builder.Services.AddScoped<ITenantAuthService, TenantAuthService>();
builder.Services.AddScoped<IOrganisationService, OrganisationService>();
builder.Services.AddScoped<ITenantManagementService, TenantManagementService>();
builder.Services.AddScoped<ITenantUserContext, TenantUserContext>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<IOpportunityService, OpportunityService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IRoutingService, RoutingService>();
builder.Services.AddScoped<IFamilyRelationshipService, FamilyRelationshipService>();
builder.Services.AddScoped<IFuneralCaseService, FuneralCaseService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IServiceArrangementService, ServiceArrangementService>();
builder.Services.AddScoped<IFleetService, FleetService>();
builder.Services.AddScoped<IMortuaryService, MortuaryService>();
builder.Services.AddScoped<IRepatriationService, RepatriationService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IMemorialService, MemorialService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICommandPaletteService, CommandPaletteService>();
builder.Services.AddScoped<IDealBoardService, DealBoardService>();
builder.Services.AddScoped<IArrangementWizardService, ArrangementWizardService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IFileStorageService, MinioStorageService>();
builder.Services.AddScoped<IStateSyncService, StateSyncService>();
builder.Services.AddScoped<IProductivityScorecardService, ProductivityScorecardService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IHubContextWrapper, HubContextWrapper>();

builder.Services.AddHttpClient<IProductivityScorecardService, ProductivityScorecardService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Prometheus:Url"] ?? "http://localhost:9090");
});

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddSingleton<IMessageBus, MessageBus>();

builder.Services.AddHostedService<SlaEscalationWorker>();
builder.Services.AddHostedService<EventConsumerService>();
builder.Services.AddHostedService<NotificationConsumerService>();
builder.Services.AddScoped<TenantDbContextFactory>();
builder.Services.AddScoped<PlatformAuthService>();

// TenantDbContext - Resolved via factory to apply tenant-specific connection string
builder.Services.AddScoped<TenantDbContext>(sp =>
{
    var factory = sp.GetRequiredService<TenantDbContextFactory>();
    return factory.CreateDbContext();
});

var app = builder.Build();

// Seed the Platform Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PlatformDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        await DbInitializer.InitializeDatabase(context, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database during startup.");
    }
}

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMetricServer();

// Tenant Resolver Middleware - Must run BEFORE authorization and controllers
app.UseMiddleware<TenantResolverMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<Khet360.Api.Hubs.NotificationHub>("/hubs/notifications");
app.MapControllers();

app.Run();
