namespace Simucraft.Client.Models
{
    public class Position
    {
        public Position(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public static Position Empty =>
            new Position(0, 0);
    }
}
