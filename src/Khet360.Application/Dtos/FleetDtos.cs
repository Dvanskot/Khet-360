using System;
using System.Collections.Generic;

namespace Khet360.Application.Dtos;

public record TelematicsUpdateDto(
    double Latitude,
    double Longitude,
    double Speed,
    double FuelLevel,
    double EngineTemperature);

public record WorkOrderCreateDto(
    string Description,
    decimal EstimatedCost,
    Guid VehicleId);

public record FuelLogDto(
    DateTime PurchaseDate,
    double Volume,
    decimal Cost,
    double MileageAtPurchase,
    string? FuelCardNumber,
    string? ReceiptImageUrl,
    Guid VehicleId);

public record DriverProfileDto(
    string FullName,
    string LicenseNumber,
    DateTime LicenseExpiryDate,
    string ContactPhone,
    string Email,
    Guid UserId);

public record TripAssignmentDto(
    DateTime ScheduledStartTime,
    string RouteDetails,
    Guid VehicleId,
    Guid DriverId,
    Guid FuneralCaseId);

public record TripCompletionDto(
    DateTime ActualEndTime,
    string? DigitalSignature,
    string? DropOffNotes);

public record VehicleDocumentDto(
    string DocumentType,
    string DocumentNumber,
    DateTime ExpiryDate,
    Guid VehicleId);
