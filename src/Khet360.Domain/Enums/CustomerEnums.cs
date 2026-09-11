namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public enum CustomerStatus
{
    Active = 0,
    Inactive = 1,
    Suspended = 2,
    Blacklisted = 3
}

public enum RelationshipType
{
    Payer = 0,
    Spouse = 1,
    Partner = 2,
    Parent = 3,
    Child = 4,
    NextOfKin = 5,
    Deceased = 6,
    PolicyMember = 7,
    Beneficiary = 8
}

public enum AddressType
{
    Home = 0,
    Work = 1,
    Billing = 2,
    Shipping = 3
}

public enum ContactType
{
    Email = 0,
    Phone = 1,
    WhatsApp = 2
}
