using Khet360.Domain.Entities;
using Khet360.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface ICustomerService
{
    Task<Guid> CreateIndividualAsync(CreateIndividualRequest request);
    Task<Guid> CreateOrganisationAsync(CreateOrganisationRequest request);
    Task<CustomerDto?> GetCustomerByIdAsync(Guid id);
    Task UpdateCustomerAsync(UpdateCustomerRequest request);
    Task<PagedList<CustomerDto>> SearchCustomersAsync(CustomerSearchFilter filter);
}

public record CreateIndividualRequest(
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    string? Gender,
    string IdentityNumber,
    string? IdentityType,
    Guid BranchId,
    List<AddressDto> Addresses,
    List<ContactDto> Contacts);

public record CreateOrganisationRequest(
    string OrganisationName,
    string? RegistrationNumber,
    string? TaxNumber,
    string? Industry,
    Guid BranchId,
    List<AddressDto> Addresses,
    List<ContactDto> Contacts);

public record UpdateCustomerRequest(
    Guid Id,
    string? FirstName,
    string? LastName,
    string? OrganisationName,
    CustomerStatus Status,
    List<AddressDto> Addresses,
    List<ContactDto> Contacts);

public record CustomerDto(
    Guid Id,
    string FullName,
    string CustomerType,
    CustomerStatus Status,
    Guid BranchId,
    List<AddressDto> Addresses,
    List<ContactDto> Contacts);

public record AddressDto(string AddressLine1, string? AddressLine2, string City, string? Province, string PostalCode, string Country, AddressType Type, bool IsPrimary);
public record ContactDto(string Value, ContactType Type, bool IsPrimary);
public record CustomerSearchFilter(string? Query, CustomerStatus? Status, Guid? BranchId, int Page = 1, int PageSize = 20);
public record PagedList<T>(List<T> Items, int TotalCount, int Page, int PageSize);
