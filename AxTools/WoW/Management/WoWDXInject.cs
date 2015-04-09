using AxTools.Classes;
using AxTools.WoW.Management.ObjectManager;
using Fasm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;

namespace AxTools.WoW.Management
{
    internal static class WoWDXInject
    {
        private static WowProcess _wowProcess;
        private static ManagedFasm _fasm;
        private static byte[] _hookOriginalBytes;
        private static IntPtr _loopCodePtr;
        private static IntPtr _customFunctionPtr;
        private static IntPtr _returnValuePtr;
        private static IntPtr _hookPtr;
        private static int _loopCodeSize;
        private const int CustomFunctionSize = 0x256;
        private static uint _referenceProtectionType;
        private static readonly byte[] Eraser = new byte[CustomFunctionSize];
        private static IntPtr _codeCavePtr;
        private static readonly object FASMLock = new object();
        private static readonly string RandomVariableName = Utils.GetRandomString(10);
        private static readonly string OverlayFrameName = Utils.GetRandomString(10);
        private static readonly string[] RegisterNames = { "ah", "al", "bh", "bl", "ch", "cl", "dh", "dl", "eax", "ebx", "ecx", "edx" };
        private const uint PAGE_EXECUTE_READWRITE = 64;
        
        internal static bool Apply(WowProcess process)
        {
            _wowProcess = process;
            _fasm = _wowProcess.Memory.Asm;
            _fasm.SetMemorySize(0x4096);
            _hookPtr = _wowProcess.Memory.ImageBase + WowBuildInfo.CGWorldFrameRender + 3; // 3 because it's length of <<push ebp>, <mov ebp, esp>> before <mov> instruction
            _hookOriginalBytes = _wowProcess.Memory.ReadBytes(_hookPtr, 5);
            if (_hookOriginalBytes[0] == 0xA1)
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Original bytes: {2}, address: 0x{3:X}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                    BitConverter.ToString(_hookOriginalBytes), _hookPtr.ToInt32()), false, false);
                // allocate memory the new injection code pointer:
                _customFunctionPtr = _wowProcess.Memory.AllocateMemory(0x4);
                _wowProcess.Memory.Write(_customFunctionPtr, 0);
                Log.Print(string.Format("{0}:{1} :: [WoW hook] User code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint) _customFunctionPtr));
                // allocate memory the pointer return value:
                _returnValuePtr = _wowProcess.Memory.AllocateMemory(0x4);
                _wowProcess.Memory.Write(_returnValuePtr, 0);
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Return value code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint) _returnValuePtr));
                // injecting loop
                List<string> asm = new List<string>();
                RandomizeOpcode(asm, "pushad");
                RandomizeOpcode(asm, "pushfd");
                RandomizeOpcode(asm, "mov eax, [" + _customFunctionPtr + "]");
                RandomizeOpcode(asm, "test eax, eax");
                RandomizeOpcode(asm, "je @out");
                RandomizeOpcode(asm, "mov eax, [" + _customFunctionPtr + "]");
                RandomizeOpcode(asm, "call eax");
                RandomizeOpcode(asm, "mov [" + _returnValuePtr + "], eax");
                RandomizeOpcode(asm, "mov edx, " + _customFunctionPtr);
                RandomizeOpcode(asm, "mov ecx, 0");
                RandomizeOpcode(asm, "mov [edx], ecx");
                RandomizeOpcode(asm, "@out:");
                RandomizeOpcode(asm, "popfd");
                RandomizeOpcode(asm, "popad");
                _fasm.Clear();
                foreach (string str in asm)
                {
                    _fasm.AddLine(str);
                }
                int sizeAsm = _fasm.Assemble().Length;
                _loopCodeSize = sizeAsm + 5 + 5;
                _loopCodePtr = GetPointerForLoopCode(); // 5 for original instruction, 5 for return jmp
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Loop code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint) _loopCodePtr));
                _fasm.Inject((uint) _loopCodePtr);
                // injecting trampouline
                _wowProcess.Memory.WriteBytes(_loopCodePtr + sizeAsm, _hookOriginalBytes);
                // injecting return to Endscene/Present
                _fasm.Clear();
                _fasm.AddLine("jmp " + ((uint)_hookPtr + 5)); // 5 is <jmp> instruction length
                _fasm.Inject((uint) (_loopCodePtr + sizeAsm + _hookOriginalBytes.Length));
                // apply hook
                _fasm.Clear();
                _fasm.AddLine("jmp " + _loopCodePtr);
                _fasm.Inject((uint) _hookPtr);
                // Allocating codecave
                _codeCavePtr = GetPointerForCustomFunction();
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Custom function address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint) _codeCavePtr));
                // Report about success :)
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Successfully hooked, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, BitConverter.ToString(_wowProcess.Memory.ReadBytes(_hookPtr, 5))));
                return true;
            }
            Log.Print(string.Format("{0}:{1} :: [WoW hook] CGWorldFrame__Render has invalid signature, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, BitConverter.ToString(_hookOriginalBytes)), false, false);
            return false;
        }
        
        internal static void Release()
        {
            if (!_wowProcess.Memory.Process.HasExited)
            {
                try
                {
                    _wowProcess.Memory.WriteBytes(_hookPtr, _hookOriginalBytes);
                    Log.Print(string.Format("{0}:{1} :: [WoW hook] Hook is deleted, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, BitConverter.ToString(_wowProcess.Memory.ReadBytes(_hookPtr, 5))));
                }
                catch (Exception ex)
                {
                    Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't delete hook, WoW client is closed? ({2})", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message));
                }
                try
                {
                    _fasm.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't dispose FASM assembler ({2})", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message));
                }
                try
                {
                    _wowProcess.Memory.FreeMemory(_customFunctionPtr);
                    _wowProcess.Memory.FreeMemory(_returnValuePtr);
                }
                catch (Exception ex)
                {
                    Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't free memory with hook ({2})", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message));
                }
                try
                {
                    _wowProcess.Memory.WriteBytes(_loopCodePtr, new byte[_loopCodeSize]);
                    _wowProcess.Memory.WriteBytes(_codeCavePtr, Eraser);
                    uint temp;
                    WinAPI.NativeMethods.VirtualProtectEx(_wowProcess.Memory.ProcessHandle, _loopCodePtr, (UIntPtr) _loopCodeSize, _referenceProtectionType, out temp);
                    WinAPI.NativeMethods.VirtualProtectEx(_wowProcess.Memory.ProcessHandle, _codeCavePtr, (UIntPtr)CustomFunctionSize, _referenceProtectionType, out temp);
                }
                catch (Exception ex)
                {
                    Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't free memory with hook (2) ({2})", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message), true);
                }
            }
            else
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't delete hook: WoW client has been finished", _wowProcess.ProcessName, _wowProcess.ProcessID));
            }
        }

        private static IntPtr GetPointerForLoopCode()
        {
            byte[] emptyBytes = new byte[_loopCodeSize];
            int start = (int)(_wowProcess.Memory.ImageBase + _wowProcess.Memory.Process.MainModule.ModuleMemorySize - 100 - emptyBytes.Length);
            int end = (int)_wowProcess.Memory.ImageBase;
            int pageSize = Environment.SystemPageSize;
            for (int i = start; i > end; i--)
            {
                if (i % pageSize == 0)
                {
                    byte[] temp = _wowProcess.Memory.ReadBytes((IntPtr)i, emptyBytes.Length);
                    if (temp.SequenceEqual(emptyBytes))
                    {
                        if (WinAPI.NativeMethods.VirtualProtectEx(_wowProcess.Memory.ProcessHandle, (IntPtr) i, (UIntPtr) _loopCodeSize, PAGE_EXECUTE_READWRITE, out _referenceProtectionType))
                        {
                            return (IntPtr)i;
                        }
                        Log.Print(string.Format("{0}:{1} :: [WoW hook] GetPointerForLoopCode: Can't change memory protection type: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, i));
                        throw new Exception("GetPointerForLoopCode: Can't change memory protection type!");
                    }
                }
            }
            throw new Exception("GetPointerForLoopCode: can't find memory!");
        }

        private static IntPtr GetPointerForCustomFunction()
        {
            byte[] emptyBytes = new byte[CustomFunctionSize];
            int start = (int) (_wowProcess.Memory.ImageBase + _wowProcess.Memory.Process.MainModule.ModuleMemorySize - 100 - emptyBytes.Length - _loopCodeSize);
            int end = (int) _wowProcess.Memory.ImageBase;
            int pageSize = Environment.SystemPageSize;
            for (int i = start; i > end; i--)
            {
                if (i%pageSize == 0)
                {
                    byte[] temp = _wowProcess.Memory.ReadBytes((IntPtr)i, emptyBytes.Length);
                    if (temp.SequenceEqual(emptyBytes))
                    {
                        if (WinAPI.NativeMethods.VirtualProtectEx(_wowProcess.Memory.ProcessHandle, (IntPtr) i, (UIntPtr) CustomFunctionSize, PAGE_EXECUTE_READWRITE, out _referenceProtectionType))
                        {
                            return (IntPtr) i;
                        }
                        Log.Print(string.Format("{0}:{1} :: [WoW hook] GetPointerForCustomFunction: Can't change memory protection type: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, i));
                        throw new Exception("GetPointerForCustomFunction: Can't change memory protection type!");
                    }
                }
            }
            throw new Exception("GetPointerForCustomFunction: can't find memory!");
        }

        internal static void LuaDoString(string command)
        {
            // cdecl
            // todo
            return;
            lock (FASMLock)
            {
                byte[] bytesCommand = Encoding.UTF8.GetBytes(command);
                IntPtr address = _wowProcess.Memory.AllocateMemory(bytesCommand.Length + 1);
                _wowProcess.Memory.WriteBytes(address, bytesCommand);
                string[] asm = {
                    "mov eax, " + address,
                    "push 0",
                    "push eax",
                    "push eax",
                    "mov eax, " + (_wowProcess.Memory.ImageBase + WowBuildInfo.LuaDoStringAddress),
                    "call eax",
                    "add esp, 0xC",
                    "retn"
                };
                try
                {
                    InjectVoid(asm);
                }
                catch
                {
                    // ReSharper disable once RedundantJumpStatement
                    return;
                }
                finally
                {
                    _wowProcess.Memory.FreeMemory(address);
                }
            }
        }

        internal static string GetLocalizedText(string commandLine)
        {
            // thiscall
            // todo
            return string.Empty;
            lock (FASMLock)
            {
                byte[] command = Encoding.UTF8.GetBytes(commandLine);
                IntPtr address = _wowProcess.Memory.AllocateMemory(command.Length + 1);
                _wowProcess.Memory.WriteBytes(address, command);
                string[] asm = {
                    "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClntObjMgrGetActivePlayerObj),
                    "mov ecx, eax",
                    "push -1",
                    "mov edx, " + address,
                    "push edx",
                    "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.LuaGetLocalizedTextAddress),
                    "retn"
                };
                try
                {
                    return InjectReturn(asm);
                }
                catch
                {
                    return null;
                }
                finally
                {
                    _wowProcess.Memory.FreeMemory(address);
                }
            }
        }

        internal static string GetFunctionReturn(string function)
        {
            // cdecl + thiscall
            //todo
            return string.Empty;
            lock (FASMLock)
            {
                byte[] commandRequest = Encoding.UTF8.GetBytes(RandomVariableName + "=" + function);
                byte[] commandRetrieve = Encoding.UTF8.GetBytes(RandomVariableName);
                IntPtr addressRequest = _wowProcess.Memory.AllocateMemory(commandRequest.Length + 1);
                IntPtr addressRetrieve = _wowProcess.Memory.AllocateMemory(commandRetrieve.Length + 1);
                _wowProcess.Memory.WriteBytes(addressRequest, commandRequest);
                _wowProcess.Memory.WriteBytes(addressRetrieve, commandRetrieve);
                string[] asm = {
                    "mov eax, " + addressRequest,
                    "push 0",
                    "push eax",
                    "push eax",
                    "mov eax, " + (_wowProcess.Memory.ImageBase + WowBuildInfo.LuaDoStringAddress),
                    "call eax",
                    "add esp, 0xC",
                    "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClntObjMgrGetActivePlayerObj),
                    "mov ecx, eax",
                    "push -1",
                    "mov edx, " + addressRetrieve,
                    "push edx",
                    "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.LuaGetLocalizedTextAddress),
                    "retn"
                };
                try
                {
                    return InjectReturn(asm);
                }
                catch
                {
                    return null;
                }
                finally
                {
                    _wowProcess.Memory.FreeMemory(addressRetrieve);
                    _wowProcess.Memory.FreeMemory(addressRequest);
                }
            }
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

        internal static unsafe void TerrainClick(TerrainClickStruct clickStruct)
        {
            // cdecl
            IntPtr terrainClickStructPtr = _wowProcess.Memory.AllocateMemory(sizeof (TerrainClickStruct));
            _wowProcess.Memory.Write(terrainClickStructPtr, clickStruct);
            string[] asm =
            {
                "mov eax, " + terrainClickStructPtr,
                "push eax",
                "mov eax, " + (_wowProcess.Memory.ImageBase + WowBuildInfo.HandleTerrainClick),
                "call eax",
                "add esp, 0x4",
                "retn"
            };
            try
            {
                InjectVoid(asm);
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] TerrainClick error: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message), true);
            }
            finally
            {
                _wowProcess.Memory.FreeMemory(terrainClickStructPtr);
            }
        }

        internal static unsafe void MoveTo(WowPoint point)
        {
            // thiscall
            IntPtr localPlayerPtr = _wowProcess.Memory.Read<IntPtr>((IntPtr) WowBuildInfo.PlayerPtr, true);
            IntPtr locationPtr = _wowProcess.Memory.AllocateMemory(0x4 * 3);
            IntPtr guidPtr = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
            _wowProcess.Memory.Write(locationPtr, point);
            string[] asm =
            {
                "mov ecx, " + localPlayerPtr,
                "mov eax, " + guidPtr,
                "mov edx, " + locationPtr,
                "push 0",
                "push edx",
                "push eax",
                "push 4",
                "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClickToMove),
                "retn"
            };
            try
            {
                InjectVoid(asm);
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] MoveTo error: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message), true);
            }
            finally
            {
                _wowProcess.Memory.FreeMemory(locationPtr);
                _wowProcess.Memory.FreeMemory(guidPtr);
            }
        }

        internal static unsafe void TargetUnit(UInt128 guid)
        {
            // cdecl
            IntPtr guidPtr = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
            _wowProcess.Memory.Write(guidPtr, guid);
            string[] asm =
            {
                "push " + guidPtr,
                "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.TargetUnit),
                "add esp, 0x4",
                "retn"
            };
            try
            {
                InjectVoid(asm);
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] TargetUnit error: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message), true);
            }
            finally
            {
                _wowProcess.Memory.FreeMemory(guidPtr);
            }
        }

        internal static unsafe void Interact(UInt128 guid)
        {
            IntPtr guidPtr = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
            _wowProcess.Memory.Write(guidPtr, guid);
            string[] asm =
            {
                "push " + guidPtr,
                "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.Interact),
                "add esp, 0x4",
                "retn"
            };
            try
            {
                InjectVoid(asm);
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Interact error: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message), true);
            }
            finally
            {
                _wowProcess.Memory.FreeMemory(guidPtr);
            }
        }

        private static void InjectVoid(IEnumerable<string> asm)
        {
            lock (FASMLock)
            {
                _fasm.Clear();
                foreach (string str in asm)
                {
                    _fasm.AddLine(str);
                }
                _fasm.Inject((uint)_codeCavePtr);
                _wowProcess.Memory.Write(_customFunctionPtr, (int)_codeCavePtr);
                while (_wowProcess.Memory.Read<uint>(_customFunctionPtr) > 0)
                {
                    Thread.Sleep(1);
                }
                _wowProcess.Memory.WriteBytes(_codeCavePtr, Eraser);
            }
        }

        private static string InjectReturn(IEnumerable<string> asm)
        {
            lock (FASMLock)
            {
                _wowProcess.Memory.Write(_returnValuePtr, 0);
                _fasm.Clear();
                foreach (string str in asm)
                {
                    _fasm.AddLine(str);
                }
                _fasm.Inject((uint)_codeCavePtr);
                _wowProcess.Memory.Write(_customFunctionPtr, (int)_codeCavePtr);
                while (_wowProcess.Memory.Read<uint>(_customFunctionPtr) > 0)
                {
                    Thread.Sleep(1);
                }
                List<byte> retnByte = new List<byte>();
                uint dwAddress = _wowProcess.Memory.Read<uint>(_returnValuePtr);
                if (dwAddress != 0)
                {
                    byte buf = _wowProcess.Memory.Read<byte>((IntPtr)dwAddress);
                    while (buf != 0)
                    {
                        retnByte.Add(buf);
                        dwAddress = dwAddress + 1;
                        buf = _wowProcess.Memory.Read<byte>((IntPtr)dwAddress);
                    }
                }
                _wowProcess.Memory.WriteBytes(_codeCavePtr, Eraser);
                return Encoding.UTF8.GetString(retnByte.ToArray());
            }
        }

        private static void RandomizeOpcode(ICollection<string> list, string asm)
        {
            if (Utils.Rnd.Next(2) == 0)
            {
                list.Add("nop");
            }
            else
            {
                int ranNum = Utils.Rnd.Next(0, RegisterNames.Length);
                list.Add(string.Concat("mov ", RegisterNames[ranNum], ", ", RegisterNames[ranNum]));
            }
            list.Add(asm);
        }

    }
}
