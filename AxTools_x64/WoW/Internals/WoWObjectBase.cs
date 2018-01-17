using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AxTools.WoW.Internals
{
    public class WoWObjectBase
    {

        public virtual WoWGUID GUID { get; protected set; }
        public virtual string Name { get; protected set; }

        private void SetMouseoverUnit(WoWGUID guid)
        {
            WoWGUID mouseoverGUID = WoWManager.WoWProcess.Memory.Read<WoWGUID>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.MouseoverGUID);
            if (mouseoverGUID != guid)
            {
                WoWManager.WoWProcess.Memory.Write(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.MouseoverGUID, guid);
            }
        }

        public void Interact()
        {
            WoWManager.WoWProcess.WaitWhileWoWIsMinimized();
            if (Info.IsInGame)
            {
                if (Settings.Instance.WoWInteractMouseover != Keys.None)
                {
                    SetMouseoverUnit(GUID);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Settings.Instance.WoWInteractMouseover, IntPtr.Zero);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Settings.Instance.WoWInteractMouseover, IntPtr.Zero);
                }
                else
                {
                    Notify.TrayPopup("Attention!", "Please set up WoW internal keybinds in <Settings -> World of Warcraft -> Ingame key binds>", NotifyUserType.Warn, true);
                    Thread.Sleep(3000);
                }
            }
        }

        public virtual void Target()
        {
            WoWManager.WoWProcess.WaitWhileWoWIsMinimized();
            if (Info.IsInGame)
            {
                if (Settings.Instance.WoWTargetMouseover != Keys.None)
                {
                    SetMouseoverUnit(GUID);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYDOWN, (IntPtr)Settings.Instance.WoWTargetMouseover, IntPtr.Zero);
                    NativeMethods.SendMessage(WoWManager.WoWProcess.MainWindowHandle, Win32Consts.WM_KEYUP, (IntPtr)Settings.Instance.WoWTargetMouseover, IntPtr.Zero);
                }
                else
                {
                    Notify.TrayPopup("Attention!", "Please set up WoW internal keybinds in <Settings -> World of Warcraft -> Ingame key binds>", NotifyUserType.Warn, true);
                    Thread.Sleep(3000);
                }
            }
        }

    }
}
