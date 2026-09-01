using System.Collections.Generic;
using System.Threading.Tasks;
using Khet360.Application.Dtos;

namespace Khet360.Application.Interfaces;

public interface ICommandPaletteService
{
    Task<List<CommandActionDto>> GetAvailableCommandsAsync(string context = null);
}
