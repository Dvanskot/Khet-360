using System;
using System.Collections.Generic;
using Khet360.Domain.Enums;

namespace Khet360.Application.Dtos;

public record VendorDto(
    Guid Id,
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    string Category,
    VendorStatus Status);

public record VendorCreateDto(
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    string Category,
    string? TaxId,
    string? BankDetails);

public record VendorOrderDto(
    Guid Id,
    string OrderReference,
    VendorOrderStatus Status,
    DateTime OrderedAt,
    decimal TotalAmount,
    Guid VendorId,
    Guid FuneralCaseId,
    List<VendorOrderItemDto> Items);

public record VendorOrderCreateDto(
    string OrderReference,
    Guid VendorId,
    Guid FuneralCaseId,
    List<VendorOrderItemCreateDto> Items,
    string? Notes);

public record VendorOrderItemDto(
    Guid Id,
    string ItemDescription,
    int Quantity,
    decimal UnitPrice,
    bool IsConfirmed);

public record VendorOrderItemCreateDto(
    string ItemDescription,
    int Quantity,
    decimal UnitPrice);

public record VendorOrderUpdateDto(
    VendorOrderStatus Status,
    DateTime? ConfirmedAt,
    DateTime? DeliveredAt,
    string? Notes);

public record VendorItemConfirmationDto(
    Guid ItemId,
    bool IsConfirmed);
