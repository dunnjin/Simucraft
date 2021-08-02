using Simucraft.Server.Core;

namespace Simucraft.Server.Strategies
{
    public class RequestUpdateGameCharacter : IGameStateStrategyRequest
    {
        public GameCharacterRequest GameCharacter { get; set; }
    }
}
