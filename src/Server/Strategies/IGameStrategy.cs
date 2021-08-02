using Simucraft.Server.Core;
using System;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public interface IGameStrategy
    {
        Type Type { get; }

        Task<GameInformationResponse> RequestAsync(Guid gameId, IGameStrategyRequest request);
    }

    public abstract class GameStrategy<T> : IGameStrategy
        where T : IGameStrategyRequest
    {
        Type IGameStrategy.Type => typeof(T);

        Task<GameInformationResponse> IGameStrategy.RequestAsync(Guid gameId, IGameStrategyRequest request) =>
            this.ExecuteAsync(gameId, (T)request);

        public abstract Task<GameInformationResponse> ExecuteAsync(Guid gameId, T request);
    }
}
