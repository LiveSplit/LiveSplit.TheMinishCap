using System;
namespace LiveSplit.TheMinishCap
{
    [Flags]
    public enum InventoryItem : byte
    {
        None = 0,

        PauseMenu = 1 << 0,
        SmithsSword = 1 << 2,
        WhiteSword = 1 << 4,
        WhiteSword2 = 1 << 6,

        WhiteSword3 = 1 << 0,
        SwordLamp = 1 << 2,
        FourSword = 1 << 4,
        Bombs = 1 << 6,

        RemoteBombs = 1 << 0,
        Bow = 1 << 2,
        Bow2 = 1 << 4,
        Boomerang = 1 << 6,

        Boomerang2 = 1 << 0,
        Shield = 1 << 2,
        MirrorShield = 1 << 4,
        Lamp = 1 << 6,

        Lamp2 = 1 << 0,
        GustJar = 1 << 2,
        CaneOfPacci = 1 << 4,
        MoleMitts = 1 << 6,

        RocsCape = 1 << 0,
        PegasusBoots = 1 << 2,
        Unknown = 1 << 4,
        Ocarina = 1 << 6
    }
}
