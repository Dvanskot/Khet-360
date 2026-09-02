using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Dtos;
using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Khet360.Infrastructure.Services;

public class VendorService : IVendorService
{
    private readonly TenantDbContext _db;
    private readonly ITenantService _tenantService;

    public VendorService(TenantDbContext db, ITenantService tenantService)
    {
        _db = db;
        _tenantService = tenantService;
    }

    public async Task<Guid> RegisterVendorAsync(VendorCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var vendor = new Vendor
        {
            Id = Guid.NewGuid(),
            CompanyName = dto.CompanyName,
            ContactName = dto.ContactName,
            Email = dto.Email,
            Phone = dto.Phone,
            Category = dto.Category,
            Status = VendorStatus.Pending,
            TaxId = dto.TaxId,
            BankDetails = dto.BankDetails,
            BranchId = branchId
        };

        _db.Vendors.Add(vendor);
        await _db.SaveChangesAsync();

        return vendor.Id;
    }

    public async Task<VendorDto?> GetVendorAsync(Guid id)
    {
        var v = await _db.Vendors.FindAsync(id);
        if (v == null) return null;

        return new VendorDto(v.Id, v.CompanyName, v.ContactName, v.Email, v.Phone, v.Category, v.Status);
    }

    public async Task<IEnumerable<VendorDto>> GetVendorsByCategoryAsync(string category, Guid branchId)
    {
        return await _db.Vendors
            .Where(v => v.Category == category && v.BranchId == branchId)
            .Select(v => new VendorDto(v.Id, v.CompanyName, v.ContactName, v.Email, v.Phone, v.Category, v.Status))
            .ToListAsync();
    }

    public async Task UpdateVendorStatusAsync(Guid id, VendorStatus status)
    {
        var v = await _db.Vendors.FindAsync(id);
        if (v == null) throw new KeyNotFoundException("Vendor not found.");

        v.Status = status;
        await _db.SaveChangesAsync();
    }

    public async Task<Guid> CreateOrderAsync(VendorOrderCreateDto dto, Guid branchId)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("Tenant context not found.");

        var order = new VendorOrder
        {
            Id = Guid.NewGuid(),
            OrderReference = dto.OrderReference,
            Status = VendorOrderStatus.Requested,
            OrderedAt = DateTime.UtcNow,
            VendorId = dto.VendorId,
            FuneralCaseId = dto.FuneralCaseId,
            Notes = dto.Notes,
            BranchId = branchId
        };

        if (dto.Items != null)
        {
            foreach (var itemDto in dto.Items)
            {
                order.Items.Add(new VendorOrderItem
                {
                    Id = Guid.NewGuid(),
                    ItemDescription = itemDto.ItemDescription,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    IsConfirmed = false,
                    BranchId = branchId
                });
            }
        }

        order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        _db.VendorOrders.Add(order);
        await _db.SaveChangesAsync();

        return order.Id;
    }

    public async Task<VendorOrderDto?> GetOrderAsync(Guid id)
    {
        var order = await _db.VendorOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        return new VendorOrderDto(
            order.Id,
            order.OrderReference,
            order.Status,
            order.OrderedAt,
            order.TotalAmount,
            order.VendorId,
            order.FuneralCaseId,
            order.Items.Select(i => new VendorOrderItemDto(i.Id, i.ItemDescription, i.Quantity, i.UnitPrice, i.IsConfirmed)).ToList());
    }

    public async Task UpdateOrderStatusAsync(Guid id, VendorOrderUpdateDto dto)
    {
        var order = await _db.VendorOrders.FindAsync(id);
        if (order == null) throw new KeyNotFoundException("Order not found.");

        order.Status = dto.Status;
        order.ConfirmedAt = dto.ConfirmedAt ?? order.ConfirmedAt;
        order.DeliveredAt = dto.DeliveredAt ?? order.DeliveredAt;
        order.Notes = dto.Notes ?? order.Notes;

        await _db.SaveChangesAsync();
    }

    public async Task ConfirmOrderItemAsync(Guid orderId, VendorItemConfirmationDto confirmation)
    {
        var item = await _db.VendorOrderItems.FindAsync(new object[] { confirmation.ItemId }); // Simplified lookup
        // In reality, we'd check if the item belongs to the orderId

        // Since VendorOrderItem doesn't have a composite key in this simple implementation, we find it directly
        // but for safety, we'll use a query.
        var itemActual = await _db.VendorOrderItems
            .FirstOrDefaultAsync(i => i.Id == confirmation.ItemId && i.VendorOrderId == orderId);

        if (itemActual == null) throw new KeyNotFoundException("Order item not found.");

        itemActual.IsConfirmed = confirmation.IsConfirmed;
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<VendorOrderDto>> GetOrdersByVendorAsync(Guid vendorId)
    {
        var orders = await _db.VendorOrders
            .Include(o => o.Items)
            .Where(o => o.VendorId == vendorId)
            .ToListAsync();

        return orders.Select(o => new VendorOrderDto(
            o.Id,
            o.OrderReference,
            o.Status,
            o.OrderedAt,
            o.TotalAmount,
            o.VendorId,
            o.FuneralCaseId,
            o.Items.Select(i => new VendorOrderItemDto(i.Id, i.ItemDescription, i.Quantity, i.UnitPrice, i.IsConfirmed)).ToList()));
    }
}
