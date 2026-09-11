using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IInventoryService
{
    Task<int> GetStockLevelAsync(Guid productId, Guid branchId);
    Task UpdateStockAsync(Guid productId, Guid branchId, int quantityChanged, InventoryTransactionType type, Guid userId, string? correlationId = null, string? referenceId = null, string? notes = null);
    Task<IEnumerable<InventoryStock>> GetLowStockItemsAsync(Guid branchId);
    Task InitializeStockAsync(Guid productId, Guid branchId, int initialQuantity, Guid userId);
}
