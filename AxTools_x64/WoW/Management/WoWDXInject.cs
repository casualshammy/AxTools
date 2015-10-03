using AxTools.Helpers;
using AxTools.WinAPI;
using AxTools.WoW.Management.ObjectManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AxTools.WoW.Management
{
    [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
    [Obfuscation(Exclude = false, Feature = "constants")]
    internal static class WoWDXInject
    {
        private static WowProcess _wowProcess;
        private static readonly string RandomVariableName = Utils.GetRandomString(10);
        private static readonly string OverlayFrameName = Utils.GetRandomString(10);
        private const uint PAGE_EXECUTE_READWRITE = 64;
        private static readonly Dictionary<IntPtr, int> MemoryWaitingToBeFreed = new Dictionary<IntPtr, int>();
        private static readonly object MemoryWaitingToBeFreedLock = new object();
        private static readonly Timer TimerForMemory = new Timer(1000);
        private static readonly object _executeLock = new object();

        static WoWDXInject()
        {
            TimerForMemory.Elapsed += TimerForMemory_OnElapsed;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="process">Ref to <see cref="WowProcess"/> we want to inject</param>
        /// <returns>True if successful, false if hook point's signature is invalid</returns>
        internal static bool Apply(WowProcess process)
        {
            _wowProcess = process;
            TimerForMemory.Start();
            byte[] hookOriginalBytes = _wowProcess.Memory.ReadBytes(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, WowBuildInfoX64.HookLength);
            if (HookPtrSignatureIsValid(hookOriginalBytes))
            {
                Log.Info(string.Format("{0}:{1} :: [WoW hook] Signature is valid, address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render).ToInt64()));
                return true;
            }
            Log.Error(string.Format("{0}:{1} :: [WoW hook] Hook point has invalid signature, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, BitConverter.ToString(hookOriginalBytes)));
            return false;
        }
        
        internal static void Release()
        {
            TimerForMemory.Stop();
            if (!_wowProcess.Memory.Process.HasExited)
            {
                int counter = 10;
                while (MemoryWaitingToBeFreed.Count > 0 && counter > 0)
                {
                    FreeOldAllocatedMemory();
                    counter--;
                    Thread.Sleep(200);
                }
                if (MemoryWaitingToBeFreed.Count > 0)
                {
                    Log.Error(string.Format("{0}:{1} :: [WoW hook] Memory leak, count: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, MemoryWaitingToBeFreed.Count));
                }
            }
            lock (MemoryWaitingToBeFreedLock)
            {
                MemoryWaitingToBeFreed.Clear();
            }
        }

        private static void TimerForMemory_OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            FreeOldAllocatedMemory();
        }

        /// <summary>
        ///     You should call it STRICTLY after ExecuteWait. ExecuteWait has a lock.
        /// </summary>
        /// <param name="addresses"></param>
        private static void EnqueueAllocatedMemoryForWipe(params IntPtr[] addresses)
        {
            int currentTime = Environment.TickCount;
            lock (MemoryWaitingToBeFreedLock)
            {
                foreach (IntPtr intPtr in addresses)
                {
                    MemoryWaitingToBeFreed.Add(intPtr, currentTime);
                }
            }
        }

        private static void FreeOldAllocatedMemory()
        {
            lock (MemoryWaitingToBeFreedLock)
            {
                int currentTime = Environment.TickCount;
                List<IntPtr> entriesToRemove = new List<IntPtr>();
                foreach (KeyValuePair<IntPtr, int> keyValuePair in MemoryWaitingToBeFreed)
                {
                    if (currentTime - keyValuePair.Value >= 1000)
                    {
                        _wowProcess.Memory.FreeMemory(keyValuePair.Key);
                        entriesToRemove.Add(keyValuePair.Key);
                    }
                }
                foreach (IntPtr intPtr in entriesToRemove)
                {
                    MemoryWaitingToBeFreed.Remove(intPtr);
                }
            }
        }

        private static bool HookPtrSignatureIsValid(byte[] originalBytes)
        {
            return originalBytes.SequenceEqual(WowBuildInfoX64.HookPattern);
        }

        private static byte[] CreateByteCode(byte[] userFunction)
        {
            byte[] originalCode = _wowProcess.Memory.ReadBytes(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, WowBuildInfoX64.HookLength);
            byte[] firstInt = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render).ToInt64());
            byte[] secondInt = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render + 4).ToInt64());
            byte[] thirdInt = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render + 8).ToInt64());
            byte[] originalPtr = firstInt;
            byte[] ret = new byte[]
            {
                // Save the flags
                //0x9c, // pushfq                
                // Save the registers
                0x50, // push rax
                0x51, // push rcx
                0x52, // push rdx
                0x53, // push rbx
                0x55, // push rbp
                0x56, // push rsi
                0x57, // push rdi
                0x41, 0x50, // push r8
                0x41, 0x51, // push r9
                0x41, 0x52, // push r10
                0x41, 0x53, // push r11
                0x41, 0x54, // push r12
                0x41, 0x55, // push r13
                0x41, 0x56, // push r14
                0x41, 0x57 // push r15
            }.Concat(userFunction).Concat(new byte[]
            {
                // Restore the registers
                0x41, 0x5F, // pop r15
                0x41, 0x5E, // pop r14
                0x41, 0x5D, // pop r13
                0x41, 0x5C, // pop r12
                0x41, 0x5B, // pop r11
                0x41, 0x5A, // pop r10
                0x41, 0x59, // pop r9
                0x41, 0x58, // pop r8
                0x5F, // pop rdi
                0x5E, // pop rsi
                0x5D, // pop rbp
                0x5B, // pop rbx
                0x5A, // pop rdx
                0x59, // pop rcx
                0x58, // pop rax
                // Restore the flags
                //0x9D, // popfq
                0xB8, originalCode[0], originalCode[1], originalCode[2], originalCode[3], // mov eax, value // +4
                0x2E, 0xA3, firstInt[0], firstInt[1], firstInt[2], firstInt[3], firstInt[4], firstInt[5], firstInt[6], firstInt[7], // mov address, eax // +14
                0xB8, originalCode[4], originalCode[5], originalCode[6], originalCode[7], // mov eax, value // +19
                0x2E, 0xA3, secondInt[0], secondInt[1], secondInt[2], secondInt[3], secondInt[4], secondInt[5], secondInt[6], secondInt[7], // mov address, eax // +29
                0xB8, originalCode[8], originalCode[9], originalCode[10], originalCode[11], // mov eax, value // +34
                0x2E, 0xA3, thirdInt[0], thirdInt[1], thirdInt[2], thirdInt[3], thirdInt[4], thirdInt[5], thirdInt[6], thirdInt[7], // mov address, eax // +44
                0x90, // nop (for fun)
                0x90, // nop (for fun)
                0x48, 0xB8, originalPtr[0], originalPtr[1], originalPtr[2], originalPtr[3], originalPtr[4], originalPtr[5], originalPtr[6], originalPtr[7], // movabs rax, 0xAAAAAAAAAAAAAAAA
                0xFF, 0xE0 // jmp rax
            }).ToArray();
            return ret;
        }

        private static void ExecuteWait(IntPtr injectedFunction)
        {
            lock (_executeLock)
            {
                byte[] originalCode = _wowProcess.Memory.ReadBytes(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, WowBuildInfoX64.HookLength);
                if (HookPtrSignatureIsValid(originalCode))
                {
                    byte[] hookJmpBytes = BitConverter.GetBytes(injectedFunction.ToInt64());
                    byte[] byteCode =
                    {
                        0x48, 0xB8, hookJmpBytes[0], hookJmpBytes[1], hookJmpBytes[2], hookJmpBytes[3], hookJmpBytes[4], hookJmpBytes[5], hookJmpBytes[6], hookJmpBytes[7], // movabs rax, 0xAAAAAAAAAAAAAAAA
                        0xFF, 0xE0 // jmp rax
                    };
                    uint lpflOldProtect;
                    NativeMethods.VirtualProtectEx(_wowProcess.Memory.ProcessHandle, _wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, (UIntPtr) WowBuildInfoX64.HookLength, PAGE_EXECUTE_READWRITE, out lpflOldProtect);
                    _wowProcess.Memory.WriteBytes(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, byteCode);
                    int counter = 0;
                    while (!_wowProcess.Memory.ReadBytes(_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, WowBuildInfoX64.HookLength).SequenceEqual(originalCode))
                    {
                        Thread.Sleep(1);
                        counter++;
                        if (counter == 1000 && IsWoWWindowMinimized())
                        {
                            AppSpecUtils.NotifyUser("Attention!", "AxTools is stuck because it can't interact with minimized WoW client. Please activate WoW window!", NotifyUserType.Warn, true);
                        }
                    }
                    NativeMethods.VirtualProtectEx(_wowProcess.Memory.ProcessHandle, _wowProcess.Memory.ImageBase + WowBuildInfoX64.CGWorldFrame_Render, (UIntPtr) WowBuildInfoX64.HookLength, lpflOldProtect, out lpflOldProtect);
                }
                else
                {
                    Log.Info(string.Format("{0}:{1} :: [WoW hook] CGWorldFrame::Render has invalid signature, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, BitConverter.ToString(originalCode)));
                }
            }
        }

        private static bool IsWoWWindowMinimized()
        {
            int wsMinimize = 0x20000000;
            long style = NativeMethods.GetWindowLong64(_wowProcess.MainWindowHandle, NativeMethods.GWL_STYLE);
            return (style & wsMinimize) != 0;
        }

        internal static void LuaDoString(string command)
        {
            byte[] commandBytes = Encoding.UTF8.GetBytes(command);
            IntPtr cmdAddr = _wowProcess.Memory.AllocateMemory(commandBytes.Length + 1);
            _wowProcess.Memory.WriteBytes(cmdAddr, commandBytes);
            byte[] cmdAddrBytes = BitConverter.GetBytes(cmdAddr.ToInt64());
            byte[] luaFunctionPtr = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.FrameScript_ExecuteBuffer).ToInt64());
            byte[] byteCode = CreateByteCode(new byte[]
            {
                0x48, 0x83, 0xEC, 0x20, // sub rsp, 0x20
                0x48, 0xB9, cmdAddrBytes[0], cmdAddrBytes[1], cmdAddrBytes[2], cmdAddrBytes[3], cmdAddrBytes[4], cmdAddrBytes[5], cmdAddrBytes[6], cmdAddrBytes[7], // mov rcx, 8bytes
                0x48, 0xBA, cmdAddrBytes[0], cmdAddrBytes[1], cmdAddrBytes[2], cmdAddrBytes[3], cmdAddrBytes[4], cmdAddrBytes[5], cmdAddrBytes[6], cmdAddrBytes[7], // mov rdx, 8bytes
                0x49, 0xC7, 0xC0, 0x00, 0x00, 0x00, 0x00, // mov r8, 0x0
                0x48, 0xB8, luaFunctionPtr[0], luaFunctionPtr[1], luaFunctionPtr[2], luaFunctionPtr[3], luaFunctionPtr[4], luaFunctionPtr[5], luaFunctionPtr[6], luaFunctionPtr[7], // movabs rax, 8bytes
                0xFF, 0xD0, // call rax
                0x48, 0x83, 0xC4, 0x20 // add rsp, 0x20
            });
            IntPtr mem = _wowProcess.Memory.AllocateMemory(byteCode.Length);
            _wowProcess.Memory.WriteBytes(mem, byteCode);
            ExecuteWait(mem);
            EnqueueAllocatedMemoryForWipe(mem, cmdAddr);
        }

        internal static unsafe string GetFunctionReturn(string function)
        {
            IntPtr localPlayerPtr = _wowProcess.Memory.Read<IntPtr>(_wowProcess.Memory.ImageBase + WowBuildInfoX64.PlayerPtr);
            byte[] cmdRequest = Encoding.UTF8.GetBytes(RandomVariableName + "=" + function);
            byte[] cmdRetrieve = Encoding.UTF8.GetBytes(RandomVariableName);
            IntPtr addrRequest = _wowProcess.Memory.AllocateMemory(cmdRequest.Length + 1);
            IntPtr addrRetrieve = _wowProcess.Memory.AllocateMemory(cmdRetrieve.Length + 1);
            _wowProcess.Memory.WriteBytes(addrRequest, cmdRequest);
            _wowProcess.Memory.WriteBytes(addrRetrieve, cmdRetrieve);
            byte[] cmdRequestBytes = BitConverter.GetBytes(addrRequest.ToInt64());
            byte[] cmdRetrieveBytes = BitConverter.GetBytes(addrRetrieve.ToInt64());
            byte[] localPlayerPtrBytes = BitConverter.GetBytes(localPlayerPtr.ToInt64());
            byte[] executeBufferPtr = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.FrameScript_ExecuteBuffer).ToInt64());
            byte[] getLocalizedTextPtr = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.FrameScript_GetLocalizedText).ToInt64());
            IntPtr ptrToResult = _wowProcess.Memory.AllocateMemory(sizeof (IntPtr));
            byte[] ptrToResultBytes = BitConverter.GetBytes(ptrToResult.ToInt64());
            byte[] byteCode = CreateByteCode(new byte[]
            {
                0x48, 0x83, 0xEC, 0x20, // sub rsp, 20h
                0x48, 0xB9, cmdRequestBytes[0], cmdRequestBytes[1], cmdRequestBytes[2], cmdRequestBytes[3], cmdRequestBytes[4], cmdRequestBytes[5], cmdRequestBytes[6], cmdRequestBytes[7], // mov rcx, 8bytes
                0x48, 0xBA, cmdRequestBytes[0], cmdRequestBytes[1], cmdRequestBytes[2], cmdRequestBytes[3], cmdRequestBytes[4], cmdRequestBytes[5], cmdRequestBytes[6], cmdRequestBytes[7], // mov rdx, 8bytes
                0x49, 0xC7, 0xC0, 0x00, 0x00, 0x00, 0x00, // mov r8, 0x0
                0x48, 0xB8, executeBufferPtr[0], executeBufferPtr[1], executeBufferPtr[2], executeBufferPtr[3], executeBufferPtr[4], executeBufferPtr[5], executeBufferPtr[6], executeBufferPtr[7], // movabs rax, value
                0xFF, 0xD0, // call rax
                0x48, 0x83, 0xC4, 0x20, // add rsp, 20h
                0x48, 0x83, 0xEC, 0x20, // sub rsp, 20h
                0x48, 0xB9, localPlayerPtrBytes[0], localPlayerPtrBytes[1], localPlayerPtrBytes[2], localPlayerPtrBytes[3], localPlayerPtrBytes[4], localPlayerPtrBytes[5], localPlayerPtrBytes[6], localPlayerPtrBytes[7],
                // mov rcx, value
                0x48, 0xBA, cmdRetrieveBytes[0], cmdRetrieveBytes[1], cmdRetrieveBytes[2], cmdRetrieveBytes[3], cmdRetrieveBytes[4], cmdRetrieveBytes[5], cmdRetrieveBytes[6], cmdRetrieveBytes[7], // mov rdx, value
                0x49, 0xC7, 0xC0, 0xFF, 0xFF, 0xFF, 0xFF, // mov r8, -1
                0x48, 0xB8, getLocalizedTextPtr[0], getLocalizedTextPtr[1], getLocalizedTextPtr[2], getLocalizedTextPtr[3], getLocalizedTextPtr[4], getLocalizedTextPtr[5], getLocalizedTextPtr[6], getLocalizedTextPtr[7],
                // movabs rax, value
                0xFF, 0xD0, // call rax
                0x48, 0xA3, ptrToResultBytes[0], ptrToResultBytes[1], ptrToResultBytes[2], ptrToResultBytes[3], ptrToResultBytes[4], ptrToResultBytes[5], ptrToResultBytes[6], ptrToResultBytes[7], // movabs ds:0x130000000, rax
                0x48, 0x83, 0xC4, 0x20 // add rsp, 20h
            });
            IntPtr mem = _wowProcess.Memory.AllocateMemory(byteCode.Length);
            _wowProcess.Memory.WriteBytes(mem, byteCode);
            ExecuteWait(mem);
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
            EnqueueAllocatedMemoryForWipe(addrRequest, addrRetrieve, ptrToResult, mem);
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
            string colorRed = (color.R / 255f).ToString(culture);
            string colorGreen = (color.G / 255f).ToString(culture);
            string colorBlue = (color.B / 255f).ToString(culture);
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
            if (GetFunctionReturn("GetCVar(\"autointeract\")") == "1")
            {
                IntPtr playerPtr = _wowProcess.Memory.Read<IntPtr>(_wowProcess.Memory.ImageBase + WowBuildInfoX64.PlayerPtr);
                byte[] playerPtrBytes = BitConverter.GetBytes(playerPtr.ToInt64());
                IntPtr locationPtr = _wowProcess.Memory.AllocateMemory(sizeof(WowPoint));
                _wowProcess.Memory.Write(locationPtr, point);
                byte[] locationPtrBytes = BitConverter.GetBytes(locationPtr.ToInt64());
                byte[] functionPtr = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGUnit_C_InitializeTrackingState).ToInt64());
                byte[] byteCode = CreateByteCode(new byte[]
                {
                    0x48, 0x83, 0xEC, 0x20, // sub rsp, 20h
                    0x48, 0xB9, playerPtrBytes[0], playerPtrBytes[1], playerPtrBytes[2], playerPtrBytes[3], playerPtrBytes[4], playerPtrBytes[5], playerPtrBytes[6], playerPtrBytes[7], // mov rcx, value
                    0x48, 0xBA, locationPtrBytes[0], locationPtrBytes[1], locationPtrBytes[2], locationPtrBytes[3], locationPtrBytes[4], locationPtrBytes[5], locationPtrBytes[6], locationPtrBytes[7], // mov rdx, value
                    0x49, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // mov r8, 0
                    0x48, 0xB8, functionPtr[0], functionPtr[1], functionPtr[2], functionPtr[3], functionPtr[4], functionPtr[5], functionPtr[6], functionPtr[7], // movabs rax, value
                    0xFF, 0xD0, // call rax
                    0x48, 0x83, 0xC4, 0x20 // add rsp, 20h
                });
                IntPtr mem = _wowProcess.Memory.AllocateMemory(byteCode.Length);
                _wowProcess.Memory.WriteBytes(mem, byteCode);
                ExecuteWait(mem);
                EnqueueAllocatedMemoryForWipe(mem, locationPtr);
            }
            else
            {
                throw new InvalidOperationException("You should enable ClickToMove first");
            }
        }

        internal static unsafe void TargetUnit(UInt128 guid)
        {
            IntPtr guidAddress = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
            _wowProcess.Memory.Write(guidAddress, guid);
            byte[] guidAddressBytes = BitConverter.GetBytes(guidAddress.ToInt64());
            byte[] functionPtr = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGGameUI_Target).ToInt64());
            byte[] byteCode = CreateByteCode(new byte[]
            {
                0x48, 0x83, 0xEC, 0x20, // sub rsp, 20h
                0x48, 0xB9, guidAddressBytes[0], guidAddressBytes[1], guidAddressBytes[2], guidAddressBytes[3], guidAddressBytes[4], guidAddressBytes[5], guidAddressBytes[6], guidAddressBytes[7], // mov rcx, value
                0x48, 0xB8, functionPtr[0], functionPtr[1], functionPtr[2], functionPtr[3], functionPtr[4], functionPtr[5], functionPtr[6], functionPtr[7], // movabs rax, 0xAAAAAAAAAAAAAAAA
                0xFF, 0xD0, // call rax
                0x48, 0x83, 0xC4, 0x20 // add rsp, 20h
            });
            IntPtr mem = _wowProcess.Memory.AllocateMemory(byteCode.Length);
            _wowProcess.Memory.WriteBytes(mem, byteCode);
            ExecuteWait(mem);
            EnqueueAllocatedMemoryForWipe(mem, guidAddress);
        }

        internal static unsafe void Interact(UInt128 guid)
        {
            IntPtr guidAddress = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
            _wowProcess.Memory.Write(guidAddress, guid);
            byte[] guidAddressBytes = BitConverter.GetBytes(guidAddress.ToInt64());
            byte[] functionPtr = BitConverter.GetBytes((_wowProcess.Memory.ImageBase + WowBuildInfoX64.CGGameUI_Interact).ToInt64());
            byte[] byteCode = CreateByteCode(new byte[]
            {
                0x48, 0x83, 0xEC, 0x20, // sub rsp, 20h
                0x48, 0xB9, guidAddressBytes[0], guidAddressBytes[1], guidAddressBytes[2], guidAddressBytes[3], guidAddressBytes[4], guidAddressBytes[5], guidAddressBytes[6], guidAddressBytes[7], // mov rcx, value
                0x48, 0xB8, functionPtr[0], functionPtr[1], functionPtr[2], functionPtr[3], functionPtr[4], functionPtr[5], functionPtr[6], functionPtr[7], // movabs rax, 0xAAAAAAAAAAAAAAAA
                0xFF, 0xD0, // call rax
                0x48, 0x83, 0xC4, 0x20 // add rsp, 20h
            });
            IntPtr mem = _wowProcess.Memory.AllocateMemory(byteCode.Length);
            _wowProcess.Memory.WriteBytes(mem, byteCode);
            ExecuteWait(mem);
            EnqueueAllocatedMemoryForWipe(mem, guidAddress);
        }

    }
}
