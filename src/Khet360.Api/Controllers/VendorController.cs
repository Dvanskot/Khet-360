using Microsoft.AspNetCore.Mvc;
using Khet360.Application.Interfaces;
using Khet360.Application.Dtos;
using Khet360.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] VendorCreateDto dto, [FromQuery] Guid branchId)
    {
        var id = await _vendorService.RegisterVendorAsync(dto, branchId);
        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var vendor = await _vendorService.GetVendorAsync(id);
        if (vendor == null) return NotFound();
        return Ok(vendor);
    }

    [HttpGet("category")]
    public async Task<IActionResult> GetByCategory([FromQuery] string category, [FromQuery] Guid branchId)
    {
        var vendors = await _vendorService.GetVendorsByCategoryAsync(category, branchId);
        return Ok(vendors);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] VendorStatus status)
    {
        await _vendorService.UpdateVendorStatusAsync(id, status);
        return NoContent();
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] VendorOrderCreateDto dto, [FromQuery] Guid branchId)
    {
        var id = await _vendorService.CreateOrderAsync(dto, branchId);
        return Ok(id);
    }

    [HttpGet("orders/{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _vendorService.GetOrderAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPut("orders/{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] VendorOrderUpdateDto dto)
    {
        await _vendorService.UpdateOrderStatusAsync(id, dto);
        return NoContent();
    }

    [HttpPost("orders/confirm-item")]
    public async Task<IActionResult> ConfirmItem([FromBody] VendorItemConfirmationDto confirmation, [FromQuery] Guid orderId)
    {
        await _vendorService.ConfirmOrderItemAsync(orderId, confirmation);
        return NoContent();
    }

    [HttpGet("orders/vendor/{vendorId}")]
    public async Task<IActionResult> GetVendorOrders(Guid vendorId)
    {
        var orders = await _vendorService.GetOrdersByVendorAsync(vendorId);
        return Ok(orders);
    }
}
