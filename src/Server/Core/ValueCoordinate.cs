namespace Simucraft.Server.Core
{
    public class ValueCoordinate : Coordinate
    {
        public ValueCoordinate(int x, int y, int value)
            : base(x, y)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }
}
