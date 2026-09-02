using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public interface IProductionService
{
    Task<Guid> CreateProductionOrderAsync(Guid memorialId);
    Task<ProductionOrderDto> GetProductionOrderAsync(Guid id);
    Task<List<ProductionOrderDto>> GetActiveOrdersAsync();
    Task AdvanceStageAsync(Guid orderId, Guid artisanId);
    Task LogTimeAsync(Guid orderId, Guid artisanId, double hours, string notes);
    Task PerformQualityCheckAsync(Guid orderId, Guid inspectorId, bool passed, string comments);
}

public record ProductionOrderDto(
    Guid Id,
    Guid MemorialId,
    ProductionStage CurrentStage,
    ProductionStatus Status,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc,
    List<ProductionLogDto> Logs
);

public record ProductionLogDto(Guid Id, ProductionStage Stage, string ArtisanName, DateTime StartedAtUtc, DateTime EndedAtUtc, double DurationHours, string? Notes);

public class ProductionService : IProductionService
{
    private readonly TenantDbContext _db;

    public ProductionService(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> CreateProductionOrderAsync(Guid memorialId)
    {
        var order = new ProductionOrder
        {
            Id = Guid.NewGuid(),
            MemorialId = memorialId,
            CurrentStage = ProductionStage.OrderConfirmed,
            Status = ProductionStatus.InProgress,
            StartedAtUtc = DateTime.UtcNow
        };
        _db.ProductionOrders.Add(order);
        await _db.SaveChangesAsync();
        return order.Id;
    }

    public async Task<ProductionOrderDto> GetProductionOrderAsync(Guid id)
    {
        var order = await _db.ProductionOrders
            .Include(o => o.Logs)
            .ThenInclude(l => l.Artisan)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) throw new KeyNotFoundException("Production order not found.");

        return new ProductionOrderDto(
            order.Id,
            order.MemorialId,
            order.CurrentStage,
            order.Status,
            order.StartedAtUtc,
            order.CompletedAtUtc,
            order.Logs.Select(l => new ProductionLogDto(l.Id, l.Stage, l.Artisan.FirstName + " " + l.Artisan.LastName, l.StartedAtUtc, l.EndedAtUtc, l.DurationHours, l.Notes)).ToList()
        );
    }

    public async Task<List<ProductionOrderDto>> GetActiveOrdersAsync()
    {
        var orders = await _db.ProductionOrders
            .Where(o => o.Status == ProductionStatus.InProgress)
            .Include(o => o.Logs)
            .ThenInclude(l => l.Artisan)
            .ToListAsync();

        return orders.Select(o => new ProductionOrderDto(
            o.Id,
            o.MemorialId,
            o.CurrentStage,
            o.Status,
            o.StartedAtUtc,
            o.CompletedAtUtc,
            o.Logs.Select(l => new ProductionLogDto(l.Id, l.Stage, l.Artisan.FirstName + " " + l.Artisan.LastName, l.StartedAtUtc, l.EndedAtUtc, l.DurationHours, l.Notes)).ToList()
        )).ToList();
    }

    public async Task AdvanceStageAsync(Guid orderId, Guid artisanId)
    {
        var order = await _db.ProductionOrders.FindAsync(orderId);
        if (order == null) throw new KeyNotFoundException("Production order not found.");

        // Advance stage
        var nextStage = order.CurrentStage switch
        {
            ProductionStage.OrderConfirmed => ProductionStage.SlabSelection,
            ProductionStage.SlabSelection => ProductionStage.CuttingShaping,
            ProductionStage.CuttingShaping => ProductionStage.Polishing,
            ProductionStage.Polishing => ProductionStage.Engraving,
            ProductionStage.Engraving => ProductionStage.Finishing,
            ProductionStage.Finishing => ProductionStage.QualityCheck,
            ProductionStage.QualityCheck => ProductionStage.ReadyForDelivery,
            ProductionStage.ReadyForDelivery => ProductionStage.ReadyForDelivery,
            _ => order.CurrentStage
        };

        order.CurrentStage = nextStage;

        if (nextStage == ProductionStage.ReadyForDelivery)
        {
            order.Status = ProductionStatus.Completed;
            order.CompletedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task LogTimeAsync(Guid orderId, Guid artisanId, double hours, string notes)
    {
        var order = await _db.ProductionOrders.FindAsync(orderId);
        if (order == null) throw new KeyNotFoundException("Production order not found.");

        var log = new ProductionLog
        {
            Id = Guid.NewGuid(),
            ProductionOrderId = orderId,
            Stage = order.CurrentStage,
            ArtisanId = artisanId,
            StartedAtUtc = DateTime.UtcNow.AddHours(-hours),
            EndedAtUtc = DateTime.UtcNow,
            DurationHours = hours,
            Notes = notes
        };

        _db.ProductionLogs.Add(log);
        await _db.SaveChangesAsync();
    }

    public async Task PerformQualityCheckAsync(Guid orderId, Guid inspectorId, bool passed, string comments)
    {
        var order = await _db.ProductionOrders.FindAsync(orderId);
        if (order == null) throw new KeyNotFoundException("Production order not found.");

        if (order.CurrentStage != ProductionStage.QualityCheck)
            throw new InvalidOperationException("Order must be in QualityCheck stage.");

        var check = new QualityCheck
        {
            Id = Guid.NewGuid(),
            ProductionOrderId = orderId,
            Stage = ProductionStage.QualityCheck,
            Passed = passed,
            Comments = comments,
            InspectorId = inspectorId,
            CheckedAtUtc = DateTime.UtcNow
        };

        _db.QualityChecks.Add(check);

        if (passed)
        {
            await AdvanceStageAsync(orderId, inspectorId);
        }
        else
        {
            order.Status = ProductionStatus.OnHold;
        }

        await _db.SaveChangesAsync();
    }
}
