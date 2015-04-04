using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.Classes;
using AxTools.Forms;
using AxTools.WoW.Management.ObjectManager;
using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Assembly;
using Binarysharp.MemoryManagement.Native;
using Fasm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using WindowsFormsAero.TaskDialog;
using GreyMagic;
using GreyMagic.Native;
using SafeMemoryHandle = GreyMagic.SafeMemoryHandle;

namespace AxTools.WoW.Management
{
    internal static class WoWDXInject
    {
        private static WowProcess _wowProcess;
        private static byte[] _hookOriginalBytes;
        private static readonly int CodeCaveSize = 0x256;
        private static readonly byte[] Eraser = new byte[CodeCaveSize];
        private static IntPtr _injectedCode;
        private static IntPtr _addresseInjection;
        private static IntPtr _retnInjectionAsm;
        private static IntPtr _codeCavePtr;

        private static readonly object FASMLock = new object();
        private static readonly string RandomVariableName = Utils.GetRandomString(10);
        private static readonly string OverlayFrameName = Utils.GetRandomString(10);
        private static readonly string[] RegisterNames = { "ah", "al", "bh", "bl", "ch", "cl", "dh", "dl", "eax", "ebx", "ecx", "edx" };
        //private static readonly int DX11PresentAddress = 0x21BD1;
        //private static readonly int DX9EndsceneAddress = 0x2279F;
        
        private static ManagedFasm _fasm;
        private static IntPtr _hookPtr;

        internal static bool Apply(WowProcess process)
        {
            _wowProcess = process;
            _fasm = _wowProcess.Memory.Asm;
            _fasm.SetMemorySize(0x4096);
            _codeCavePtr = _wowProcess.Memory.AllocateMemory(CodeCaveSize);
            //uint offset;
            //bool isDX11 = _wowProcess.Memory.Process.Modules.Cast<ProcessModule>().Any(m => m.ModuleName == "d3d11.dll");
            //Log.Print(string.Format("{0}:{1} :: [WoW hook] DX version: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, isDX11 ? 11 : 9), false, false);
            _hookPtr = _wowProcess.Memory.ImageBase + WowBuildInfo.CGWorldFrameRender + 3; // 3 because it's start of <mov> instruction after <push ebp>, <mov ebp, esp>
            //if (isDX11)
            //{
            //    foreach (ProcessModule module in _wowProcess.Memory.Process.Modules.Cast<ProcessModule>().Where(module => module.ModuleName == "dxgi.dll"))
            //    {
            //        _hookPtr = module.BaseAddress + DX11PresentAddress;
            //    }
            //    offset = 0xB;
            //}
            //else
            //{
            //    foreach (ProcessModule module in _wowProcess.Memory.Process.Modules.Cast<ProcessModule>().Where(module => module.ModuleName == "d3d9.dll"))
            //    {
            //        _hookPtr = module.BaseAddress + DX9EndsceneAddress;
            //    }
            //    offset = 0x19;
            //}
            //if (_hookPtr == IntPtr.Zero)
            //{
            //    Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't find DX library!", _wowProcess.ProcessName, _wowProcess.ProcessID), true);
            //    return false;
            //}
            _hookOriginalBytes = _wowProcess.Memory.ReadBytes(_hookPtr, 5);
            if (_hookOriginalBytes[0] == 0xA1)
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Original bytes: {2}, address: 0x{3:X}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                    BitConverter.ToString(_hookOriginalBytes), _hookPtr.ToInt32()), false, false);
                // allocate memory to store injected code:
                _injectedCode = _wowProcess.Memory.AllocateMemory(2048);
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Loop code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint) _injectedCode));
                // allocate memory the new injection code pointer:
                _addresseInjection = _wowProcess.Memory.AllocateMemory(0x4);
                _wowProcess.Memory.Write(_addresseInjection, 0);
                Log.Print(string.Format("{0}:{1} :: [WoW hook] User code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint) _addresseInjection));
                // allocate memory the pointer return value:
                _retnInjectionAsm = _wowProcess.Memory.AllocateMemory(0x4);
                _wowProcess.Memory.Write(_retnInjectionAsm, 0);
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Return value code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint) _retnInjectionAsm));
                // injecting loop
                List<string> asm = new List<string>();
                RandomizeOpcode(asm, "pushad");
                RandomizeOpcode(asm, "pushfd");
                RandomizeOpcode(asm, "@start:");
                RandomizeOpcode(asm, "mov eax, [" + _addresseInjection + "]");
                RandomizeOpcode(asm, "test eax, eax");
                RandomizeOpcode(asm, "je @out");
                //RandomizeOpcode(asm, "mov ecx, 1");
                //RandomizeOpcode(asm, "cmp ecx, [" + _addresseInjection + "]");
                //RandomizeOpcode(asm, "jne @clear");         // переход если (ax)<>len
                //RandomizeOpcode(asm, "jmp @SwitchToTwo");   // переход если (ax)=len
                //RandomizeOpcode(asm, "@SwitchToTwo:");
                RandomizeOpcode(asm, "mov edx, " + _addresseInjection);
                RandomizeOpcode(asm, "mov ecx, 2");
                RandomizeOpcode(asm, "mov [edx], ecx");
                RandomizeOpcode(asm, "@loop:");
                RandomizeOpcode(asm, "mov ecx, 3");
                RandomizeOpcode(asm, "cmp ecx, [" + _addresseInjection + "]");
                RandomizeOpcode(asm, "jne @loop");
                RandomizeOpcode(asm, "mov edx, " + _addresseInjection);
                RandomizeOpcode(asm, "mov ecx, 0");
                RandomizeOpcode(asm, "mov [edx], ecx");
                RandomizeOpcode(asm, "@out:");
                RandomizeOpcode(asm, "popfd");
                RandomizeOpcode(asm, "popad");

                int sizeAsm = InjectAsm(asm, (uint) _injectedCode);
                // injecting trampouline
                _wowProcess.Memory.WriteBytes(_injectedCode + sizeAsm, _hookOriginalBytes);
                // injecting return to Endscene/Present
                asm.Clear();
                asm.Add("jmp " + ((uint) _hookPtr + offset + (isDX11 ? 5 : 7)));
                InjectAsm(asm, (uint) (_injectedCode + sizeAsm + _hookOriginalBytes.Length));
                //
                asm.Clear();
                asm.Add("jmp " + (_injectedCode));
                InjectAsm(asm, (uint) _hookPtr + offset);
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Successfully hooked, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                    BitConverter.ToString(_wowProcess.Memory.ReadBytes(_hookPtr + (int) offset, isDX11 ? 5 : 7))));
                return true;
            }
            Log.Print(string.Format("{0}:{1} :: [WoW hook] CGWorldFrame__Render has invalid signature, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, BitConverter.ToString(_hookOriginalBytes)),
                false, false);
            return false;
        }
        
        internal static void Release()
        {
            if (!_wowProcess.Memory.Process.HasExited)
            {
                try
                {
                    bool isDX11 = _wowProcess.Memory.Process.Modules.Cast<ProcessModule>().Any(m => m.ModuleName == "d3d11.dll");
                    uint offset = (uint)(isDX11 ? 0xB : 0x5);
                    _wowProcess.Memory.WriteBytes(_hookPtr + (int)offset, _hookOriginalBytes);
                    Log.Print(
                        string.Format("{0}:{1} :: [WoW hook] Hook is deleted, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                            BitConverter.ToString(_wowProcess.Memory.ReadBytes(_hookPtr + (int)offset, isDX11 ? 5 : 7))));
                }
                catch (Exception ex)
                {
                    Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't delete hook, WoW client is closed? ({2})", _wowProcess.ProcessName, _wowProcess.ProcessID, ex.Message));
                }
                // ReSharper disable EmptyGeneralCatchClause
                try
                {
                    _fasm.Dispose();
                }
                catch
                {

                }
                try
                {
                    _wowProcess.Memory.FreeMemory(_injectedCode);
                    _wowProcess.Memory.FreeMemory(_addresseInjection);
                    _wowProcess.Memory.FreeMemory(_retnInjectionAsm);
                    _wowProcess.Memory.FreeMemory(_codeCavePtr);
                }
                catch
                {
                    
                }
                // ReSharper restore EmptyGeneralCatchClause
            }
            else
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't delete hook: WoW client has been finished", _wowProcess.ProcessName, _wowProcess.ProcessID));
            }
        }

        internal static void LuaDoString(string command)
        {
            //todo
            return;
            //using (MemorySharp sharp = new MemorySharp(Process.GetProcessesByName("Wow")[0].Id))
            //{
            //    //sharp[_wowProcess.Memory.ImageBase + WowBuildInfo.TargetUnit].Execute(CallingConventions.Cdecl, guid);
            //    try
            //    {
            //        sharp.Assembly.Execute(sharp.Modules.MainModule.BaseAddress + WowBuildInfo.LuaDoStringAddress, CallingConventions.Cdecl, command, command, 0);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
            //return;

            

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
            //todo
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
            try
            {
                IntPtr locationPtr = _wowProcess.Memory.AllocateMemory(sizeof(TerrainClickStruct));
                _wowProcess.Memory.Write(locationPtr, clickStruct);
                using (MemorySharp sharp = new MemorySharp(_wowProcess.ProcessID))
                {
                    using (AssemblyTransaction transaction = sharp.Assembly.BeginTransaction())
                    {
                        transaction.AddLine("mov eax, " + locationPtr);
                        transaction.AddLine("push eax");
                        transaction.AddLine("mov eax, " + (_wowProcess.Memory.ImageBase + WowBuildInfo.HandleTerrainClick));
                        transaction.AddLine("call eax");
                        transaction.AddLine("add esp, 0x4");
                        transaction.AddLine("retn");
                    }
                }
                _wowProcess.Memory.FreeMemory(locationPtr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




            //todo
            return;
            IntPtr ctmPoint = _wowProcess.Memory.AllocateMemory(sizeof(TerrainClickStruct));
            _wowProcess.Memory.Write(ctmPoint, clickStruct);
            string[] asm =
            {
                "mov eax, " + ctmPoint,
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
            catch
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
            finally
            {
                _wowProcess.Memory.FreeMemory(ctmPoint);
            }
        }

        //todo
        private static MemorySharp _sharp;
        private static ThreadContext SuspendThreadAndFindGoodEIP()
        {
            uint baseAddress = (uint) _wowProcess.Memory.ImageBase;
            int size = _wowProcess.Memory.Process.MainModule.ModuleMemorySize;
            if (_sharp == null)
            {
                _sharp = new MemorySharp(_wowProcess.ProcessID);
            }
            _sharp.Threads.MainThread.Suspend();
            ThreadContext oldContext = _sharp.Threads.MainThread.Context;
            for (int i = 0; i < 1000; i++)
            {
                if (oldContext.Eip <= 0 || oldContext.Eip <= baseAddress || oldContext.Eip >= baseAddress + size)
                {
                    _sharp.Threads.MainThread.Resume();
                    Thread.Sleep(1);
                    _sharp.Threads.MainThread.Suspend();
                    oldContext = _sharp.Threads.MainThread.Context;
                }
                else
                {
                    return oldContext;
                }
            }
            throw new Exception("EIP not found!");
        }

        //todo
        private static IntPtr _prevCodeCave = IntPtr.Zero;
        private static AllocatedMemory FindGoodCodecave(int needBytes)
        {
            if (_prevCodeCave != IntPtr.Zero)
            {
                byte[] bytes = _wowProcess.Memory.ReadBytes(_prevCodeCave, needBytes);
                if (bytes.SequenceEqual(new byte[needBytes]))
                {
                    return new AllocatedMemory(_prevCodeCave, needBytes);
                }
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < needBytes; i++)
            {
                sb.Append("00 ");
            }
            string patternString = sb.ToString().Trim();
            Pattern zeroBytes = Pattern.FromTextstyle("Free space", patternString, new AddModifier(0), new LeaModifier(LeaType.SimpleAddress));
            try
            {
                IntPtr address = IntPtr.Zero;
                foreach (IntPtr i in zeroBytes.Find(_wowProcess.Memory, 0, 0xFFFFFF))
                {
                    address = i;
                    break;
                }
                address = _wowProcess.Memory.ImageBase + (int) address;
                _prevCodeCave = address;
                return new AllocatedMemory(_prevCodeCave, needBytes);
            }
            catch (Exception)
            {
                throw new Exception("Can't find memory for codecave!");
            }
        }

        private class Pattern
        {
            public string Name { get; private set; }
            public byte[] Bytes { get; private set; }
            public bool[] Mask { get; private set; }
            const int CacheSize = 0x500;
            public List<IModifier> Modifiers = new List<IModifier>();

            private bool DataCompare(byte[] data, uint dataOffset)
            {
                return !Mask.Where((t, i) => t && Bytes[i] != data[dataOffset + i]).Any();
            }

            private IEnumerable<IntPtr> FindStart(ExternalProcessReader bm, int startAddress, int endAddress)
            {
                var mainModule = bm.Process.MainModule;
                var start = mainModule.BaseAddress + startAddress;
                var size = Math.Min(mainModule.ModuleMemorySize, endAddress);
                var patternLength = Bytes.Length;
                for (int i = (size - patternLength); i >= 0; i -= (CacheSize - patternLength))
                {
                    byte[] cache = bm.ReadBytes(start + i, CacheSize > size - i ? size - i : CacheSize);
                    for (uint i2 = 0; i2 < (cache.Length - patternLength); i2++)
                    {
                        if (DataCompare(cache, i2))
                        {
                            yield return start + (int)(i + i2);
                        }
                    }
                }
            }

            public IEnumerable<IntPtr> Find(ExternalProcessReader bm, int startAddress, int endAddress)
            {
                foreach (IntPtr intPtr in FindStart(bm, startAddress, endAddress))
                {
                    IntPtr start = intPtr;
                    foreach (IModifier modifier in Modifiers)
                    {
                        start = modifier.Apply(bm, start);
                    }
                    yield return start - (int) bm.Process.MainModule.BaseAddress;
                }
            }

            public static Pattern FromTextstyle(string name, string pattern, params IModifier[] modifiers)
            {
                var ret = new Pattern { Name = name };
                if (modifiers != null)
                    ret.Modifiers = modifiers.ToList();
                var split = pattern.Split(' ');
                int index = 0;
                ret.Bytes = new byte[split.Length];
                ret.Mask = new bool[split.Length];
                foreach (var token in split)
                {
                    if (token.Length > 2)
                        throw new InvalidDataException("Invalid token: " + token);
                    if (token.Contains("?"))
                        ret.Mask[index++] = false;
                    else
                    {
                        byte data = byte.Parse(token, NumberStyles.HexNumber);
                        ret.Bytes[index] = data;
                        ret.Mask[index] = true;
                        index++;
                    }
                }
                return ret;
            }
        }

        private interface IModifier
        {
            IntPtr Apply(ExternalProcessReader bm, IntPtr address);
        }

        private class AddModifier : IModifier
        {
            public uint Offset { get; private set; }

            public AddModifier(uint val)
            {
                Offset = val;
            }

            public IntPtr Apply(ExternalProcessReader bm, IntPtr addr)
            {
                return (addr + (int)Offset);
            }
        }

        private enum LeaType
        {
            Byte,
            Word,
            Dword,
            E8,
            SimpleAddress
        }

        private class LeaModifier : IModifier
        {
            public LeaType Type { get; private set; }

            public LeaModifier()
            {
                Type = LeaType.Dword;
            }

            public LeaModifier(LeaType type)
            {
                Type = type;
            }

            public IntPtr Apply(ExternalProcessReader bm, IntPtr address)
            {
                switch (Type)
                {
                    case LeaType.Byte:
                        return (IntPtr)bm.Read<byte>(address);
                    case LeaType.Word:
                        return (IntPtr)bm.Read<ushort>(address);
                    case LeaType.Dword:
                        return (IntPtr)bm.Read<uint>(address);
                    case LeaType.E8:
                        return address + 4 + bm.Read<int>(address); // 4 = <call instruction size> - <E8>
                    case LeaType.SimpleAddress:
                        return address;
                }
                throw new InvalidDataException("Unknown LeaType");
            }
        }

        private class AllocatedMemory : IDisposable
        {
            public readonly IntPtr Address;
            private readonly uint prevMemoryProtectionType;
            private readonly UIntPtr len;

            public AllocatedMemory(IntPtr address, int length)
            {
                Address = address;
                len = (UIntPtr) length;
                VirtualProtectEx(_wowProcess.Memory.ProcessHandle, Address, len, (uint)MemoryProtectionType.PAGE_EXECUTE_READWRITE, out prevMemoryProtectionType);
            }

            public void Dispose()
            {
                uint temp;
                VirtualProtectEx(_wowProcess.Memory.ProcessHandle, Address, len, prevMemoryProtectionType, out temp);
                Log.Print("Memory address disposed: 0x" + Address.ToInt32().ToString("X"));
            }

            [DllImport("kernel32.dll")]
            private static extern bool VirtualProtectEx(SafeMemoryHandle hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        }

        

        internal static unsafe void MoveTo(WowPoint point)
        {
            //try
            //{
            //    IntPtr localPlayerPtr = _wowProcess.Memory.Read<IntPtr>((IntPtr)WowBuildInfo.PlayerPtr, true);
            //    IntPtr locationPtr = _wowProcess.Memory.AllocateMemory(0x4 * 3);
            //    IntPtr guidPtr = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
            //    _wowProcess.Memory.Write(locationPtr, point.X);
            //    _wowProcess.Memory.Write(locationPtr + 0x4, point.Y);
            //    _wowProcess.Memory.Write(locationPtr + 0x8, point.Z);
            //    using (MemorySharp sharp = new MemorySharp(_wowProcess.ProcessID))
            //    {
            //        //IntPtr p = _wowProcess.Memory.AllocateMemory(0x1000);
            //        //MessageBox.Show(((uint) p).ToString("X"));
            //        //_wowProcess.Memory.Write(_addresseInjection, 1);
            //        //sharp.Assembly.InjectAndExecute(new[]
            //        //{
            //        //    "@start:",
            //        //    "mov ecx, 2",
            //        //    "cmp ecx, [" + _addresseInjection + "]",
            //        //    "jne @start",

            //        //    "mov ecx, " + localPlayerPtr,
            //        //    "mov eax, " + guidPtr,
            //        //    "mov edx, " + locationPtr,
            //        //    "push 0",
            //        //    "push edx",
            //        //    "push eax",
            //        //    "push 4",
            //        //    "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClickToMove),
            //        //    "mov edx, " + _addresseInjection,
            //        //    "mov ecx, 3",
            //        //    "mov [edx], ecx",
            //        //    "retn"
            //        //}, p);
            //        //_wowProcess.Memory.FreeMemory(p);
            //        using (AssemblyTransaction transaction = sharp.Assembly.BeginTransaction())
            //        {
            //            transaction.AddLine("mov ecx, " + localPlayerPtr);
            //            transaction.AddLine("mov eax, " + guidPtr);
            //            transaction.AddLine("mov edx, " + locationPtr);
            //            transaction.AddLine("push 0");
            //            transaction.AddLine("push edx");
            //            transaction.AddLine("push eax");
            //            transaction.AddLine("push 4");
            //            transaction.AddLine("call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClickToMove));
            //            transaction.AddLine("retn");
            //        }
            //    }
            //    _wowProcess.Memory.FreeMemory(locationPtr);
            //    _wowProcess.Memory.FreeMemory(guidPtr);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    IntPtr localPlayerPtr = _wowProcess.Memory.Read<IntPtr>((IntPtr)WowBuildInfo.PlayerPtr, true);
                    IntPtr locationPtr = _wowProcess.Memory.AllocateMemory(0x4 * 3);
                    IntPtr guidPtr = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
                    _wowProcess.Memory.Write(locationPtr, point.X);
                    _wowProcess.Memory.Write(locationPtr + 0x4, point.Y);
                    _wowProcess.Memory.Write(locationPtr + 0x8, point.Z);
                    using (MemorySharp sharp = new MemorySharp(_wowProcess.ProcessID))
                    {
                        IntPtr p = _wowProcess.Memory.AllocateMemory(0x1000);
                        _wowProcess.Memory.Write(_addresseInjection, 1);
                        sharp.Assembly.InjectAndExecute(new[]
                        {
                            "@start:",
                            "mov ecx, 2",
                            "cmp ecx, [" + _addresseInjection + "]",
                            "jne @start",

                            "mov ecx, " + localPlayerPtr,
                            "mov eax, " + guidPtr,
                            "mov edx, " + locationPtr,
                            "push 0",
                            "push edx",
                            "push eax",
                            "push 4",
                            "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClickToMove),
                            "mov edx, " + _addresseInjection,
                            "mov ecx, 3",
                            "mov [edx], ecx",
                            "retn"
                        }, p);
                        _wowProcess.Memory.FreeMemory(p);
                        //using (AssemblyTransaction transaction = sharp.Assembly.BeginTransaction())
                        //{
                        //    transaction.AddLine("mov ecx, " + localPlayerPtr);
                        //    transaction.AddLine("mov eax, " + guidPtr);
                        //    transaction.AddLine("mov edx, " + locationPtr);
                        //    transaction.AddLine("push 0");
                        //    transaction.AddLine("push edx");
                        //    transaction.AddLine("push eax");
                        //    transaction.AddLine("push 4");
                        //    transaction.AddLine("call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClickToMove));
                        //    transaction.AddLine("retn");
                        //}
                    }
                    _wowProcess.Memory.FreeMemory(locationPtr);
                    _wowProcess.Memory.FreeMemory(guidPtr);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            
            //IntPtr localPlayerPtr = _wowProcess.Memory.Read<IntPtr>((IntPtr)WowBuildInfo.PlayerPtr, true);
            //IntPtr locationPtr = _wowProcess.Memory.AllocateMemory(0x4 * 3);
            //IntPtr guidPtr = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
            //_wowProcess.Memory.Write(locationPtr, point.X);
            //_wowProcess.Memory.Write(locationPtr + 0x4, point.Y);
            //_wowProcess.Memory.Write(locationPtr + 0x8, point.Z);
            //ThreadContext threadContext = SuspendThreadAndFindGoodEIP();
            //_fasm.Clear();
            //_fasm.AddLine("push " + threadContext.Eip);
            //_fasm.AddLine("pushad");
            //_fasm.AddLine("pushfd");
            //_fasm.AddLine("mov ecx, " + localPlayerPtr);
            //_fasm.AddLine("mov eax, " + guidPtr);
            //_fasm.AddLine("mov edx, " + locationPtr);
            //_fasm.AddLine("push 0");
            //_fasm.AddLine("push edx");
            //_fasm.AddLine("push eax");
            //_fasm.AddLine("push 4");
            //_fasm.AddLine("call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClickToMove));
            //_fasm.AddLine("popfd");
            //_fasm.AddLine("popad");
            //_fasm.AddLine("retn");
            //AllocatedMemory codeCave = FindGoodCodecave(_fasm.Assemble().Length + 1);
            //_fasm.Inject((uint)codeCave.Address);
            //threadContext.Eip = (uint) codeCave.Address;

            //_sharp.Threads.MainThread.Context = threadContext;
            //_sharp.Threads.MainThread.Resume();
            //Task.Factory.StartNew(() =>
            //{
            //    Thread.Sleep(2000);
            //    codeCave.Dispose();
            //});
            }
            Log.Print("stopwatch: " + stopwatch.ElapsedMilliseconds);




            //todo
            return;
            IntPtr ctmPoint = _wowProcess.Memory.AllocateMemory(0x4 * 3);
            IntPtr ctmGUID = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
            _wowProcess.Memory.Write(ctmPoint, point.X);
            _wowProcess.Memory.Write(ctmPoint + 0x4, point.Y);
            _wowProcess.Memory.Write(ctmPoint + 0x8, point.Z);
            string[] asm =
            {
                "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.ClntObjMgrGetActivePlayerObj),
                "mov ecx, eax",
                "mov eax, " + ctmGUID,
                "mov edx, " + ctmPoint,
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
            catch
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
            finally
            {
                _wowProcess.Memory.FreeMemory(ctmPoint);
                _wowProcess.Memory.FreeMemory(ctmGUID);
            }
        }

        internal static unsafe void TargetUnit(UInt128 guid)
        {
            // cdecl
            try
            {
                IntPtr address = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
                _wowProcess.Memory.Write(address, guid);
                using (MemorySharp sharp = new MemorySharp(_wowProcess.ProcessID))
                {
                    using (AssemblyTransaction transaction = sharp.Assembly.BeginTransaction())
                    {
                        transaction.AddLine("push " + address);
                        transaction.AddLine("call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.TargetUnit));
                        transaction.AddLine("add esp, 0x4");
                        transaction.AddLine("retn");
                    }
                }
                _wowProcess.Memory.FreeMemory(address);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




            //todo
            return;
            lock (FASMLock)
            {
                IntPtr address = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
                _wowProcess.Memory.Write(address, guid);
                string[] asm =
                {
                    "push " + address,
                    "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.TargetUnit),
                    "add esp, 0x4",
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
            }
        }

        internal static unsafe void Interact(UInt128 guid)
        {
            //todo
            return;
            lock (FASMLock)
            {
                IntPtr address = _wowProcess.Memory.AllocateMemory(sizeof(UInt128));
                _wowProcess.Memory.Write(address, guid);
                string[] asm =
                {
                    "push " + address,
                    "call " + (_wowProcess.Memory.ImageBase + WowBuildInfo.Interact),
                    "add esp, 0x4",
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
            }
        }

        private static void InjectVoid(IEnumerable<string> asm)
        {
            InjectAsm(asm, (uint)_codeCavePtr);
            _wowProcess.Memory.Write(_addresseInjection, (int)_codeCavePtr);
            while (_wowProcess.Memory.Read<uint>(_addresseInjection) > 0)
            {
                Thread.Sleep(1);
            }
            _wowProcess.Memory.WriteBytes(_codeCavePtr, Eraser);
        }

        private static string InjectReturn(IEnumerable<string> asm)
        {
            _wowProcess.Memory.Write(_retnInjectionAsm, 0);
            InjectAsm(asm, (uint)_codeCavePtr);
            _wowProcess.Memory.Write(_addresseInjection, (int)_codeCavePtr);
            while (_wowProcess.Memory.Read<uint>(_addresseInjection) > 0)
            {
                Thread.Sleep(1);
            }
            List<byte> retnByte = new List<byte>();
            uint dwAddress = _wowProcess.Memory.Read<uint>(_retnInjectionAsm);
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

        internal static void MoveHook()
        {
            //int playclawHookAbsoluteAddress = _wowProcess.Memory.Read<int>(_dxAddress.HookPtr + 1) + (int)_dxAddress.HookPtr + 5;
            //if (MessageBox.Show("playclawHookAbsoluteAddress" + playclawHookAbsoluteAddress.ToString("X"), "AxTools", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    int internalFunctionAbsoluteAddress = _wowProcess.Memory.Read<int>(_dxAddress.HookPtr + 0x2E) + (int)_dxAddress.HookPtr + 0x2D + 5;
            //    if (MessageBox.Show("internalFunctionAbsoluteAddress" + internalFunctionAbsoluteAddress.ToString("X"), "AxTools", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        string[] asm =
            //        {
            //            "jmp " + playclawHookAbsoluteAddress
            //        };
            //        InjectAsm(asm, (uint) internalFunctionAbsoluteAddress);
            //        _wowProcess.Memory.WriteBytes(_dxAddress.HookPtr, new byte[] { 0x8B, 0xFF, 0x55, 0x8B, 0xEC });
            //    }
            //}

            //byte[] origBytes = _wowProcess.Memory.ReadBytes(_dxAddress.HookPtr + 29, 5);
            //IntPtr myCodeCave = _wowProcess.Memory.AllocateMemory(2048);
            //string[] asm =
            //{
            //    "pushad",
            //    "pushfd",
            //    "jmp " + (playclawHookAbsoluteAddress + 3),
            //    "popfd",
            //    "popad"
            //};
            //int offset = InjectAsm(asm, (uint)myCodeCave);
            //offset += _wowProcess.Memory.WriteBytes(myCodeCave + offset, origBytes);
            //asm = new[]
            //{
            //    "jmp " + (_dxAddress.HookPtr + 29 + 5)
            //};
            //InjectAsm(asm, (uint)(myCodeCave + offset));
            //MessageBox.Show(myCodeCave.ToInt32().ToString("X"));
            //asm = new[]
            //{
            //    "jmp " + myCodeCave
            //};
            //_wowProcess.Memory.WriteBytes(_dxAddress.HookPtr, new byte[] { 0x8B, 0xFF, 0x55, 0x8B, 0xEC });
            //InjectAsm(asm, (uint)(_dxAddress.HookPtr + 29));
        }

    }
}
