namespace Simucraft.Client.Models
{
    public class StatEffect
    {
        public string Stat { get; set; }

        public string Operator { get; set; }

        public string Effect { get; set; }

        public static StatEffect Empty =>
            new StatEffect
            {
                Operator = "+=",
            };
    }
}
