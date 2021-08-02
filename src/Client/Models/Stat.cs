namespace Simucraft.Client.Models
{
    public class Stat
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public static Stat Empty =>
            new Stat
            {

            };
    }
}
