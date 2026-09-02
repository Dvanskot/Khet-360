using System;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IOutboxService
{
    Task EnqueueAsync<T>(T eventMessage) where T : class;
}
