using System.Collections.Generic;

namespace Simucraft.Client.Models
{
    public class GameInitialized
    {
        public IEnumerable<Map> Maps { get; set; } = new List<Map>();

        public IEnumerable<Character> Characters { get; set; } = new List<Character>();

        public bool IsOwner { get; set; }
    }
}
