using System.Collections.Generic;

namespace Simucraft.Client.Models
{
    public class RequestPlacement
    {
        public IEnumerable<GameCharacter> GameCharacters { get; set; } = new List<GameCharacter>();
    }
}
