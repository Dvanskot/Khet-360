using System;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public record POSSaleRequest(
    Guid CustomerId,
    Guid BranchId,
    List<POSSaleItemRequest> Items,
    decimal PaymentAmount,
    string PaymentReference);

public record POSSaleItemRequest(
    Guid ProductId,
    int Quantity);

public interface IPOSService
{
    Task<Guid> CreateQuickSaleAsync(POSSaleRequest request);
}
