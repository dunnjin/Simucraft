using Simucraft.Server.Core;
using System.Collections.Generic;

namespace Simucraft.Server.Strategies
{
    public class RequestCombatPlacement : IGameStateStrategyRequest
    {
        public IEnumerable<GameCharacterRequest> GameCharacters { get; set; } = new List<GameCharacterRequest>();
    }
}
