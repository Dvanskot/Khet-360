namespace Khet360.Domain.Enums;

public enum RepatriationStatus
{
    Requested,
    InTransit,
    ClearedCustoms,
    Arrived,
    Cancelled
}

public enum TransportMethod
{
    Air,
    Road,
    Rail,
    Sea
}
