namespace Simucraft.Client.Common
{
    public static class ByteSize
    {
        private const int BYTES = 1024;

        public static long FromMegaBytes(int number) =>
            number * BYTES * BYTES;

        public static long FromKiloBytes(int number) =>
            number * BYTES;
    }
}
