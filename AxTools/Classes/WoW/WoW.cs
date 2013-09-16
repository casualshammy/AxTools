﻿using AxTools.Classes.WoW.DX;
using Fasm;
using GreyMagic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;

namespace AxTools.Classes.WoW
{
    internal static class WoW
    {
        internal static WowProcess WProc;
        internal static bool Hooked { private set; get; }

        internal static HookResult Hook(WowProcess process)
        {
            WProc = process;
            if (WProc.Memory == null)
                WProc.Memory = new ExternalProcessReader(Process.GetProcessById(WProc.ProcessID));

            #region Apply DirectX hook

            _dxAddress = new Dirext3D(Process.GetProcessById(WProc.ProcessID));
            _fasm = WProc.Memory.Asm;
            _fasm.SetMemorySize(0x4096);
            _codeCavePtr = WProc.Memory.AllocateMemory(CodeCaveSize);
            uint offset;
            Log.Print(string.Format("{0}:{1} :: [WoW hook] DX version: {2}", WProc.ProcessName, WProc.ProcessID, _dxAddress.UsingDirectX11 ? 11 : 9),
                      false, false);
            if (_dxAddress.UsingDirectX11)
            {
                offset = 0xB;
                _endSceneOriginalBytes = WProc.Memory.ReadBytes(_dxAddress.HookPtr + (int)offset, 5);
                if (_endSceneOriginalBytes[0] != 0xA1)
                {
                    Log.Print(string.Format("{0}:{1} :: [WoW hook] Incorrect DX version, bytes: {2}", WProc.ProcessName, WProc.ProcessID,
                              BitConverter.ToString(_endSceneOriginalBytes)), false, false);
                    return HookResult.IncorrectDirectXVersion;
                }
            }
            else
            {
                offset = 0x5;
                _endSceneOriginalBytes = WProc.Memory.ReadBytes(_dxAddress.HookPtr + (int)offset, 7);
            }
            Log.Print(
                string.Format("{0}:{1} :: [WoW hook] Original bytes: {2}, address: 0x{3:X}", WProc.ProcessName, WProc.ProcessID,
                              BitConverter.ToString(_endSceneOriginalBytes), (uint)_dxAddress.HookPtr + offset), false, false);
            // allocate memory to store injected code:
            _injectedCode = WProc.Memory.AllocateMemory(2048);
            Log.Print(
                string.Format("{0}:{1} :: [WoW hook] Loop code address: 0x{2:X}", WProc.ProcessName, WProc.ProcessID, (uint)_injectedCode),
                false);
            // allocate memory the new injection code pointer:
            _addresseInjection = WProc.Memory.AllocateMemory(0x4);
            WProc.Memory.Write(_addresseInjection, 0);
            Log.Print(
                string.Format("{0}:{1} :: [WoW hook] User code address: 0x{2:X}", WProc.ProcessName, WProc.ProcessID, (uint)_addresseInjection),
                false);
            // allocate memory the pointer return value:
            _retnInjectionAsm = WProc.Memory.AllocateMemory(0x4);
            WProc.Memory.Write(_retnInjectionAsm, 0);
            Log.Print(
                string.Format("{0}:{1} :: [WoW hook] Return value code address: 0x{2:X}", WProc.ProcessName, WProc.ProcessID, (uint)_retnInjectionAsm), false);
            // injecting loop
            List<string> asm = new List<string>();
            RandomizeOpcode(asm, "pushad");
            RandomizeOpcode(asm, "pushfd");
            RandomizeOpcode(asm, "mov eax, [" + _addresseInjection + "]");
            RandomizeOpcode(asm, "test eax, eax");
            RandomizeOpcode(asm, "je @out");
            RandomizeOpcode(asm, "mov eax, [" + _addresseInjection + "]");
            RandomizeOpcode(asm, "call eax");
            RandomizeOpcode(asm, "mov [" + _retnInjectionAsm + "], eax");
            RandomizeOpcode(asm, "mov edx, " + _addresseInjection);
            RandomizeOpcode(asm, "mov ecx, 0");
            RandomizeOpcode(asm, "mov [edx], ecx");
            RandomizeOpcode(asm, "@out:");
            RandomizeOpcode(asm, "popfd");
            RandomizeOpcode(asm, "popad");
            int sizeAsm = InjectAsm(asm, (uint)_injectedCode);
            // injecting trampouline
            WProc.Memory.WriteBytes(_injectedCode + sizeAsm, _endSceneOriginalBytes);
            // injecting return to Endscene/Present
            asm.Clear();
            asm.Add("jmp " + ((uint)_dxAddress.HookPtr + offset + (_dxAddress.UsingDirectX11 ? 5 : 7)));
            InjectAsm(asm, (uint)(_injectedCode + sizeAsm + _endSceneOriginalBytes.Length));
            //
            asm.Clear();
            asm.Add("jmp " + (_injectedCode));
            InjectAsm(asm, (uint)_dxAddress.HookPtr + offset);
            Log.Print(
                string.Format("{0}:{1} :: [WoW hook] Successfully hooked, bytes: {2}", WProc.ProcessName, WProc.ProcessID,
                              BitConverter.ToString(WProc.Memory.ReadBytes(_dxAddress.HookPtr + (int)offset, _dxAddress.UsingDirectX11 ? 5 : 7))), false);

            #endregion

            Hooked = true;
            return HookResult.Successful;
        }
        internal static void Unhook()
        {
            DeleteDXHook();
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Total objects cached: {2}", WProc.ProcessName, WProc.ProcessID, WowObject.Names.Count), false, false);
            WowObject.Names.Clear();
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Total players cached: {2}", WProc.ProcessName, WProc.ProcessID, WowPlayer.Names.Count), false, false);
            WowPlayer.Names.Clear();
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Total NPC cached: {2}", WProc.ProcessName, WProc.ProcessID, WowNpc.Names.Count), false);
            WowNpc.Names.Clear();
            Hooked = false;
        }

        #region DXInject

        private static byte[] _endSceneOriginalBytes;
        private static readonly int CodeCaveSize = 0x256;
        private static readonly byte[] Eraser = new byte[CodeCaveSize];
        private static IntPtr _injectedCode;
        private static IntPtr _addresseInjection;
        private static IntPtr _retnInjectionAsm;
        private static IntPtr _codeCavePtr;
        
        private static readonly object FASMLock = new object();
        private static readonly string RandomVariableName = Utils.GetRandomString(10);
        private static readonly string[] RegisterNames = {
            "ah", "al", "bh", "bl", "ch", "cl", "dh", "dl", "eax",
            "ebx", "ecx", "edx"
        };
        #region Ingame overlay initialization string
        private static readonly string InitializeIngameOverlay = "if (AxToolsMainOverlayChildren == nil) then\r\n" +
                                                                 "   AxToolsMainOverlay = CreateFrame(\"Frame\", \"AxToolsMainOverlay\", UIParent);\r\n" +
                                                                 "   AxToolsMainOverlayChildren = CreateFrame(\"Frame\", \"AxToolsMainOverlayChildren\", AxToolsMainOverlay)\r\n" +
                                                                 "   AxToolsMainOverlayChildren:SetWidth(1)\r\nAxToolsMainOverlayChildren:SetHeight(1)\r\n" +
                                                                 "   AxToolsMainOverlayChildren:SetPoint(\"CENTER\", UIParent, \"CENTER\", 0, 100)\r\n" +
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
                                                                 "end";
        #endregion
        private static ManagedFasm _fasm;
        private static Dirext3D _dxAddress;

        private static void DeleteDXHook()
        {
            try
            {
                uint offset = (uint) (_dxAddress.UsingDirectX11 ? 0xB : 0x5);
                WProc.Memory.WriteBytes(_dxAddress.HookPtr + (int) offset, _endSceneOriginalBytes);
                Log.Print(
                    string.Format("{0}:{1} :: [WoW hook] Hook is deleted, bytes: {2}", WProc.ProcessName, WProc.ProcessID,
                                  BitConverter.ToString(WProc.Memory.ReadBytes(_dxAddress.HookPtr + (int) offset, _dxAddress.UsingDirectX11 ? 5 : 7))), false);
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't delete hook, WoW client is closed? ({2})", WProc.ProcessName, WProc.ProcessID,
                                        ex.Message), false);
            }
            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                _fasm.Dispose();
            }
            catch{}
            // ReSharper restore EmptyGeneralCatchClause
            try
            {
                _dxAddress.Device.Dispose();
                _dxAddress = null;
            }
            finally
            {
                WProc.Memory.FreeMemory(_injectedCode);
                WProc.Memory.FreeMemory(_addresseInjection);
                WProc.Memory.FreeMemory(_retnInjectionAsm);
                WProc.Memory.FreeMemory(_codeCavePtr);
            }
        }
        private static int InjectAsm(IEnumerable<string> asm, uint dwAddress)
        {
            int size;
            _fasm.Clear();
            foreach (string str in asm)
            {
                _fasm.AddLine(str);
            }
            try
            {
                size = _fasm.Assemble().Length;
                _fasm.Inject(dwAddress);
            }
            catch
            {
                size = 0;
            }
            return size;
        }
        private static void RandomizeOpcode(List<string> list, string asm)
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

        internal static bool LuaDoString(string command)
        {
            lock (FASMLock)
            {
                byte[] bytesCommand = Encoding.UTF8.GetBytes(command);
                IntPtr address = WProc.Memory.AllocateMemory(bytesCommand.Length + 1);
                WProc.Memory.WriteBytes(address, bytesCommand);
                string[] asm = {
                    "mov eax, " + address,
                    "push 0",
                    "push eax",
                    "push eax",
                    "mov eax, " + (WProc.Memory.ImageBase + WowBuildInfo.LuaDoStringAddress),
                    "call eax",
                    "add esp, 0xC",
                    "retn"
                };
                try
                {
                    InjectAsm(asm, (uint) _codeCavePtr);
                    WProc.Memory.Write(_addresseInjection, (int)_codeCavePtr);
                    while (WProc.Memory.Read<uint>(_addresseInjection) > 0)
                    {
                        Thread.Sleep(1);
                    }
                    WProc.Memory.WriteBytes(_codeCavePtr, Eraser);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    WProc.Memory.FreeMemory(address);
                }
            }
        }
        internal static string GetLocalizedText(string commandLine)
        {
            lock (FASMLock)
            {
                byte[] command = Encoding.UTF8.GetBytes(commandLine);
                IntPtr address = WProc.Memory.AllocateMemory(command.Length + 1);
                WProc.Memory.WriteBytes(address, command);
                string[] asm = {
                    "call " + (WProc.Memory.ImageBase + WowBuildInfo.ClntObjMgrGetActivePlayerObjAddress),
                    "mov ecx, eax",
                    "push -1",
                    "mov edx, " + address,
                    "push edx",
                    "call " + (WProc.Memory.ImageBase + WowBuildInfo.LuaGetLocalizedTextAddress),
                    "retn"
                };
                try
                {
                    WProc.Memory.Write(_retnInjectionAsm, 0);
                    InjectAsm(asm, (uint)_codeCavePtr);
                    WProc.Memory.Write(_addresseInjection, (int)_codeCavePtr);
                    while (WProc.Memory.Read<uint>(_addresseInjection) > 0)
                    {
                        Thread.Sleep(1);
                    }
                    List<byte> retnByte = new List<byte>();
                    uint dwAddress = WProc.Memory.Read<uint>(_retnInjectionAsm);
                    if (dwAddress != 0)
                    {
                        byte buf = WProc.Memory.Read<byte>((IntPtr)dwAddress);
                        while (buf != 0)
                        {
                            retnByte.Add(buf);
                            dwAddress = dwAddress + 1;
                            buf = WProc.Memory.Read<byte>((IntPtr)dwAddress);
                        }
                    }
                    WProc.Memory.WriteBytes(_codeCavePtr, Eraser);
                    return Encoding.UTF8.GetString(retnByte.ToArray());
                }
                catch
                {
                    return string.Empty;
                }
                finally
                {
                    WProc.Memory.FreeMemory(address);
                }
            }
        }
        internal static string GetFunctionReturn(string function)
        {
            lock (FASMLock)
            {
                byte[] commandRequest = Encoding.UTF8.GetBytes(RandomVariableName + "=" + function);
                byte[] commandRetrieve = Encoding.UTF8.GetBytes(RandomVariableName);
                IntPtr addressRequest = WProc.Memory.AllocateMemory(commandRequest.Length + 1);
                IntPtr addressRetrieve = WProc.Memory.AllocateMemory(commandRetrieve.Length + 1);
                WProc.Memory.WriteBytes(addressRequest, commandRequest);
                WProc.Memory.WriteBytes(addressRetrieve, commandRetrieve);
                string[] asm = {
                    "mov eax, " + addressRequest,
                    "push 0",
                    "push eax",
                    "push eax",
                    "mov eax, " + (WProc.Memory.ImageBase + WowBuildInfo.LuaDoStringAddress),
                    "call eax",
                    "add esp, 0xC",
                    "call " + (WProc.Memory.ImageBase + WowBuildInfo.ClntObjMgrGetActivePlayerObjAddress),
                    "mov ecx, eax",
                    "push -1",
                    "mov edx, " + addressRetrieve,
                    "push edx",
                    "call " + (WProc.Memory.ImageBase + WowBuildInfo.LuaGetLocalizedTextAddress),
                    "retn"
                };
                try
                {
                    WProc.Memory.Write(_retnInjectionAsm, 0);
                    InjectAsm(asm, (uint)_codeCavePtr);
                    WProc.Memory.Write(_addresseInjection, (int)_codeCavePtr);
                    while (WProc.Memory.Read<uint>(_addresseInjection) > 0)
                    {
                        Thread.Sleep(1);
                    }
                    List<byte> retnByte = new List<byte>();
                    uint dwAddress = WProc.Memory.Read<uint>(_retnInjectionAsm);
                    if (dwAddress != 0)
                    {
                        byte buf = WProc.Memory.Read<byte>((IntPtr)dwAddress);
                        while (buf != 0)
                        {
                            retnByte.Add(buf);
                            dwAddress = dwAddress + 1;
                            buf = WProc.Memory.Read<byte>((IntPtr)dwAddress);
                        }
                    }
                    WProc.Memory.WriteBytes(_codeCavePtr, Eraser);
                    return Encoding.UTF8.GetString(retnByte.ToArray());
                }
                catch
                {
                    return string.Empty;
                }
                finally
                {
                    WProc.Memory.FreeMemory(addressRetrieve);
                    WProc.Memory.FreeMemory(addressRequest);
                }
            }
        }
        internal static bool ShowOverlayText(string text, string icon, Color color, bool flash = false)
        {
            string function = "AxToolsMainOverlayChildren.text:SetText(\"" + text + "\");\r\n" +
                              "AxToolsMainOverlayChildren.text:SetVertexColor(" + (color.R/255f).ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                              (color.G/255f).ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " + (color.B/255f).ToString(System.Globalization.CultureInfo.InvariantCulture) + ", 1);\r\n" +
                              "AxToolsMainOverlayChildren.icon:SetTexture(\"" + icon + "\");\r\n" +
                              "AxToolsMainOverlayChildren.elapsed = 0;\r\n" +
                              "AxToolsMainOverlayChildren:Show();";
            if (flash)
            {
                function += "\r\n" +
                            "AxToolsMainOverlayChildren.flash.elapsed = 0;\r\n" +
                            "AxToolsMainOverlayChildren.flash:Show();";
            }
            LuaDoString(InitializeIngameOverlay + "\r\n" + function);
            return true;
        }
        //internal static bool InteractGameObject(IntPtr gameObjectAddress)
        //{
        //    string[] asm = new[] {
        //        "mov ecx, " + gameObjectAddress,
        //        "call " + (WProc.Memory.ImageBase + WowBuildInfo.GameObjectOnRightClick),
        //        "retn"
        //    };
        //    lock (LockObject)
        //    {
        //        try
        //        {
        //            InjectAsm(asm, (uint) _codeCavePtr);
        //            WProc.Memory.Write(_addresseInjection, (int)_codeCavePtr);
        //            while (WProc.Memory.Read<uint>(_addresseInjection) > 0)
        //            {
        //                Thread.Sleep(1);
        //            }
        //            WProc.Memory.WriteBytes(_codeCavePtr, Eraser);
        //            return true;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //internal static bool MoveTo(float x, float y, float z)
        //{
        //    IntPtr ctmPoint = WProc.Memory.AllocateMemory(0x4 * 3);
        //    IntPtr ctmGUID = WProc.Memory.AllocateMemory(0x4 * 2);
        //    WProc.Memory.Write(ctmPoint, x);
        //    WProc.Memory.Write(ctmPoint + 0x4, y);
        //    WProc.Memory.Write(ctmPoint + 0x8, z);
        //    string[] asm = new[] {
        //            "call " + (WProc.Memory.ImageBase + WowBuildInfo.ClntObjMgrGetActivePlayerObjAddress),
        //            "mov ecx, eax",
        //            "mov eax, " + ctmGUID,
        //            "mov edx, " + ctmPoint,
        //            "push 0",
        //            "push edx",
        //            "push eax",
        //            "push 4",
        //            "call " + (WProc.Memory.ImageBase + WowBuildInfo.ClickToMove),
        //            "retn"
        //        };
        //    try
        //    {
        //        InjectAsm(asm, (uint)_codeCavePtr);
        //        WProc.Memory.Write(_addresseInjection, (int)_codeCavePtr);
        //        while (WProc.Memory.Read<uint>(_addresseInjection) > 0)
        //        {
        //            Thread.Sleep(1);
        //        }
        //        WProc.Memory.WriteBytes(_codeCavePtr, Eraser);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        WProc.Memory.FreeMemory(ctmPoint);
        //        WProc.Memory.FreeMemory(ctmGUID);
        //    }
        //}
        internal static bool TargetUnit(ulong guid)
        {
            lock (FASMLock)
            {
                uint dwHiWord = (uint) (guid >> 32);
                uint dwLoWord = (uint) guid;
                string[] asm = {
                    "mov eax, " + dwHiWord,
                    "push eax",
                    "mov eax, " + dwLoWord,
                    "push eax",
                    "mov eax, " + (WProc.Memory.ImageBase + WowBuildInfo.SelectTarget),
                    "call eax",
                    "add esp, 0x8",
                    "retn"
                };
                try
                {
                    InjectAsm(asm, (uint) _codeCavePtr);
                    WProc.Memory.Write(_addresseInjection, (int) _codeCavePtr);
                    while (WProc.Memory.Read<uint>(_addresseInjection) > 0)
                    {
                        Thread.Sleep(1);
                    }
                    WProc.Memory.WriteBytes(_codeCavePtr, Eraser);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        internal static bool Interact(ulong guid)
        {
            lock (FASMLock)
            {
                uint dwHiWord = (uint) (guid >> 32);
                uint dwLoWord = (uint) guid;
                string[] asm = {
                    "mov eax, " + dwHiWord,
                    "push eax",
                    "mov eax, " + dwLoWord,
                    "push eax",
                    "mov eax, " + (WProc.Memory.ImageBase + WowBuildInfo.Interact),
                    "call eax",
                    "add esp, 0x8",
                    "retn"
                };
                try
                {
                    InjectAsm(asm, (uint) _codeCavePtr);
                    WProc.Memory.Write(_addresseInjection, (int) _codeCavePtr);
                    while (WProc.Memory.Read<uint>(_addresseInjection) > 0)
                    {
                        Thread.Sleep(1);
                    }
                    WProc.Memory.WriteBytes(_codeCavePtr, Eraser);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        
        #endregion

        internal static class LocalPlayer
        {
            internal static ulong GUID;
            internal static WowPointF Location;
            internal static IntPtr Address;
            internal static uint Health;
            internal static uint HealthMax;
            internal static uint CastingSpellID;
            internal static uint ChannelSpellID;
            internal static float Rotation;
            internal static uint Level;
            internal static ulong TargetGUID;
            internal static bool IsAlliance;
        }
        internal static void Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            lock (PulseLock)
            {
                wowObjects.Clear();
                wowUnits.Clear();
                wowNpcs.Clear();
                IntPtr manager = WProc.Memory.Read<IntPtr>(WProc.Memory.ImageBase + WowBuildInfo.ObjectManager);
                ulong playerGUID = WProc.Memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
                IntPtr currObject = WProc.Memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
                for (int i = WProc.Memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                    (i < 10) && (i > 0);
                    i = WProc.Memory.Read<int>(currObject + WowBuildInfo.ObjectType))
                {
                    switch (i)
                    {
                        case 3:
                            wowNpcs.Add(new WowNpc(currObject));
                            break;
                        case 4:
                            ulong objectGUID = WProc.Memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID);
                            if (objectGUID == playerGUID)
                            {
                                LocalPlayer.Address = currObject;
                                LocalPlayer.GUID = playerGUID;
                                LocalPlayer.Location = new WowPointF(WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationX),
                                                                     WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationY),
                                                                     WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationZ));
                                IntPtr desc = WProc.Memory.Read<IntPtr>(LocalPlayer.Address + WowBuildInfo.UnitDescriptors);
                                LocalPlayer.Health = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitHealth);
                                LocalPlayer.HealthMax = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitHealthMax);
                                LocalPlayer.CastingSpellID = WProc.Memory.Read<uint>(LocalPlayer.Address + WowBuildInfo.UnitCastingID);
                                LocalPlayer.ChannelSpellID = WProc.Memory.Read<uint>(LocalPlayer.Address + WowBuildInfo.UnitChannelingID);
                                LocalPlayer.Rotation = WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitRotation);
                                LocalPlayer.Level = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitLevel);
                                LocalPlayer.TargetGUID = WProc.Memory.Read<ulong>(desc + 0x50);
                                uint factionTemplate = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitFactionTemplate);
                                LocalPlayer.IsAlliance = factionTemplate == 0x89b || factionTemplate == 0x65d || factionTemplate == 0x73 ||
                                                         factionTemplate == 4 || factionTemplate == 3 || factionTemplate == 1 || factionTemplate == 2401;
                            }
                            else
                            {
                                wowUnits.Add(new WowPlayer(currObject));
                            }
                            break;
                        case 5:
                            wowObjects.Add(new WowObject(currObject));
                            break;
                    }
                    currObject = WProc.Memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
                }
            }
        }
        internal static void Pulse(List<WowObject> wowObjects, List<WowNpc> wowNpcs)
        {
            lock (PulseLock)
            {
                wowObjects.Clear();
                wowNpcs.Clear();
                IntPtr manager = WProc.Memory.Read<IntPtr>(WProc.Memory.ImageBase + WowBuildInfo.ObjectManager);
                ulong playerGUID = WProc.Memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
                IntPtr currObject = WProc.Memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
                for (int i = WProc.Memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                    (i < 10) && (i > 0);
                    i = WProc.Memory.Read<int>(currObject + WowBuildInfo.ObjectType))
                {
                    switch (i)
                    {
                        case 3:
                            wowNpcs.Add(new WowNpc(currObject));
                            break;
                        case 4:
                            if (WProc.Memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID) == playerGUID)
                            {
                                LocalPlayer.Address = currObject;
                                LocalPlayer.GUID = playerGUID;
                                LocalPlayer.Location = new WowPointF(WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationX),
                                                                     WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationY),
                                                                     WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationZ));
                                IntPtr desc = WProc.Memory.Read<IntPtr>(LocalPlayer.Address + WowBuildInfo.UnitDescriptors);
                                LocalPlayer.Health = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitHealth);
                                LocalPlayer.HealthMax = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitHealthMax);
                                LocalPlayer.CastingSpellID = WProc.Memory.Read<uint>(LocalPlayer.Address + WowBuildInfo.UnitCastingID);
                                LocalPlayer.ChannelSpellID = WProc.Memory.Read<uint>(LocalPlayer.Address + WowBuildInfo.UnitChannelingID);
                                LocalPlayer.Rotation = WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitRotation);
                                LocalPlayer.Level = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitLevel);
                                LocalPlayer.TargetGUID = WProc.Memory.Read<ulong>(desc + 0x50);
                                uint mFactionTemplate = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitFactionTemplate);
                                LocalPlayer.IsAlliance = mFactionTemplate == 0x89b || mFactionTemplate == 0x65d || mFactionTemplate == 0x73 ||
                                                         mFactionTemplate == 4 || mFactionTemplate == 3 || mFactionTemplate == 1 || mFactionTemplate == 2401;
                            }
                            break;
                        case 5:
                            wowObjects.Add(new WowObject(currObject));
                            break;
                    }
                    currObject = WProc.Memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
                }
            }
        }
        internal static void Pulse(List<WowObject> wowObjects)
        {
            lock (PulseLock)
            {
                wowObjects.Clear();
                IntPtr manager = WProc.Memory.Read<IntPtr>(WProc.Memory.ImageBase + WowBuildInfo.ObjectManager);
                ulong playerGUID = WProc.Memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
                bool localPlayerFound = false;
                IntPtr currObject = WProc.Memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
                for (int i = WProc.Memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                    (i < 10) && (i > 0);
                    i = WProc.Memory.Read<int>(currObject + WowBuildInfo.ObjectType))
                {
                    switch (i)
                    {
                        case 4:
                            if (!localPlayerFound)
                            {
                                var objectGUID = WProc.Memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID);
                                if (objectGUID == playerGUID)
                                {
                                    LocalPlayer.Address = currObject;
                                    LocalPlayer.GUID = playerGUID;
                                    LocalPlayer.Location = new WowPointF(WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationX),
                                                                         WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationY),
                                                                         WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationZ));
                                    IntPtr desc = WProc.Memory.Read<IntPtr>(currObject + WowBuildInfo.UnitDescriptors);
                                    LocalPlayer.Health = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitHealth);
                                    LocalPlayer.HealthMax = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitHealthMax);
                                    LocalPlayer.CastingSpellID = WProc.Memory.Read<uint>(LocalPlayer.Address + WowBuildInfo.UnitCastingID);
                                    LocalPlayer.ChannelSpellID = WProc.Memory.Read<uint>(LocalPlayer.Address + WowBuildInfo.UnitChannelingID);
                                    LocalPlayer.Rotation = WProc.Memory.Read<float>(currObject + WowBuildInfo.UnitRotation);
                                    LocalPlayer.Level = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitLevel);
                                    LocalPlayer.TargetGUID = WProc.Memory.Read<ulong>(desc + 0x50);
                                    uint mFactionTemplate = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitFactionTemplate);
                                    LocalPlayer.IsAlliance = mFactionTemplate == 0x89b || mFactionTemplate == 0x65d || mFactionTemplate == 0x73 ||
                                                             mFactionTemplate == 4 || mFactionTemplate == 3 || mFactionTemplate == 1 || mFactionTemplate == 2401;
                                    localPlayerFound = true;
                                }
                            }
                            break;
                        case 5:
                            wowObjects.Add(new WowObject(currObject));
                            break;
                    }
                    currObject = WProc.Memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
                }
            }
        }
        internal static void Pulse()
        {
            lock (PulseLock)
            {
                IntPtr manager = WProc.Memory.Read<IntPtr>(WProc.Memory.ImageBase + WowBuildInfo.ObjectManager);
                ulong playerGUID = WProc.Memory.Read<ulong>(manager + WowBuildInfo.LocalGUID);
                IntPtr currObject = WProc.Memory.Read<IntPtr>(manager + WowBuildInfo.ObjectManagerFirstObject);
                for (int i = WProc.Memory.Read<int>(currObject + WowBuildInfo.ObjectType);
                    (i < 10) && (i > 0);
                    i = WProc.Memory.Read<int>(currObject + WowBuildInfo.ObjectType))
                {
                    if (i == 4 && WProc.Memory.Read<ulong>(currObject + WowBuildInfo.ObjectGUID) == playerGUID)
                    {
                        LocalPlayer.Address = currObject;
                        LocalPlayer.GUID = playerGUID;
                        LocalPlayer.Location = new WowPointF(WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationX),
                                                             WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationY),
                                                             WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitLocationZ));
                        IntPtr desc = WProc.Memory.Read<IntPtr>(LocalPlayer.Address + WowBuildInfo.UnitDescriptors);
                        LocalPlayer.Health = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitHealth);
                        LocalPlayer.HealthMax = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitHealthMax);
                        LocalPlayer.CastingSpellID = WProc.Memory.Read<uint>(LocalPlayer.Address + WowBuildInfo.UnitCastingID);
                        LocalPlayer.ChannelSpellID = WProc.Memory.Read<uint>(LocalPlayer.Address + WowBuildInfo.UnitChannelingID);
                        LocalPlayer.Rotation = WProc.Memory.Read<float>(LocalPlayer.Address + WowBuildInfo.UnitRotation);
                        LocalPlayer.Level = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitLevel);
                        LocalPlayer.TargetGUID = WProc.Memory.Read<ulong>(desc + 0x50);
                        uint mFactionTemplate = WProc.Memory.Read<uint>(desc + WowBuildInfo.UnitFactionTemplate);
                        LocalPlayer.IsAlliance = mFactionTemplate == 0x89b || mFactionTemplate == 0x65d || mFactionTemplate == 0x73 ||
                                                 mFactionTemplate == 4 || mFactionTemplate == 3 || mFactionTemplate == 1 || mFactionTemplate == 2401;
                        return;
                    }
                    currObject = WProc.Memory.Read<IntPtr>(currObject + WowBuildInfo.ObjectManagerNextObject);
                }
            }
        }

        private static readonly object PulseLock = new object();
    }
}