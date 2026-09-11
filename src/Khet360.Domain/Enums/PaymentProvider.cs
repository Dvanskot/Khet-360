namespace Khet360.Domain.Enums;

using Khet360.Domain.Entities.Tenant;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Common;

public enum PaymentProvider
{
    Netcash,
    Stripe,
    PayFast,
    PayPal,
    PeachPayments,
    Ozow,
    Yoco,
    Paystack
}
