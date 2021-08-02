using System;

namespace Simucraft.Server.Strategies
{
    public class RequestAddCharacter : IGameStateStrategyRequest
    {
        public Guid CharacterId { get; set; }
    }
}
