using System;

namespace Simucraft.Server.Models
{
    public interface IAggregateRoot
    {
        Guid Id { get; set; }
    }
}
