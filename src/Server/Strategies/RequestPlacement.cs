using Simucraft.Server.Core;
using System.Collections.Generic;

namespace Simucraft.Server.Strategies
{
    // TODO: Change to move and target single character, going to create an endpoint for each major action.
    public class RequestPlacement : IGameStateStrategyRequest
    {
        public IEnumerable<GameCharacterRequest> GameCharacters { get; set; } = new List<GameCharacterRequest>();
    }
}
