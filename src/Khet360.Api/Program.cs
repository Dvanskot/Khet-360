using Khet360.Api.Middleware;
using Khet360.Application.Interfaces;
using Khet360.Infrastructure.Persistence;
using Khet360.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Platform Database Configuration
var connectionString = builder.Configuration.GetConnectionString("PlatformConnection");
builder.Services.AddDbContext<PlatformDbContext>(options =>
    options.UseSqlServer(connectionString));

// Tenant Service - Must be Scoped to persist tenant for the duration of the request
builder.Services.AddScoped<ITenantService, TenantService>();

var app = builder.Build();

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Tenant Resolver Middleware - Must run BEFORE authorization and controllers
app.UseMiddleware<TenantResolverMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();
