using AxTools.Classes;
using AxTools.Forms;
using AxTools.WoW.DX;
using AxTools.WoW.Management.ObjectManager;
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

namespace AxTools.WoW.Management
{
    internal static class WoWDXInject
    {
        private static WowProcess _wowProcess;
        private static byte[] _endSceneOriginalBytes;
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
        #region Ingame overlay initialization string

        private static readonly string InitializeIngameOverlay = ("if (AxToolsMainOverlayChildren == nil) then\r\n" +
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
        private static ManagedFasm _fasm;
        private static DirectX3D _dxAddress;

        internal static bool Apply(WowProcess process)
        {
            _wowProcess = process;
            _dxAddress = new DirectX3D(Process.GetProcessById(_wowProcess.ProcessID));
            //GreyMagic.AllocatedMemory allocatedMemory = _wowProcess.Memory.CreateAllocatedMemory(20);
            _fasm = _wowProcess.Memory.Asm;
            _fasm.SetMemorySize(0x4096);
            _codeCavePtr = _wowProcess.Memory.AllocateMemory(CodeCaveSize);
            uint offset;
            Log.Print(string.Format("{0}:{1} :: [WoW hook] DX version: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, _dxAddress.UsingDirectX11 ? 11 : 9), false, false);
            if (_dxAddress.UsingDirectX11)
            {
                offset = 0xB;
                _endSceneOriginalBytes = _wowProcess.Memory.ReadBytes(_dxAddress.HookPtr + (int)offset, 5);
                if (_endSceneOriginalBytes[0] != 0xA1)
                {
                    if (_endSceneOriginalBytes[0] == 0xE9)
                    {
                        TaskDialogButton yesNo = TaskDialogButton.Yes + (int)TaskDialogButton.No;
                        TaskDialog taskDialog = new TaskDialog("DirectX is already hooked by some tool", "AxTools", "Do you want to overwrite this hook? It may cause a crash!", yesNo, TaskDialogIcon.Warning);
                        if (taskDialog.Show(MainForm.Instance).CommonButton == Result.Yes)
                        {
                            Log.Print(
                                string.Format("{0}:{1} :: [WoW hook] DX is already hooked, trying to overwrite, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                                    BitConverter.ToString(_endSceneOriginalBytes)), false, false);
                        }
                        else
                        {
                            Log.Print(
                                string.Format("{0}:{1} :: [WoW hook] DX is already hooked, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                                    BitConverter.ToString(_endSceneOriginalBytes)), false, false);
                            return false;
                        }
                    }
                    else
                    {
                        Log.Print(string.Format("{0}:{1} :: [WoW hook] Incorrect DX version, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID, BitConverter.ToString(_endSceneOriginalBytes)), true);
                        return false;
                    }
                }
            }
            else
            {
                offset = 0x5;
                _endSceneOriginalBytes = _wowProcess.Memory.ReadBytes(_dxAddress.HookPtr + (int)offset, 7);
            }
            IntPtr dxgiAddress = new IntPtr(0);
            foreach (ProcessModule module in _wowProcess.Memory.Process.Modules.Cast<ProcessModule>().Where(module => module.ModuleName == "dxgi.dll"))
            {
                dxgiAddress = module.BaseAddress;
            }
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Original bytes: {2}, address: 0x{3:X}; it's dxgi.dll + {4:X}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                BitConverter.ToString(_endSceneOriginalBytes), (uint) _dxAddress.HookPtr + offset, (uint) (_dxAddress.HookPtr - (int) dxgiAddress)), false, false);
            // allocate memory to store injected code:
            _injectedCode = _wowProcess.Memory.AllocateMemory(2048);
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Loop code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint)_injectedCode));
            // allocate memory the new injection code pointer:
            _addresseInjection = _wowProcess.Memory.AllocateMemory(0x4);
            _wowProcess.Memory.Write(_addresseInjection, 0);
            Log.Print(string.Format("{0}:{1} :: [WoW hook] User code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint)_addresseInjection));
            // allocate memory the pointer return value:
            _retnInjectionAsm = _wowProcess.Memory.AllocateMemory(0x4);
            _wowProcess.Memory.Write(_retnInjectionAsm, 0);
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Return value code address: 0x{2:X}", _wowProcess.ProcessName, _wowProcess.ProcessID, (uint)_retnInjectionAsm));
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
            _wowProcess.Memory.WriteBytes(_injectedCode + sizeAsm, _endSceneOriginalBytes);
            // injecting return to Endscene/Present
            asm.Clear();
            asm.Add("jmp " + ((uint)_dxAddress.HookPtr + offset + (_dxAddress.UsingDirectX11 ? 5 : 7)));
            InjectAsm(asm, (uint)(_injectedCode + sizeAsm + _endSceneOriginalBytes.Length));
            //
            asm.Clear();
            asm.Add("jmp " + (_injectedCode));
            InjectAsm(asm, (uint)_dxAddress.HookPtr + offset);
            Log.Print(string.Format("{0}:{1} :: [WoW hook] Successfully hooked, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                BitConverter.ToString(_wowProcess.Memory.ReadBytes(_dxAddress.HookPtr + (int)offset, _dxAddress.UsingDirectX11 ? 5 : 7))));
            return true;
        }
        
        internal static void Release()
        {
            if (!_wowProcess.Memory.Process.HasExited)
            {
                try
                {
                    uint offset = (uint) (_dxAddress.UsingDirectX11 ? 0xB : 0x5);
                    _wowProcess.Memory.WriteBytes(_dxAddress.HookPtr + (int) offset, _endSceneOriginalBytes);
                    Log.Print(
                        string.Format("{0}:{1} :: [WoW hook] Hook is deleted, bytes: {2}", _wowProcess.ProcessName, _wowProcess.ProcessID,
                            BitConverter.ToString(_wowProcess.Memory.ReadBytes(_dxAddress.HookPtr + (int) offset, _dxAddress.UsingDirectX11 ? 5 : 7))));
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
                // ReSharper restore EmptyGeneralCatchClause
                try
                {
                    _dxAddress.Device.Dispose();
                    _dxAddress = null;
                }
                finally
                {
                    _wowProcess.Memory.FreeMemory(_injectedCode);
                    _wowProcess.Memory.FreeMemory(_addresseInjection);
                    _wowProcess.Memory.FreeMemory(_retnInjectionAsm);
                    _wowProcess.Memory.FreeMemory(_codeCavePtr);
                }
            }
            else
            {
                Log.Print(string.Format("{0}:{1} :: [WoW hook] Can't delete hook: WoW client has been finished", _wowProcess.ProcessName, _wowProcess.ProcessID));
            }
        }

        internal static void LuaDoString(string command)
        {
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
            //LuaDoString("UIErrorsFrame:AddMessage(\"Plugin <" + moduleTask.Name + "> is started\", 0.0, 1.0, 0.0)");
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
            LuaDoString(InitializeIngameOverlay + "\r\n" + function);
        }

        internal static unsafe void TerrainClick(TerrainClickStruct clickStruct)
        {
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

        internal static void MoveTo(WowPoint point)
        {
            IntPtr ctmPoint = _wowProcess.Memory.AllocateMemory(0x4 * 3);
            IntPtr ctmGUID = _wowProcess.Memory.AllocateMemory(0x4 * 2);
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

        internal static void TargetUnit(ulong guid)
        {
            lock (FASMLock)
            {
                uint dwHiWord = (uint)(guid >> 32);
                uint dwLoWord = (uint)guid;
                string[] asm = {
                    "mov eax, " + dwHiWord,
                    "push eax",
                    "mov eax, " + dwLoWord,
                    "push eax",
                    "mov eax, " + (_wowProcess.Memory.ImageBase + WowBuildInfo.SelectTarget),
                    "call eax",
                    "add esp, 0x8",
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

        internal static void Interact(ulong guid)
        {
            lock (FASMLock)
            {
                uint dwHiWord = (uint)(guid >> 32);
                uint dwLoWord = (uint)guid;
                string[] asm = {
                    "mov eax, " + dwHiWord,
                    "push eax",
                    "mov eax, " + dwLoWord,
                    "push eax",
                    "mov eax, " + (_wowProcess.Memory.ImageBase + WowBuildInfo.Interact),
                    "call eax",
                    "add esp, 0x8",
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
