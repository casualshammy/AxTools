using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.Management.ObjectManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using AxTools.WoW.PluginSystem.API;
using MyMemory;
using MyMemory.Hooks;

namespace AxTools.WoW.Management
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    [Obfuscation(Exclude = false, Feature = "constants")]
    internal static class WoWDXInject
    {
        private static WowProcess _wowProcess;
        private static RemoteProcess _remoteProcess;
        private static HookJmp _hookJmp;
        private static readonly string RandomVariableName = Utils.GetRandomString(10);
        private static readonly string OverlayFrameName = Utils.GetRandomString(10);
        private static readonly object HookLock = new object();

        /// <summary>
        ///     
        /// </summary>
        /// <param name="process">Ref to <see cref="WowProcess"/> we want to inject</param>
        /// <returns>True if successful, false if hook point's signature is invalid</returns>
        internal static bool Apply(WowProcess process)
        {
            _wowProcess = process;
            byte[] hookOriginalBytes = _wowProcess.Memory.ReadBytes(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, WowBuildInfoX64.HookPattern.Length);
            if (hookOriginalBytes.SequenceEqual(WowBuildInfoX64.HookPattern))
            {
                lock (HookLock)
                {
                    _remoteProcess = new RemoteProcess((uint) _wowProcess.ProcessID);
                    _hookJmp = _remoteProcess.HooksManager.CreateJmpHook(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, WowBuildInfoX64.HookPattern.Length);
                }
                Log.Info(string.Format("{0} [WoW hook] Signature is valid, address: 0x{1:X}", _wowProcess, (_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render).ToInt64()));
                return true;
            }
            Log.Error(string.Format("{0} [WoW hook] Hook point has invalid signature, bytes: {1}", _wowProcess, BitConverter.ToString(hookOriginalBytes)));
            return false;
        }

        internal static void Release()
        {
            if (!_wowProcess.Memory.Process.HasExited)
            {
                lock (HookLock)
                {
                    _remoteProcess.Dispose();
                }
            }
        }

        private static bool IsWoWWindowMinimized()
        {
            long style = NativeMethods.GetWindowLong64(_wowProcess.MainWindowHandle, Win32Consts.GWL_STYLE);
            return (style & Win32Consts.WS_MINIMIZE) != 0;
        }

        internal static void LuaDoString(string command)
        {
            byte[] commandBytes = Encoding.UTF8.GetBytes(command);
            IntPtr cmdAddr = _remoteProcess.MemoryManager.AllocateRawMemory((uint) (commandBytes.Length + 1));
            _remoteProcess.MemoryManager.WriteBytes(cmdAddr, commandBytes);
            string[] code =
            {
                "sub rsp, 0x20",
                "mov rcx, 0x" + cmdAddr.ToInt64().ToString("X"),
                "mov rdx, 0x" + cmdAddr.ToInt64().ToString("X"),
                "mov r8, 0x0",
                "mov rax, 0x" + (_wowProcess.Memory.ImageBase + WowBuildInfoX64.FrameScript_ExecuteBuffer).ToInt64().ToString("X"),
                "call rax",
                "add rsp, 0x20",
                "retn"
            };
            HookApplyExecuteRemove(code);
            _remoteProcess.MemoryManager.FreeRawMemory(cmdAddr);
        }

        /// <summary>
        ///     Returns string.Empty if something went wrong
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        internal static unsafe string GetFunctionReturn(string function)
        {
            IntPtr localPlayerPtr = _remoteProcess.MemoryManager.Read<IntPtr>(_wowProcess.Memory.ImageBase + WowBuildInfoX64.PlayerPtr);
            if (localPlayerPtr == IntPtr.Zero)
            {
                Log.Error("GetFunctionReturn was called while player ptr is null");
                return "";
            }
            byte[] cmdRequest = Encoding.UTF8.GetBytes(RandomVariableName + "=" + function);
            byte[] cmdRetrieve = Encoding.UTF8.GetBytes(RandomVariableName);
            IntPtr addrRequest = _remoteProcess.MemoryManager.AllocateRawMemory((uint) (cmdRequest.Length + 1));
            IntPtr addrRetrieve = _remoteProcess.MemoryManager.AllocateRawMemory((uint) (cmdRetrieve.Length + 1));
            IntPtr ptrToResult = _remoteProcess.MemoryManager.AllocateRawMemory((uint) sizeof (IntPtr));
            _wowProcess.Memory.WriteBytes(addrRequest, cmdRequest);
            _wowProcess.Memory.WriteBytes(addrRetrieve, cmdRetrieve);
            string[] code =
            {
                "sub rsp, 0x20",
                "mov rcx, 0x" + addrRequest.ToInt64().ToString("X"),
                "mov rdx, 0x" + addrRequest.ToInt64().ToString("X"),
                "mov r8, 0x0",
                "mov rax, 0x" + (_wowProcess.Memory.ImageBase + WowBuildInfoX64.FrameScript_ExecuteBuffer).ToInt64().ToString("X"),
                "call rax",
                "mov rcx, 0x" + localPlayerPtr.ToInt64().ToString("X"),
                "mov rdx, 0x" + addrRetrieve.ToInt64().ToString("X"),
                "mov r8, 0xFFFFFFFF",
                "mov rax, 0x" + (_wowProcess.Memory.ImageBase + WowBuildInfoX64.FrameScript_GetLocalizedText).ToInt64().ToString("X"),
                "call rax",
                "mov rcx, 0x" + ptrToResult.ToInt64().ToString("X"),
                "mov [rcx], rax",
                "add rsp, 0x20",
                "retn"
            };
            HookApplyExecuteRemove(code);
            List<byte> retnByte = new List<byte>();
            IntPtr dwAddress = _wowProcess.Memory.Read<IntPtr>(ptrToResult);
            if (dwAddress != IntPtr.Zero)
            {
                byte buf = _wowProcess.Memory.Read<byte>(dwAddress);
                while (buf != 0)
                {
                    retnByte.Add(buf);
                    dwAddress = dwAddress + 1;
                    buf = _wowProcess.Memory.Read<byte>(dwAddress);
                }
            }
            _remoteProcess.MemoryManager.FreeRawMemory(addrRequest);
            _remoteProcess.MemoryManager.FreeRawMemory(addrRetrieve);
            _remoteProcess.MemoryManager.FreeRawMemory(ptrToResult);
            return Encoding.UTF8.GetString(retnByte.ToArray());
        }

        internal static void ShowOverlayText(string text, string icon, Color color, bool flash = false)
        {
            #region Ingame overlay initialization string

            string initializeIngameOverlay = ("if (AxToolsMainOverlayChildren == nil) then\r\n" +
                                              "   AxToolsMainOverlay = CreateFrame(\"Frame\", \"AxToolsMainOverlay\", UIParent);\r\n" +
                                              "   AxToolsMainOverlayChildren = CreateFrame(\"Frame\", \"AxToolsMainOverlayChildren\", AxToolsMainOverlay)\r\n" +
                                              "   AxToolsMainOverlayChildren:SetWidth(1)\r\nAxToolsMainOverlayChildren:SetHeight(1)\r\n" +
                                              "   AxToolsMainOverlayChildren:SetPoint(\"CENTER\", UIParent, \"CENTER\", 0, 100)\r\n" +
                                              "   AxToolsMainOverlayChildren:SetFrameStrata(\"TOOLTIP\")\r\n" +
                                              "   AxToolsMainOverlayChildren.text = AxToolsMainOverlayChildren:CreateFontString(nil, \"ARTWORK\")\r\n" +
                                              "   AxToolsMainOverlayChildren.text:SetFont(\"Fonts\\\\FRIZQT__.TTF\", 40, \"THICKOUTLINE\")\r\n" +
                                              "   AxToolsMainOverlayChildren.text:SetPoint(\"CENTER\", AxToolsMainOverlayChildren, \"CENTER\", 25, 0)\r\n" +
                                              "   AxToolsMainOverlayChildren.icon = AxToolsMainOverlayChildren:CreateTexture(nil, \"ARTWORK\")\r\n" +
                                              "   AxToolsMainOverlayChildren.icon:SetTexCoord(0.07, 0.93, 0.07, 0.93)\r\n" +
                                              "   AxToolsMainOverlayChildren.icon:SetPoint(\"RIGHT\", AxToolsMainOverlayChildren.text, \"LEFT\", -10, 0)\r\n" +
                                              "   AxToolsMainOverlayChildren.icon:SetWidth(40)\r\n" +
                                              "   AxToolsMainOverlayChildren.icon:SetHeight(40)\r\n" +
                                              "   AxToolsMainOverlayChildren:Hide()\r\n" +
                                              "   AxToolsMainOverlayChildren.elapsed = 0\r\n" +
                                              "   AxToolsMainOverlayChildren:SetScript(\"OnUpdate\", function(self, elapsed)\r\n" +
                                              "       self.elapsed = self.elapsed + elapsed\r\n" +
                                              "       if self.elapsed <= 0.1 then\r\n" +
                                              "           self:SetAlpha(1-((0.1-self.elapsed)/0.1))\r\n" +
                                              "       elseif self.elapsed <= 1.8 then\r\n" +
                                              "           self:SetAlpha(1)\r\n" +
                                              "       elseif self.elapsed <= 1.9 then\r\n" +
                                              "           self:SetAlpha((1.9 - self.elapsed)/0.1)\r\n" +
                                              "       else\r\n" +
                                              "           self:SetAlpha(0)\r\n" +
                                              "           self:Hide()\r\n" +
                                              "       end\r\n" +
                                              "   end)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash = CreateFrame(\"Frame\", \"AxToolsMainOverlayChildrenFlash\", UIParent)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash:SetAllPoints()\r\n" +
                                              "   AxToolsMainOverlayChildren.flash:Hide()\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.tex = AxToolsMainOverlayChildren.flash:CreateTexture(nil, \"BACKGROUND\")\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.tex:SetTexture(\"Interface\\\\FullScreenTextures\\\\LowHealth\")\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.tex:SetBlendMode(\"ADD\")\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.tex:SetAllPoints(AxToolsMainOverlayChildren.flash)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.texture = AxToolsMainOverlayChildren.flash.tex\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.AnimationGroup = AxToolsMainOverlayChildrenFlash:CreateAnimationGroup()\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.faderOut = AxToolsMainOverlayChildren.flash.AnimationGroup:CreateAnimation(\"Alpha\")\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.faderOut:SetChange(-1)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.faderOut:SetDuration(0.3)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.faderOut:SetOrder(1)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.faderIn = AxToolsMainOverlayChildren.flash.AnimationGroup:CreateAnimation(\"Alpha\")\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.faderIn:SetChange(1)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.faderIn:SetDuration(0.3)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.faderIn:SetOrder(2)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.AnimationGroup:SetScript(\"OnFinished\", function(self) self:Play() end)\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.AnimationGroup:Play()\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.flashing = false\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.enable = false\r\n" +
                                              "   AxToolsMainOverlayChildren.flash.elapsed = 0\r\n" +
                                              "   AxToolsMainOverlayChildren.flash:SetScript(\"OnUpdate\", function(self, elapsed)\r\n" +
                                              "       self.elapsed = self.elapsed + elapsed\r\n" +
                                              "       if (self.elapsed >= 2) then\r\n" +
                                              "           self:Hide()\r\n" +
                                              "       end\r\n" +
                                              "   end)\r\n" +
                                              "end").Replace("AxToolsMainOverlay", OverlayFrameName);

            #endregion

            CultureInfo culture = CultureInfo.InvariantCulture;
            string colorRed = (color.R/255f).ToString(culture);
            string colorGreen = (color.G/255f).ToString(culture);
            string colorBlue = (color.B/255f).ToString(culture);
            string function = OverlayFrameName + "Children.text:SetText(\"" + text + "\");\r\n" +
                              OverlayFrameName + "Children.text:SetVertexColor(" + colorRed + ", " + colorGreen + ", " + colorBlue + ", 1);\r\n" +
                              OverlayFrameName + "Children.icon:SetTexture(\"" + icon + "\");\r\n" +
                              OverlayFrameName + "Children.elapsed = 0;\r\n" +
                              OverlayFrameName + "Children:Show();";
            if (flash)
            {
                function += "\r\n" +
                            OverlayFrameName + "Children.flash.elapsed = 0;\r\n" +
                            OverlayFrameName + "Children.flash:Show();";
            }
            LuaDoString(initializeIngameOverlay + "\r\n" + function);
        }

        internal static unsafe void MoveTo(WowPoint point)
        {
            if (GameFunctions.Lua_IsCTMEnabled())
            {
                IntPtr playerPtr = _remoteProcess.MemoryManager.Read<IntPtr>(_wowProcess.Memory.ImageBase + WowBuildInfoX64.PlayerPtr);
                IntPtr locationPtr = _remoteProcess.MemoryManager.AllocateRawMemory((uint) sizeof (WowPoint));
                _remoteProcess.MemoryManager.Write(locationPtr, point);
                string[] code =
                {
                    "sub rsp, 0x20",
                    "mov rcx, 0x" + playerPtr.ToInt64().ToString("X"),
                    "mov rdx, 0x" + locationPtr.ToInt64().ToString("X"),
                    "mov r8, 0x0",
                    "mov rax, 0x" + (_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGUnit_C_InitializeTrackingState).ToInt64().ToString("X"),
                    "call rax",
                    "add rsp, 0x20",
                    "retn"
                };
                HookApplyExecuteRemove(code);
                _remoteProcess.MemoryManager.FreeRawMemory(locationPtr);
            }
            else
            {
                throw new InvalidOperationException("You should enable ClickToMove first");
            }
        }

        internal static unsafe void TargetUnit(UInt128 guid)
        {
            IntPtr guidAddress = _remoteProcess.MemoryManager.AllocateRawMemory((uint) sizeof (UInt128));
            _remoteProcess.MemoryManager.Write(guidAddress, guid);
            string[] code =
            {
                "sub rsp, 0x20",
                "mov rcx, 0x" + guidAddress.ToInt64().ToString("X"),
                "mov rax, 0x" + (_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGGameUI_Target).ToInt64().ToString("X"),
                "call rax",
                "add rsp, 0x20",
                "retn"
            };
            HookApplyExecuteRemove(code);
            _remoteProcess.MemoryManager.FreeRawMemory(guidAddress);
        }

        internal static unsafe void Interact(UInt128 guid)
        {
            IntPtr guidAddress = _remoteProcess.MemoryManager.AllocateRawMemory((uint) sizeof (UInt128));
            _remoteProcess.MemoryManager.Write(guidAddress, guid);
            string[] code =
            {
                "sub rsp, 0x20",
                "mov rcx, 0x" + guidAddress.ToInt64().ToString("X"),
                "mov rax, 0x" + (_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGGameUI_Interact).ToInt64().ToString("X"),
                "call rax",
                "add rsp, 0x20",
                "retn"
            };
            HookApplyExecuteRemove(code);
            _remoteProcess.MemoryManager.FreeRawMemory(guidAddress);
        }

        private static void HookApplyExecuteRemove(string[] asm)
        {
            lock (HookLock)
            {
                if (IsWoWWindowMinimized())
                {
                    AppSpecUtils.NotifyUser("Attention!", "AxTools is stuck because it can't interact with minimized WoW client. Please activate WoW window!", NotifyUserType.Warn, true);
                }
                _hookJmp.Apply();
                _hookJmp.Executor.Execute<long>(asm, true);
                _hookJmp.Remove();
            }
        }

    }
}
