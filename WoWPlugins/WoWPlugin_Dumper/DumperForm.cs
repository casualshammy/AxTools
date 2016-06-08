using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_Dumper
{
    public partial class DumperForm : Form
    {
        private readonly Dumper dumper;

        public DumperForm(Dumper dumperInstance)
        {
            InitializeComponent();
            dumper = dumperInstance;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            string origText = ((Button) sender).Text;
            ((Button) sender).Text = "Please wait";
            progressBar1.Maximum = 7;
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
            Log("Local player---------------------------------------");
            Log(string.Format("\tGUID: 0x{0}; Address: 0x{1:X}; Location: {2}; ZoneID: {3}; ZoneName: {4}; IsLooting: {5}; Name: {6}", localPlayer.GUID, localPlayer.Address.ToInt64(), localPlayer.Location,
                GameFunctions.ZoneID, GameFunctions.ZoneText, GameFunctions.IsLooting, localPlayer.Name));
            Log("----Local player buffs----");
            foreach (string info in localPlayer.Auras.AsParallel().Select(l => string.Format("ID: {0}; Name: {1}; Stack: {2}; TimeLeft: {3}; OwnerGUID: {4}", l.SpellId, l.Name, l.Stack, l.TimeLeftInMs, l.OwnerGUID)))
            {
                Log("\t" + info);
            }
            Log("----Mouseover----");
            Log(string.Format("\tGUID: {0}", GameFunctions.MouseoverGUID));
            progressBar1.Value++;
            Log("----Inventory slots----");
            foreach (WoWItem item in localPlayer.Inventory.AsParallel())
            {
                Log(string.Format("\tID: {0}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.Enchant));
            }
            progressBar1.Value++;
            Log("----Items in bags----");
            foreach (WoWItem item in localPlayer.ItemsInBags)
            {
                Log(string.Format("\tID: {0}; GUID: {7}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}; BagID, SlotID: {5} {6}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.Enchant,
                    item.BagID, item.SlotID, item.GUID));
            }
            progressBar1.Value++;
            Log("Objects-----------------------------------------");
            foreach (WowObject i in wowObjects)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                Log(string.Format("\t{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; OwnerGUID: 0x{4}; Address: 0x{5:X}; EntryID: {6}", i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), i.OwnerGUID,
                    i.Address.ToInt64(), i.EntryID));
            }
            progressBar1.Value++;
            Log("Npcs-----------------------------------------");
            foreach (WowNpc i in wowNpcs)
            {
                Log(string.Format("\t{0}; Location: {1}; Distance: {2}; HP:{3}; MaxHP:{4}; Address:0x{5:X}; GUID:0x{6}; EntryID: {7}", i.Name, i.Location,
                    (int)i.Location.Distance(localPlayer.Location), i.Health, i.HealthMax, i.Address.ToInt64(), i.GUID, i.EntryID));
            }
            progressBar1.Value++;
            Log("Players-----------------------------------------");
            foreach (WowPlayer i in wowUnits)
            {
                Log(string.Format(
                    "\t{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; Address:{4:X}; Class:{5}; Level:{6}; HP:{7}; MaxHP:{8}; TargetGUID: 0x{9}; IsAlliance:{10}; Auras: {{ {11} }}",
                    i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), i.Address.ToInt64(), i.Class, i.Level, i.Health, i.HealthMax,
                    i.TargetGUID, i.Faction, string.Join(",", i.Auras.Select(l => l.Name + "::" + l.Stack + "::" + l.TimeLeftInMs + "::" + l.OwnerGUID.ToString()))));
            }
            progressBar1.Value++;
            Log("UIFrames-----------------------------------------");
            foreach (WoWUIFrame frame in WoWUIFrame.GetAllFrames())
            {
                Log(string.Format("\tName: {0}; Visible: {1}; EditboxText: {2}", frame.GetName, frame.IsVisible, frame.EditboxText));
            }
            progressBar1.Value++;
        }

    }
}
