using JetBrains.Annotations;

namespace JaroWinklerRecordSearch
{
    [PublicAPI]
    public static class BitOperations
    {
        private static readonly int[] BitcountsTable = InitializeBitcounts();

        private static int[] InitializeBitcounts()
        {
            var bitcounts = new int[65536];
            var (pos1, pos2) = (-1, -1);
            for (var i = 1; i < 65536; i++, pos1++)
            {
                if (pos1 == pos2)
                    (pos1, pos2) = (0, i);
                bitcounts[i] = bitcounts[pos1] + 1;
            }
            return bitcounts;
        }

        public static int BitCount64(ulong value)
        {
            //return (int)Popcnt.X64.PopCount(v);
            return BitCount32((uint)(value & 0x0000_0000_FFFF_FFFFF)) +
                   BitCount32((uint)((value >> 32) & 0x0000_0000_FFFF_FFFF));
        }

        public static int BitCount32(uint value)
        {
            return BitcountsTable[value & 0x0000_FFFF] + BitcountsTable[(value >> 16) & 0x0000_FFFF];
        }
    }
}