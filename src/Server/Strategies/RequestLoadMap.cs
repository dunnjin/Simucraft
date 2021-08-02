using System;

namespace Simucraft.Server.Strategies
{
    public class RequestLoadMap : IGameStrategyRequest
    {
        public Guid MapId { get; set; }
    }
}
