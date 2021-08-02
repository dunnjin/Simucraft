using Simucraft.Server.Core;
using Simucraft.Server.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public class GameStrategyService : IGameStrategyService
    {
        private readonly IDictionary<Type, IGameStrategy> _gameStrategies;

        public GameStrategyService(IEnumerable<IGameStrategy> gameStrategies)
        {
            _gameStrategies = gameStrategies.ToDictionary(k => k.Type, v => v);
        }

        public Task<GameInformationResponse> RequestAsync<T>(Guid gameId, T request)
            where T : IGameStrategyRequest =>
            _gameStrategies[request.GetType()].RequestAsync(gameId, request);
    }
}
