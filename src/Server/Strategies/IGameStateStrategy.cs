using Simucraft.Server.Core;
using System;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public interface IGameStateStrategy
    {
        Type Type { get; }

        Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, IGameStateStrategyRequest request);
    }

    public abstract class GameStateStrategy<T> : IGameStateStrategy
        where T : IGameStateStrategyRequest
    {
        Type IGameStateStrategy.Type => typeof(T);

        Task<GameStateInformationResponse> IGameStateStrategy.RequestAsync(Guid gameId, Guid userId, IGameStateStrategyRequest request) =>
            this.RequestAsync(gameId, userId, (T)request);

        public abstract Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, T request);
    }
}
