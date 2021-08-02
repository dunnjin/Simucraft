using System;

namespace Simucraft.Server.Strategies
{
    public class RequestCombatMove : IGameStateStrategyRequest
    {
        public Guid GameCharacterId { get; set; }
    }
}
