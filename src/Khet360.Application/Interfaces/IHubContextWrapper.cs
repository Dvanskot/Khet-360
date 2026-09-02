using System;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IHubContextWrapper
{
    Task SendStateChangedAsync(Guid branchId, string entityType, Guid entityId, string action);
}
