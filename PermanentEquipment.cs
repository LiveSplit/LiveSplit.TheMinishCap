using System;

namespace LiveSplit.TheMinishCap
{
    [Flags]
    public enum PermanentEquipment : byte
    {
        None = 0,
        GripRing = 1 << 0,
        PowerBracelets = 1 << 2,
        Flippers = 1 << 4,
        Unknown = 1 << 6
    }
}
