using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Domain.Enums;

namespace Khet360.Application.Interfaces;

public interface IVendorService
{
    Task<Guid> RegisterVendorAsync(VendorCreateDto dto, Guid branchId);
    Task<VendorDto?> GetVendorAsync(Guid id);
    Task<IEnumerable<VendorDto>> GetVendorsByCategoryAsync(string category, Guid branchId);
    Task UpdateVendorStatusAsync(Guid id, VendorStatus status);

    Task<Guid> CreateOrderAsync(VendorOrderCreateDto dto, Guid branchId);
    Task<VendorOrderDto?> GetOrderAsync(Guid id);
    Task UpdateOrderStatusAsync(Guid id, VendorOrderUpdateDto dto);
    Task ConfirmOrderItemAsync(Guid orderId, VendorItemConfirmationDto confirmation);
    Task<IEnumerable<VendorOrderDto>> GetOrdersByVendorAsync(Guid vendorId);
}
