using Khet360.Application.Interfaces;
using Khet360.Domain.Entities;
using Khet360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Khet360.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly TenantDbContext _tenantDb;
    private readonly ITenantUserContext _userContext;
    private readonly ITenantService _tenantService;
    private readonly ICacheService _cache;

    public CustomerService(TenantDbContext tenantDb, ITenantUserContext userContext, ITenantService tenantService, ICacheService cache)
    {
        _tenantDb = tenantDb;
        _userContext = userContext;
        _tenantService = tenantService;
        _cache = cache;
    }

    public async Task<Guid> CreateIndividualAsync(CreateIndividualRequest request)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        var customer = new IndividualCustomer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            IdentityNumber = request.IdentityNumber,
            IdentityType = request.IdentityType,
            BranchId = request.BranchId,
            CreatedAt = DateTime.UtcNow
        };

        customer.Addresses = request.Addresses.Select(a => new CustomerAddress
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Type = a.Type,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            Province = a.Province,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsPrimary = a.IsPrimary
        }).ToList();

        customer.Contacts = request.Contacts.Select(c => new CustomerContact
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Type = c.Type,
            Value = c.Value,
            IsPrimary = c.IsPrimary
        }).ToList();

        _tenantDb.Customers.Add(customer);
        await _tenantDb.SaveChangesAsync();

        return customer.Id;
    }

    public async Task<Guid> CreateOrganisationAsync(CreateOrganisationRequest request)
    {
        var tenantId = _tenantService.CurrentTenant?.Id
            ?? throw new InvalidOperationException("No tenant context found.");

        var customer = new OrganisationCustomer
        {
            Id = Guid.NewGuid(),
            OrganisationName = request.OrganisationName,
            RegistrationNumber = request.RegistrationNumber,
            TaxNumber = request.TaxNumber,
            Industry = request.Industry,
            BranchId = request.BranchId,
            CreatedAt = DateTime.UtcNow
        };

        customer.Addresses = request.Addresses.Select(a => new CustomerAddress
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Type = a.Type,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            Province = a.Province,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsPrimary = a.IsPrimary
        }).ToList();

        customer.Contacts = request.Contacts.Select(c => new CustomerContact
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Type = c.Type,
            Value = c.Value,
            IsPrimary = c.IsPrimary
        }).ToList();

        _tenantDb.Customers.Add(customer);
        await _tenantDb.SaveChangesAsync();

        return customer.Id;
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id)
    {
        string cacheKey = $"customer:{id}";
        var cachedCustomer = await _cache.GetAsync<CustomerDto>(cacheKey);
        if (cachedCustomer != null) return cachedCustomer;

        var customer = await _tenantDb.Customers
            .Include(c => c.Addresses)
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null) return null;

        var dto = MapToDto(customer);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30));

        return dto;
    }

    public async Task UpdateCustomerAsync(UpdateCustomerRequest request)
    {
        var customer = await _tenantDb.Customers
            .Include(c => c.Addresses)
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == request.Id);

        if (customer == null) throw new KeyNotFoundException("Customer not found.");

        if (customer is IndividualCustomer individual)
        {
            individual.FirstName = request.FirstName ?? individual.FirstName;
            individual.LastName = request.LastName ?? individual.LastName;
        }
        else if (customer is OrganisationCustomer org)
        {
            org.OrganisationName = request.OrganisationName ?? org.OrganisationName;
        }

        customer.Status = request.Status;
        customer.UpdatedAt = DateTime.UtcNow;

        // Update addresses and contacts (simplified: replace all)
        _tenantDb.CustomerAddresses.RemoveRange(customer.Addresses);
        customer.Addresses = request.Addresses.Select(a => new CustomerAddress
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Type = a.Type,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            Province = a.Province,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsPrimary = a.IsPrimary
        }).ToList();

        _tenantDb.CustomerContacts.RemoveRange(customer.Contacts);
        customer.Contacts = request.Contacts.Select(c => new CustomerContact
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Type = c.Type,
            Value = c.Value,
            IsPrimary = c.IsPrimary
        }).ToList();

        await _tenantDb.SaveChangesAsync();

        // Invalidate cache
        await _cache.RemoveAsync($"customer:{customer.Id}");
    }

    public async Task<PagedList<CustomerDto>> SearchCustomersAsync(CustomerSearchFilter filter)
    {
        var query = _tenantDb.Customers.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Query))
        {
            // Using casting instead of 'is' pattern matching for EF Core compatibility
            query = query.Where(c =>
                (c as IndividualCustomer != null && ((IndividualCustomer)c).FirstName.Contains(filter.Query)) ||
                (c as IndividualCustomer != null && ((IndividualCustomer)c).LastName.Contains(filter.Query)) ||
                (c as IndividualCustomer != null && ((IndividualCustomer)c).IdentityNumber.Contains(filter.Query)) ||
                (c as OrganisationCustomer != null && ((OrganisationCustomer)c).OrganisationName.Contains(filter.Query)));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(c => c.Status == filter.Status.Value);
        }

        if (filter.BranchId.HasValue)
        {
            query = query.Where(c => c.BranchId == filter.BranchId.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Include(c => c.Addresses)
            .Include(c => c.Contacts)
            .ToListAsync();

        return new PagedList<CustomerDto>(
            items.Select(MapToDto).ToList(),
            total,
            filter.Page,
            filter.PageSize);
    }

    private CustomerDto MapToDto(Customer customer)
    {
        string fullName = customer switch
        {
            IndividualCustomer ic => $"{ic.FirstName} {ic.LastName}",
            OrganisationCustomer oc => oc.OrganisationName,
            _ => "Unknown"
        };

        return new CustomerDto(
            customer.Id,
            fullName,
            customer is IndividualCustomer ? "Individual" : "Organisation",
            customer.Status,
            customer.BranchId,
            customer.Addresses.Select(a => new AddressDto(a.AddressLine1, a.AddressLine2, a.City, a.Province, a.PostalCode, a.Country, a.Type, a.IsPrimary)).ToList(),
            customer.Contacts.Select(c => new ContactDto(c.Value, c.Type, c.IsPrimary)).ToList()
        );
    }
}
