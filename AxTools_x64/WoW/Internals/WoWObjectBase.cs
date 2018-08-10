using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.PluginSystem.API;
using FMemory;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AxTools.WoW.Internals
{
    public class WoWObjectBase
    {
        protected MemoryManager memory;
        protected WowProcess wowProcess;
        protected GameInterface info;

        internal WoWObjectBase(WowProcess wow)
        {
            memory = wow.Memory;
            wowProcess = wow;
        }

        public virtual WoWGUID GUID { get; protected set; }
        public virtual string Name { get; protected set; }

        private void SetMouseoverUnit(WoWGUID guid)
        {
            WoWGUID mouseoverGUID = memory.Read<WoWGUID>(memory.ImageBase + WowBuildInfoX64.MouseoverGUID);
            if (mouseoverGUID != guid)
            {
                memory.Write(memory.ImageBase + WowBuildInfoX64.MouseoverGUID, guid);
            }
        }

        public void Interact()
        {
            wowProcess.WaitWhileWoWIsMinimized();
            info = info ?? new GameInterface(wowProcess);
            if (info.IsInGame)
            {
                if (Settings2.Instance.WoWInteractMouseover != Keys.None)
                {
                    SetMouseoverUnit(GUID);
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Settings2.Instance.WoWInteractMouseover, IntPtr.Zero);
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Settings2.Instance.WoWInteractMouseover, IntPtr.Zero);
                }
                else
                {
                    Notify.TrayPopup("Attention!", "Please set up WoW internal keybinds in <Settings2 -> World of Warcraft -> Ingame key binds>", NotifyUserType.Warn, true);
                    Thread.Sleep(3000);
                }
            }
        }

        public virtual void Target()
        {
            wowProcess.WaitWhileWoWIsMinimized();
            info = info ?? new GameInterface(wowProcess);
            if (info.IsInGame)
            {
                if (Settings2.Instance.WoWTargetMouseover != Keys.None)
                {
                    SetMouseoverUnit(GUID);
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Settings2.Instance.WoWTargetMouseover, IntPtr.Zero);
                    NativeMethods.SendMessage(wowProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Settings2.Instance.WoWTargetMouseover, IntPtr.Zero);
                }
                else
                {
                    Notify.TrayPopup("Attention!", "Please set up WoW internal keybinds in <Settings2 -> World of Warcraft -> Ingame key binds>", NotifyUserType.Warn, true);
                    Thread.Sleep(3000);
                }
            }
        }
    }
}