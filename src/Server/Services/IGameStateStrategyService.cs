using Simucraft.Server.Core;
using Simucraft.Server.Strategies;
using System;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IGameStateStrategyService
    {
        Task<GameStateInformationResponse> RequestAsync<T>(Guid gameId, Guid userId, T request)
            where T : IGameStateStrategyRequest;
    }
}
