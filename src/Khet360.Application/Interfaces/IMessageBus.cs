using System;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IMessageBus
{
    Task PublishAsync<T>(T message) where T : class;
    Task PublishRawAsync(string content);
}
