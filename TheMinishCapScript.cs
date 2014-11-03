using LiveSplit.Model;
using LiveSplit.TheMinishCap;
using System;

namespace LiveSplit.ASL
{
    public class TheMinishCapScript
    {
        protected TimerModel Model { get; set; }
        protected Emulator Emulator { get; set; }
        public ASLState OldState { get; set; }
        public ASLState State { get; set; }

        public TheMinishCapScript()
        {
            State = new ASLState();
        }

        protected void TryConnect()
        {
            if (Emulator == null || Emulator.Process.HasExited)
            {
                Emulator = Emulator.TryConnect();
                if (Emulator != null)
                {
                    Rebuild();
                    State.RefreshValues();
                    OldState = State;
                }
            }
        }

        private void Rebuild()
        {
            State.ValueDefinitions.Clear();

            var gameVersion = GetGameVersion();

            switch (gameVersion)
            {
                case GameVersion.NTSCJ: RebuildNTSCJ(); break;
                default: Emulator = null; break;
            }
        }

        private void RebuildNTSCJ()
        {
            AddPointer<PauseMenu>("PauseMenu", 0x2002B32);
            AddPointer<Scene>("Scene", 0x3000BF4);
            AddPointer<int>("DHCBigKey", 0x2002EB2);
            AddPointer<int>("Vaati3Phases", 0x30017BC);
            AddPointer<Sprite>("Sprite", 0x300116C);
            AddPointer<int>("FrameCount", 0x300100C);
            AddPointer<int>("UIXPosition", 0x3001E4E);
            AddPointer<int>("UIYPosition", 0x300187A);
        }

        private GameVersion GetGameVersion()
        {
            return GameVersion.NTSCJ;
        }

        private void AddPointer<T>(String name, int address)
        {
            AddPointer<T>(1, name, address);
        }

        private void AddPointer<T>(int length, String name, int address)
        {
            State.ValueDefinitions.Add(new ASLValueDefinition()
                {
                    Identifier = name,
                    Pointer = Emulator.CreatePointer<T>(length, address)
                });
        }

        public void Update(LiveSplitState lsState)
        {
            if (Emulator != null && !Emulator.Process.HasExited)
            {
                OldState = State.RefreshValues();

                if (lsState.CurrentPhase == TimerPhase.NotRunning)
                {
                    if (Start(lsState, OldState.Data, State.Data))
                    {
                        Model.Start();
                    }
                }
                else if (lsState.CurrentPhase == TimerPhase.Running || lsState.CurrentPhase == TimerPhase.Paused)
                {
                    if (Reset(lsState, OldState.Data, State.Data))
                    {
                        Model.Reset();
                        return;
                    }
                    else if (Split(lsState, OldState.Data, State.Data))
                    {
                        Model.Split();
                    }

                    var isPaused = IsPaused(lsState, OldState.Data, State.Data);
                    if (isPaused != null)
                        lsState.IsGameTimePaused = isPaused;

                    var gameTime = GameTime(lsState, OldState.Data, State.Data);
                    if (gameTime != null)
                        lsState.SetGameTime(gameTime);
                }
            }
            else
            {
                if (Model == null)
                {
                    Model = new TimerModel() { CurrentState = lsState };
                }
                TryConnect();
            }
        }

        public bool Start(LiveSplitState timer, dynamic old, dynamic current)
        {
            current.accumulatedFrameCount = -current.FrameCount;
            //Check for Timer Start
            return current.UIXPosition == 24 && old.UIYPosition == 144 && current.UIYPosition > 144;
        }

        public bool Split(LiveSplitState timer, dynamic old, dynamic current)
        {
            Func<InventorySlot,InventoryItem,bool> hadItem = (x,y) => old.PauseMenu.Inventory[(int)x].HasFlag(y);
            Func<InventorySlot,InventoryItem,bool> hasItem = (x,y) => current.PauseMenu.Inventory[(int)x].HasFlag(y);
            Func<Elements,bool> hasElement = (x) => current.PauseMenu.Elements.HasFlag(x);
            Func<PermanentEquipment, bool> hasEquipment = (x) => current.PauseMenu.PermanentEquipment.HasFlag(x);

            var segment = timer.CurrentSplit.Name.ToLower();
            if (segment == "enter dws" || segment == "enter deepwood shrine")
                return old.Scene != current.Scene && current.Scene == Scene.DeepwoodShrine;
            else if (segment == "ezlo")
            {
                if (current.Sprite != old.Sprite && current.Sprite == Sprite.ReceiveMinishCap)
                    current.TimeStamp = current.FrameCount;

                return current.Scene == Scene.MinishWoods
                    && current.Sprite == Sprite.ReceiveMinishCap
                    && (current.FrameCount - current.TimeStamp) >= 20;
            }
            else if (segment == "gust jar")
                return hasItem(InventorySlot.GustJar, InventoryItem.GustJar);
            else if (segment.Contains("earth"))
                return hasElement(Elements.Earth);
            else if (segment.Contains("ring"))
                return hasEquipment(PermanentEquipment.GripRing);
            else if (segment == "enter cof" || segment == "enter cave of flames")
                return old.Scene != current.Scene && current.Scene == Scene.CaveOfFlames;
            else if (segment == "cane of pacci")
                return hasItem(InventorySlot.CaneOfPacci, InventoryItem.CaneOfPacci);
            else if (segment.Contains("fire"))
                return hasElement(Elements.Fire);
            else if (segment.Contains("boots"))
                return hasItem(InventorySlot.PegasusBoots, InventoryItem.PegasusBoots);
            else if (segment.Contains("fortress") || segment.Contains("fow"))
                return old.Scene != current.Scene && current.Scene == Scene.FortressOfWinds;
            else if (segment.Contains("mitts") || segment.Contains("mole"))
                return hasItem(InventorySlot.MoleMitts, InventoryItem.MoleMitts);
            else if (segment.Contains("ocarina"))
                return hasItem(InventorySlot.Ocarina, InventoryItem.Ocarina);
            else if (segment.Contains("flippers"))
                return hasEquipment(PermanentEquipment.Flippers);
            else if (segment == "enter tod" || segment == "enter temple of droplets")
                return old.Scene != current.Scene && current.Scene == Scene.TempleOfDroplets;
            else if (segment.Contains("lantern"))
                return hasItem(InventorySlot.Lamp, InventoryItem.Lamp);
            else if (segment.Contains("water"))
                return hasElement(Elements.Water);
            else if (segment == "enter pow" || segment == "enter palace of winds")
                return old.Scene != current.Scene && current.Scene == Scene.PalaceOfWinds;
            else if (segment.Contains("cape"))
                return hasItem(InventorySlot.RocsCape, InventoryItem.RocsCape);
            else if (segment.Contains("wind"))
                return hasElement(Elements.Wind);
            else if (segment == "four sword")
            {
                if (hasItem(InventorySlot.FourSword, InventoryItem.FourSword) && !hadItem(InventorySlot.FourSword, InventoryItem.FourSword))
                    current.TimeStamp = current.FrameCount;

                return hasItem(InventorySlot.FourSword, InventoryItem.FourSword)
                    && (current.FrameCount - current.TimeStamp) >= 244;
            }
            else if ((segment.Contains("dark hyrule castle") || segment.Contains("dhc")) && (segment.Contains("big key") || segment.Contains("bk") || segment.Contains("boss key")))
                return (current.DHCBigKey & 4) == 4;
            else if (segment.Contains("vaati"))
                return current.Scene == Scene.Vaati3 && old.Vaati3Phases == 1 && current.Vaati3Phases == 0;

            return false;
        }

        public bool Reset(LiveSplitState timer, dynamic old, dynamic current)
        {
            return false;
        }

        public bool IsPaused(LiveSplitState timer, dynamic old, dynamic current)
        {
            return true;
        }

        public TimeSpan? GameTime(LiveSplitState timer, dynamic old, dynamic current)
        {
            if (current.FrameCount < old.FrameCount)
                current.accumulatedFrameCount += old.FrameCount;

            return TimeSpan.FromSeconds((current.accumulatedFrameCount + current.FrameCount) / 60.0);
        }
    }
}
