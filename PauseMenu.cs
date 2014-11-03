using System.Runtime.InteropServices;

namespace LiveSplit.TheMinishCap
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PauseMenu
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        [FieldOffset(0)]
        public InventoryItem[] Inventory;

        [FieldOffset(0x10)]
        public Elements Elements;

        [FieldOffset(0x11)]
        public PermanentEquipment PermanentEquipment;
    }
}
