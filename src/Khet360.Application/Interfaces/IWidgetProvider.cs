using System;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IWidgetProvider
{
    string WidgetId { get; }
    Task<object> GetDataAsync(Guid tenantId, Guid userId);
}
