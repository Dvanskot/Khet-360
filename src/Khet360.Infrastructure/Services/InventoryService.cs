using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly TenantDbContext _db;

    public InventoryService(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<int> GetStockLevelAsync(Guid productId, Guid branchId)
    {
        var stock = await _db.InventoryStocks
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.BranchId == branchId);
        return stock?.QuantityOnHand ?? 0;
    }

    public async Task UpdateStockAsync(Guid productId, Guid branchId, int quantityChanged, InventoryTransactionType type, Guid userId, string? correlationId = null, string? referenceId = null, string? notes = null)
    {
        var stock = await _db.InventoryStocks
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.BranchId == branchId);

        if (stock == null)
        {
            stock = new InventoryStock
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                BranchId = branchId,
                QuantityOnHand = 0,
                ReorderLevel = 5
            };
            _db.InventoryStocks.Add(stock);
        }

        stock.QuantityOnHand += quantityChanged;

        _db.InventoryTransactions.Add(new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            BranchId = branchId,
            QuantityChanged = quantityChanged,
            TransactionType = type,
            UserId = userId,
            CorrelationId = correlationId,
            ReferenceId = referenceId,
            Notes = notes,
            TimestampUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<InventoryStock>> GetLowStockItemsAsync(Guid branchId)
    {
        return await _db.InventoryStocks
            .Where(s => s.BranchId == branchId && s.QuantityOnHand <= s.ReorderLevel)
            .ToListAsync();
    }

    public async Task InitializeStockAsync(Guid productId, Guid branchId, int initialQuantity, Guid userId)
    {
        await UpdateStockAsync(productId, branchId, initialQuantity, InventoryTransactionType.Adjustment, userId, "INITIAL_STOCK", "Initializing stock levels");
    }
}
