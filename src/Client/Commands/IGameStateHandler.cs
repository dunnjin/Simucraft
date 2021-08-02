using Simucraft.Client.Models;
using System.Threading.Tasks;

namespace Simucraft.Client.Commands
{
    /// <summary>
    /// Represents Mouse
    /// </summary>
    public interface IGameStateHandler
    {
        ClientState ClientState { get; }

        Task HandleAsync(Game game, Coordinate coordinate);
    }
}
