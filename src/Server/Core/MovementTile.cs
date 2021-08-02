namespace Simucraft.Server.Core
{
    public class MovementTile : Coordinate
    {
        public MovementTile(int x, int y, int value)
            : base(x, y)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }
}
