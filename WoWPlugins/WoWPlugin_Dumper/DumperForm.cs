using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_Dumper
{
    public partial class DumperForm : Form
    {
        private readonly Dumper dumper;
        private SafeTimer chatTimer;

        public DumperForm(Dumper dumperInstance)
        {
            InitializeComponent();
            dumper = dumperInstance;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            string origText = ((Button) sender).Text;
            ((Button) sender).Text = "Please wait";
            progressBar1.Maximum = 8;
            progressBar1.Value = 0;
            Dump();
            ((Button)sender).Text = origText;
            MessageBox.Show("Dump is completed, see log file");
        }

        private void Log(string text)
        {
            dumper.LogPrint(text);
        }

        private void Dump()
        {
            Log("Dump START");
            List<WowPlayer> wowUnits = new List<WowPlayer>();
            List<WowObject> wowObjects = new List<WowObject>();
            List<WowNpc> wowNpcs = new List<WowNpc>();
            WoWPlayerMe localPlayer;
            try
            {
                localPlayer = ObjMgr.Pulse(wowObjects, wowUnits, wowNpcs);
                if (localPlayer != null)
                {
                    Log("Dump OK");
                }
                else
                {
                    Log("ERROR: localPlayer is null!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log("ERROR(0): " + ex.Message);
                return;
            }
            try
            {
                Log("Local player---------------------------------------");
                Log(string.Format("\tGUID: 0x{0}; Address: 0x{1:X}; Location: {2}; ZoneID: {3}; ZoneName: {4}; IsLooting: {5}; Name: {6}; TargetGUID: {7}; Class: {8}; Health/MaxHealth: {9}/{10}, Level: {11}; Faction: {12}; IsMounted: {13}",
                    localPlayer.GUID, localPlayer.Address.ToInt64(), localPlayer.Location, Info.ZoneID, Info.ZoneText, Info.IsLooting, localPlayer.Name, localPlayer.TargetGUID, localPlayer.Class,
                    localPlayer.Health, localPlayer.HealthMax,localPlayer.Level, localPlayer.Faction, localPlayer.IsMounted));
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("----Local player buffs----");
                foreach (string info in localPlayer.Auras.Select(l => string.Format("ID: {0}; Name: {1}; Stack: {2}; TimeLeft: {3}; OwnerGUID: {4}", l.SpellId, l.Name, l.Stack, l.TimeLeftInMs, l.OwnerGUID)))
                {
                    Log("\t" + info);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("----Mouseover----");
                Log(string.Format("\tGUID: {0}", Info.MouseoverGUID));
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            progressBar1.Value++;
            progressBar1.Value++;
            progressBar1.Value++;
            try
            {
                Log("Objects-----------------------------------------");
                foreach (WowObject i in wowObjects)
                {
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    Log(string.Format("\t{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; OwnerGUID: 0x{4}; Address: 0x{5:X}; EntryID: {6}", i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), i.OwnerGUID,
                        i.Address.ToInt64(), i.EntryID));
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            progressBar1.Value++;
            try
            {
                Log("Npcs-----------------------------------------");
                Log("NPC count: " + wowNpcs.Count);
                foreach (WowNpc i in wowNpcs)
                {
                    //Log(string.Format("\t{0}; Location: {1}; Distance: {2}; HP:{3}; MaxHP:{4}; Address:0x{5:X}; GUID:0x{6}; EntryID: {7}", i.Name, i.Location,
                    //    (int)i.Location.Distance(localPlayer.Location), i.Health, i.HealthMax, i.Address.ToInt64(), i.GUID, i.EntryID));
                    Log(string.Format("\t{0}; EntryID: {1}; Location: {2}", i.Name, i.EntryID, i.Location));
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            progressBar1.Value++;
            try
            {
                Log("Players-----------------------------------------");
                foreach (WowPlayer i in wowUnits)
                {
                    Log(string.Format(
                        "\t{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; Address:0x{4:X}; Class:{5}; Level:{6}; HP:{7}; MaxHP:{8}; TargetGUID: 0x{9}; IsAlliance:{10}; Auras: {{ {11} }}",
                        i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), i.Address.ToInt64(), i.Class, i.Level, i.Health, i.HealthMax,
                        i.TargetGUID, i.Faction, string.Join(",", i.Auras.Select(l => l.Name + "::" + l.Stack + "::" + l.TimeLeftInMs + "::" + l.OwnerGUID.ToString()))));
                }
            }
            catch (Exception)
            {
                //
            }
            progressBar1.Value++;
            progressBar1.Value++;
            progressBar1.Value++;
        }

        private void Chat_NewMessage(ChatMsg msg)
        {
            //Log(string.Format("\tType: {0}; Channel: {1}; Sender: {2}; SenderGUID: {3}; Text: {4}", msg.Type, msg.Channel, msg.Sender, msg.SenderGUID, msg.Text));
            textBox1.AppendText(string.Format("\tType: {0}; Channel: {1}; Sender: {2}; SenderGUID: {3}; Text: {4}\r\n", msg.Type, msg.Channel, msg.Sender, msg.SenderGUID, msg.Text));
        }

        private void buttonDumpobjects_Click(object sender, EventArgs e)
        {
            Log("Dump START");
            List<WowPlayer> wowUnits = new List<WowPlayer>();
            List<WowObject> wowObjects = new List<WowObject>();
            List<WowNpc> wowNpcs = new List<WowNpc>();
            try
            {
                ObjMgr.Pulse(wowObjects, wowUnits, wowNpcs);
                Log("Dump OK");
            }
            catch (Exception ex)
            {
                Log("ERROR(0): " + ex.Message);
                return;
            }
            try
            {
                Log("Objects-----------------------------------------");
                foreach (WowObject i in wowObjects)
                {
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    Log(string.Format("\t{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; OwnerGUID: 0x{4}; Address: 0x{5:X}; EntryID: {6}", i.Name, i.GUID, i.Location, "n/a", i.OwnerGUID,
                        i.Address.ToInt64(), i.EntryID));
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("Npcs-----------------------------------------");
                Log("NPC count: " + wowNpcs.Count);
                foreach (WowNpc i in wowNpcs)
                {
                    Log($"\t{i.Name}; Location: {i.Location}; Distance: {"n/a"}; Address: 0x{i.Address.ToInt64().ToString("X")} HP:{i.Health}; MaxHP:{i.HealthMax}; GUID:0x{i.GUID}; EntryID: {i.EntryID}");
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("Players-----------------------------------------");
                Log("Players count: " + wowUnits.Count);
                foreach (WowPlayer i in wowUnits)
                {
                    Log($"\t{i.Name} - GUID: 0x{i.GUID}; Location: {i.Location}; Distance: {"n/a"}; Address:0x{i.Address.ToInt64().ToString("X")}; Class:{i.Class}; Level:{i.Level}; HP:{i.Health}; MaxHP:{i.HealthMax}; " +
                        $"TargetGUID: 0x{i.TargetGUID}; IsAlliance:{i.Faction}; Auras: {{ {""} }}; GUIDBytes: {BitConverter.ToString(i.GetGUIDBytes())}"); // string.Join(",", i.Auras.Select(l => l.Name + "::" + l.Stack + "::" + l.TimeLeftInMs + "::" + l.OwnerGUID.ToString()))
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
            }
            MessageBox.Show("Completed");
        }

        private async void buttonUIFrames_Click(object sender, EventArgs e)
        {
            try
            {
                Log("UIFrames-----------------------------------------");
                foreach (WoWUIFrame frame in WoWUIFrame.GetAllFrames())
                {
                    Log(string.Format("\tName: {0}; Visible: {1}; EditboxText: {2}", frame.GetName, frame.IsVisible, frame.EditboxText));
                }
                await Task.Run(() => Log(Lua.GetValue("UnitHealth(\"player\")")));
                if (await Task.Run(() => Lua.IsTrue("1==1")))
                {
                    MessageBox.Show("Completed, lua is working properly");
                }
                else
                {
                    MessageBox.Show("Completed, lua is NOT working properly");
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                MessageBox.Show("Operation FAILED\r\n" + ex.Message);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Chat messages-----------------------------------------");
                if (chatTimer == null)
                {
                    ChatMessages.NewChatMessage += Chat_NewMessage;
                    (chatTimer = dumper.CreateTimer(1000, ChatMessages.ReadChat)).Start();
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private void ButtonDumpInventory_Click(object sender, EventArgs e)
        {
            Log("Dump START (buttonDumpInventory_Click)");
            WoWPlayerMe me = null;
            try
            {
                me = ObjMgr.Pulse();
                Log("Dump OK");
            }
            catch (Exception ex)
            {
                Log("ERROR(0): " + ex.Message);
                return;
            }
            try
            {
                Log("----Inventory slots----");
                int counter = 0;
                foreach (WoWItem item in me.Inventory)
                {
                    Log(string.Format("\tSlot: {5}; ID: {0}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.Enchant, counter));
                    counter++;
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("----Items in bags----");
                foreach (WoWItem item in me.ItemsInBags)
                {
                    Log(string.Format("\tID: {0}; GUID: {7}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}; BagID, SlotID: {5} {6}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.Enchant,
                        item.BagID, item.SlotID, item.GUID));
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            MessageBox.Show("Completed");
        }

        private void DumperForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ChatMessages.NewChatMessage -= Chat_NewMessage;
            chatTimer?.Dispose();
        }

        private void buttonDumpPlayer_Click(object sender, EventArgs e)
        {
            WoWPlayerMe lp;
            try
            {
                lp = ObjMgr.Pulse();
                if (lp != null)
                {
                    Log("Dump OK");
                }
                else
                {
                    Log("ERROR: localPlayer is null!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log("ERROR(0): " + ex.Message);
                return;
            }
            try
            {
                Log("Info---------------------------------------");
                Log($"\tZoneID: {Info.ZoneID}");
                Log($"\tZoneText: {Info.ZoneText}");
                Log($"\tIsLooting: {Info.IsLooting}");
                Log($"\tCastingSpellID: {lp.CastingSpellID}");
                Log($"\tChannelSpellID: {lp.ChannelSpellID}");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("Local player---------------------------------------");
                Log($"\tGUID: {lp.GUID}; Address: 0x{lp.Address.ToInt64().ToString("X")}; Location: {lp.Location}; "+
                    $"Name: {lp.Name}; TargetGUID: {lp.TargetGUID}; Class: {lp.Class}; Health/MaxHealth: {lp.Health}/{lp.HealthMax}, Level: {lp.Level}; Faction: {lp.Faction}; IsMounted: {lp.IsMounted}");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("----Local player buffs----");
                foreach (string info in lp.Auras.Select(l => string.Format("ID: {0}; Name: {1}; Stack: {2}; TimeLeft: {3}; OwnerGUID: {4}", l.SpellId, l.Name, l.Stack, l.TimeLeftInMs, l.OwnerGUID)))
                {
                    Log("\t" + info);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("----Mouseover----");
                Log(string.Format("\tGUID: {0}", Info.MouseoverGUID));
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            MessageBox.Show("Completed");
        }

    }
}
