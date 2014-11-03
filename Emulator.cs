using System.Diagnostics;
using System.Linq;

namespace LiveSplit.TheMinishCap
{
    public class Emulator
    {
        public Process Process { get; protected set; }
        public int OffsetEWRAM { get; protected set; }
        public int OffsetIWRAM { get; protected set; }

        protected Emulator(Process process, int offsetEWRAM, int offsetIWRAM)
        {
            Process = process;
            OffsetEWRAM = offsetEWRAM;
            OffsetIWRAM = offsetIWRAM;
        }

        public static Emulator TryConnect()
        {
            var process = Process.GetProcessesByName("VisualBoyAdvance").FirstOrDefault();
            if (process != null)
            {
                return BuildVisualBoyAdvance(process);
            }

            return null;
        }

        private static Emulator Build(Process process, int _baseEWRAM, int _baseIWRAM)
        {
            var offsetEWRAM = ~new DeepPointer<int>(process, _baseEWRAM);
            var offsetIWRAM = ~new DeepPointer<int>(process, _baseIWRAM);

            return new Emulator(process, offsetEWRAM, offsetIWRAM);
        }

        private static Emulator BuildVisualBoyAdvance(Process process)
        {
            //var version = process.MainWindowTitle;
            var _baseEWRAM = (int)EmulatorBase.VisualBoyAdvance_EWRAM;
            var _baseIWRAM = (int)EmulatorBase.VisualBoyAdvance_IWRAM;

            return Build(process, _baseEWRAM, _baseIWRAM);
        }

        public DeepPointer<T> CreatePointer<T>(int address)
        {
            return CreatePointer<T>(1, address);
        }

        public DeepPointer<T> CreatePointer<T>(int length, int address)
        {
            var memorySection = address >> 24;
            var offset = 0;
            if (memorySection == 2)
                offset = OffsetEWRAM;
            else if (memorySection == 3)
                offset = OffsetIWRAM;
            return new DeepPointer<T>(length, Process, offset - (int)Process.MainModule.BaseAddress + (address & 0xFFFFFF));
        }
    }
}
