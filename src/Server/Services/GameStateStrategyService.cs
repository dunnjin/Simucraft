using Simucraft.Server.Core;
using Simucraft.Server.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public class GameStateStrategyService : IGameStateStrategyService
    {
        private readonly IDictionary<Type, IGameStateStrategy> _gameStrategies;

        public GameStateStrategyService(IEnumerable<IGameStateStrategy> gameStrategies)
        {
            _gameStrategies = gameStrategies.ToDictionary(k => k.Type, v => v);
        }

        public Task<GameStateInformationResponse> RequestAsync<T>(Guid gameId, Guid userId, T request)
            where T : IGameStateStrategyRequest =>
            _gameStrategies[request.GetType()].RequestAsync(gameId, userId, request);
    }
}
