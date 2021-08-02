using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Client.Models
{
    public class RequestCombatPlacement
    {
        public ICollection<GameCharacter> GameCharacters { get; set; } = new List<GameCharacter>();
    }
}
