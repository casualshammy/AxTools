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
            dumper.LogPrint("Dump START");
            List<WowPlayer> wowUnits = new List<WowPlayer>();
            List<WowObject> wowObjects = new List<WowObject>();
            List<WowNpc> wowNpcs = new List<WowNpc>();
            WoWPlayerMe localPlayer;
            try
            {
                localPlayer = ObjMgr.Pulse(wowObjects, wowUnits, wowNpcs);
                if (localPlayer != null)
                {
                    dumper.LogPrint("Dump OK");
                }
                else
                {
                    dumper.LogPrint("ERROR: localPlayer is null!");
                    return;
                }
            }
            catch (Exception ex)
            {
                dumper.LogPrint("ERROR(0): " + ex.Message);
                return;
            }
            dumper.LogPrint("Local player---------------------------------------");
            dumper.LogPrint(string.Format("GUID: 0x{0}; Address: 0x{1:X}; Location: {2}; ZoneID: {3}; ZoneName: {4}; IsLooting: {5}; Name: {6}", localPlayer.GUID, localPlayer.Address.ToInt64(), localPlayer.Location,
                GameFunctions.ZoneID, GameFunctions.ZoneText, GameFunctions.IsLooting, localPlayer.Name));
            dumper.LogPrint("----Local player buffs----");
            foreach (string info in localPlayer.Auras.AsParallel().Select(l => string.Format("ID: {0}; Name: {1}; Stack: {2}; TimeLeft: {3}; OwnerGUID: {4}", l.SpellId, l.Name, l.Stack, l.TimeLeftInMs, l.OwnerGUID)))
            {
                dumper.LogPrint("\t" + info);
            }
            dumper.LogPrint("----Mouseover----");
            dumper.LogPrint(string.Format("\tGUID: {0}", GameFunctions.MouseoverGUID));
            dumper.LogPrint("----Inventory slots----");
            foreach (WoWItem item in localPlayer.Inventory.AsParallel())
            {
                dumper.LogPrint(string.Format("\tID: {0}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.Enchant));
            }
            dumper.LogPrint("----Items in bags----");
            foreach (WoWItem item in localPlayer.ItemsInBags)
            {
                dumper.LogPrint(string.Format("\tID: {0}; GUID: {7}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}; BagID, SlotID: {5} {6}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.Enchant,
                    item.BagID, item.SlotID, item.GUID));
            }
            dumper.LogPrint("Objects-----------------------------------------");
            foreach (WowObject i in wowObjects)
            {
                dumper.LogPrint(string.Format("{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; OwnerGUID: 0x{4}; Address: 0x{5:X}; EntryID: {6}", i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), i.OwnerGUID,
                    i.Address.ToInt64(), i.EntryID));
            }
            dumper.LogPrint("Npcs-----------------------------------------");
            foreach (WowNpc i in wowNpcs)
            {
                dumper.LogPrint(string.Format("{0}; Location: {1}; Distance: {2}; HP:{3}; MaxHP:{4}; Address:0x{5:X}; GUID:0x{6}; EntryID: {7}", i.Name, i.Location,
                    (int)i.Location.Distance(localPlayer.Location), i.Health, i.HealthMax, i.Address.ToInt64(), i.GUID, i.EntryID));
            }
            dumper.LogPrint("Players-----------------------------------------");
            foreach (WowPlayer i in wowUnits)
            {
                dumper.LogPrint(string.Format(
                    "{0} - GUID: 0x{1}; Location: {2}; Distance: {3}; Address:{4:X}; Class:{5}; Level:{6}; HP:{7}; MaxHP:{8}; TargetGUID: 0x{9}; IsAlliance:{10}; Auras: {{ {11} }}",
                    i.Name, i.GUID, i.Location, (int)i.Location.Distance(localPlayer.Location), i.Address.ToInt64(), i.Class, i.Level, i.Health, i.HealthMax,
                    i.TargetGUID, i.Faction, string.Join(",", i.Auras.Select(l => l.Name + "::" + l.Stack + "::" + l.TimeLeftInMs + "::" + l.OwnerGUID.ToString()))));
            }
            dumper.LogPrint("UIFrames-----------------------------------------");
            foreach (WoWUIFrame frame in WoWUIFrame.GetAllFrames())
            {
                dumper.LogPrint(string.Format("\tName: {0}; Visible: {1}; Text: {2}; EditboxText: {3}", frame.GetName, frame.IsVisible, frame.GetText, frame.EditboxText));
            }
        }
    }
}
