using Simucraft.Server.Core;
using Simucraft.Server.Strategies;
using System;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IGameStrategyService
    {
        Task<GameInformationResponse> RequestAsync<T>(Guid gameId, T request)
            where T : IGameStrategyRequest;
    }
}
