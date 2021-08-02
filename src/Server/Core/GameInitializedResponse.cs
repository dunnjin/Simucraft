using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class GameInitializedResponse
    {
        public IEnumerable<CharacterResponse> Characters { get; set; } = new List<CharacterResponse>();
        public IEnumerable<MapResponse> Maps { get; set; } = new List<MapResponse>();

        public RulesetResponse Ruleset { get; set; }

        public bool IsOwner { get; set; }
    }
}
