using System;

namespace LiveSplit.TheMinishCap
{
    [Flags]
    public enum Elements : byte
    {
        None = 0,
        Earth = 1 << 0,
        Fire = 1 << 2,
        Water = 1 << 4,
        Wind = 1 << 6
    }
}
