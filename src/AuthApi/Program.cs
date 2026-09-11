using Khet360.Application.Interfaces;
using Khet360.AuthApi.Middleware;
using Khet360.Domain.Entities.Platform;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<Khet360.AuthApi.Filters.ValidationFilter>();
});
builder.Services.AddEndpointsApiExplorer();

// Add validators from the Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<Khet360.Application.Validators.LoginRequestValidator>();

// Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    // No default scheme; we will specify scheme on authorize attributes
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:PlatformKey"] ?? throw new InvalidOperationException("Jwt:PlatformKey is missing from configuration"))),
        ClockSkew = TimeSpan.Zero
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:TenantKey"] ?? throw new InvalidOperationException("Jwt:TenantKey is missing from configuration"))),
        ClockSkew = TimeSpan.Zero
    };
});

// DbContexts
var platformConnection = builder.Configuration.GetConnectionString("PlatformConnection");
builder.Services.AddDbContext<PlatformDbContext>(options =>
    options.UseSqlServer(platformConnection));

builder.Services.AddScoped<TenantDbContextFactory>();
builder.Services.AddScoped<TenantDbContext>(sp =>
{
    var factory = sp.GetRequiredService<TenantDbContextFactory>();
    return factory.CreateDbContext();
});

// Services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IPlatformAuthService, PlatformAuthService>();
builder.Services.AddScoped<ITenantAuthService, TenantAuthService>();

// Caching
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IPlatformCacheService, PlatformCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<TenantResolverMiddleware>();
app.UseMiddleware<TenantBindingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();