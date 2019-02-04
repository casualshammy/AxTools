using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoWPlugin_Dumper
{
    public partial class DumperForm : Form
    {
        private readonly Dumper dumper;
        private SafeTimer chatTimer;
        private readonly GameInterface game;

        public DumperForm(Dumper dumperInstance, GameInterface game)
        {
            this.game = game;
            InitializeComponent();
            dumper = dumperInstance;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            var origText = ((Button)sender).Text;
            ((Button)sender).Text = "Please wait";
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
            var wowUnits = new List<WowPlayer>();
            var wowObjects = new List<WowObject>();
            var wowNpcs = new List<WowNpc>();
            WoWPlayerMe localPlayer;
            try
            {
                localPlayer = game.GetGameObjects(wowObjects, wowUnits, wowNpcs);
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
                Log(
                    $"\tGUID: 0x{localPlayer.GUID}; Address: 0x{localPlayer.Address.ToInt64():X}; Location: {localPlayer.Location}; ZoneID: {game.ZoneID}; ZoneName: {game.ZoneText}; IsLooting: {game.IsLooting}; Name: {localPlayer.Name}; TargetGUID: {localPlayer.TargetGUID}; Class: {localPlayer.Class}; Health/MaxHealth: {localPlayer.Health}/{localPlayer.HealthMax}, Level: {localPlayer.Level}; Faction: {localPlayer.Faction}; IsMounted: {localPlayer.IsMounted}");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("----Local player buffs----");
                foreach (string info in localPlayer.Auras.Select(l => $"ID: {l.SpellId}; Name: {l.Name}; Stack: {l.Stack}; TimeLeft: {l.TimeLeftInMs}; OwnerGUID: {l.OwnerGUID}"))
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
                Log($"\tGUID: {game.MouseoverGUID}");
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
                    Log($"\t{i.Name} - GUID: 0x{i.GUID}; Location: {i.Location}; Distance: {(int) i.Location.Distance(localPlayer.Location)}; OwnerGUID: 0x{i.OwnerGUID}; Address: 0x{i.Address.ToInt64():X}; EntryID: {i.EntryID}");
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
                    Log($"\t{i.Name}; EntryID: {i.EntryID}; Location: {i.Location}");
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
                    Log(
                        $"\t{i.Name} - GUID: 0x{i.GUID}; Location: {i.Location}; Distance: {(int) i.Location.Distance(localPlayer.Location)}; Address:0x{i.Address.ToInt64():X}; Class:{i.Class}; Level:{i.Level}; HP:{i.Health}; MaxHP:{i.HealthMax}; TargetGUID: 0x{i.TargetGUID}; IsAlliance:{i.Faction}; Auras: {{ {string.Join(",", i.Auras.Select(l => l.Name + "::" + l.Stack + "::" + l.TimeLeftInMs + "::" + l.OwnerGUID.ToString()))} }}");
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

        private void buttonDumpobjects_Click(object sender, EventArgs e)
        {
            Log("Dump START");
            var wowUnits = new List<WowPlayer>();
            var wowObjects = new List<WowObject>();
            var wowNpcs = new List<WowNpc>();
            try
            {
                game.GetGameObjects(wowObjects, wowUnits, wowNpcs);
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
                    Log($"\t{i.Name} - GUID: 0x{i.GUID}; Location: {i.Location}; Distance: {"n/a"}; OwnerGUID: 0x{i.OwnerGUID}; Address: 0x{i.Address.ToInt64():X}; EntryID: {i.EntryID}; IsBobbing: {i.Bobbing}");
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
                    Log($"\t{i.Name}; Location: {i.Location}; Distance: {"n/a"}; Address: 0x{i.Address.ToInt64().ToString("X")} HP:{i.Health}; MaxHP:{i.HealthMax}; GUID:0x{i.GUID}; GameGUID: {i.GetGameGUID()} EntryID: {i.EntryID}; IsLootable: {i.IsLootable_Lua()}");
                    foreach (WoWAura aura in i.Auras)
                    {
                        Log($"\t\t\t{aura.Name}; {aura.OwnerGUID}");
                    }
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
                        $"TargetGUID: 0x{i.TargetGUID}; Faction:{i.Faction}; Race: {i.Race}; Auras: {{ {""} }}; GUIDBytes: {BitConverter.ToString(i.GetGUIDBytes())}");
                    foreach (WoWAura aura in i.Auras)
                    {
                        Log($"\t\t\tName: {aura.Name}; OwnerGUID: {aura.OwnerGUID}; Stack: {aura.Stack}; TimeLeftInMs: {aura.TimeLeftInMs}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
            }
            MessageBox.Show("Completed");
        }

        private async void ButtonUIFrames_Click(object sender, EventArgs e)
        {
            try
            {
                Log("UIFrames-----------------------------------------");
                await Task.Run(() => Log("Player's health: " + game.LuaGetValue("UnitHealth(\"player\")")));
                if (await Task.Run(() => game.LuaIsTrue("1==1")))
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

        private void ButtonChat_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Chat messages-----------------------------------------");
                if (chatTimer == null)
                {
                    (chatTimer = dumper.CreateTimer(1000, game, delegate
                    {
                        foreach (ChatMsg msg in game.ReadChat())
                        {
                            //Log(string.Format("\tType: {0}; Channel: {1}; Sender: {2}; SenderGUID: {3}; Text: {4}", msg.Type, msg.Channel, msg.Sender, msg.SenderGUID, msg.Text));
                            textBox1.AppendText($"\tTimestamp: {msg.GetTimestampAsDateTime()}; Type: {msg.Type}; Channel: {msg.Channel}; Sender: {msg.Sender}; SenderGUID: {msg.SenderGUID}; Text: {msg.Text}\r\n");
                        }
                    })).Start();
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
                me = game.GetGameObjects();
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
                    Log($"\t{counter}:");
                    Log($"\t\tAddress: 0x{item.Address.ToInt64().ToString("X")}");
                    Log($"\t\tEntryID: {item.EntryID}");
                    Log($"\t\tName: {item.Name}");
                    Log($"\t\tStackSize: {item.StackSize}");
                    Log($"\t\tContainedIn: {item.ContainedIn}");
                    Log($"\t\tWeaponEnchant: {item.WeaponEnchant}");
                    Log($"\t\tSlot: {counter}");
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
                    Log(string.Format("\tID: {0}; GUID: {7}; Name: {1}; StackCount: {2}; Contained in: {3}; Enchant: {4}; BagID, SlotID: {5} {6}", item.EntryID, item.Name, item.StackSize, item.ContainedIn, item.WeaponEnchant,
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
            chatTimer?.Dispose();
        }

        private void buttonDumpPlayer_Click(object sender, EventArgs e)
        {
            WoWPlayerMe lp;
            try
            {
                lp = game.GetGameObjects();
                if (lp != null)
                {
                    Log("Dump OK");
                }
                else
                {
                    Log("ERROR: localPlayer is null!");
                    MessageBox.Show("ERROR: localPlayer is null!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log("ERROR(0): " + ex.Message);
                MessageBox.Show("ERROR(0): \r\n" + ex.Message);
                return;
            }
            try
            {
                Log("Info---------------------------------------");
                Log($"\tZoneID: {game.ZoneID}");
                Log($"\tZoneText: {game.ZoneText}");
                Log($"\tIsLooting: {game.IsLooting}");
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
                Log($"\tGUID: {lp.GUID}; Address: 0x{lp.Address.ToInt64().ToString("X")}; GUID bytes: {BitConverter.ToString(lp.GetGUIDBytes())}");
                Log($"\tLocation: {lp.Location}; Pitch: {lp.Pitch}; Speed: {lp.Speed}; IsMounted: {lp.IsMounted}; IsFlying: {lp.IsFlying}");
                Log($"\tName: {lp.Name}; TargetGUID: {lp.TargetGUID}; Class: {lp.Class}; Health/MaxHealth: {lp.Health}/{lp.HealthMax}; Power/MaxPower: {lp.Power}/{lp.PowerMax}");
                Log($"\tLevel: {lp.Level}; Faction: {lp.Faction}; Race: {lp.Race}; InCombat: {lp.InCombat}");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            try
            {
                Log("----Local player buffs----");
                foreach (string info in lp.Auras.Select(l => $"ID: {l.SpellId}; Name: {l.Name}; Stack: {l.Stack}; TimeLeft: {l.TimeLeftInMs}; OwnerGUID: {l.OwnerGUID}"))
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
                Log($"\tGUID: {game.MouseoverGUID}");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            MessageBox.Show("Completed");
        }

        private void btnSetPitch_Click(object sender, EventArgs e)
        {
            WoWPlayerMe lp;
            try
            {
                lp = game.GetGameObjects();
                if (lp != null)
                {
                    Log("Dump OK");
                    Log($"Old pitch: {lp.Pitch}");
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
            lp.Pitch = (float)((new Random().NextDouble()) / 10);
            Log($"New pitch: {lp.Pitch}");
        }

        private void btnTestVertAngle_Click(object sender, EventArgs e)
        {
            WoWPlayerMe lp;
            var npcs = new List<WowNpc>();
            try
            {
                lp = game.GetGameObjects(null, null, npcs);
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
            var target = npcs.FirstOrDefault(l => l.GUID == lp.TargetGUID);
            float angle = -(float)Math.Round(Math.Atan2(lp.Location.Distance2D(target.Location), target.Location.Z - lp.Location.Z) - Math.PI / 2, 2);
            //if (angle < 0)
            //    angle += (float)(Math.PI * 2);
            MessageBox.Show(angle.ToString());
        }
    }
}