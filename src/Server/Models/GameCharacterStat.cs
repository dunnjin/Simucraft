using Microsoft.EntityFrameworkCore;

namespace Simucraft.Server.Models
{
    [Owned]
    public class GameCharacterStat
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
