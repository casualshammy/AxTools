﻿using AxTools.Classes;
using AxTools.Components;
using AxTools.WoW;
using AxTools.WoW.Management;
using AxTools.WoW.Management.ObjectManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Forms
{
    internal partial class WowRadarOptions : BorderedMetroForm
    {
        private readonly Settings settings = Settings.Instance;
        private readonly int nameColumnWidth = 248;
        private readonly int nameColumnWidthExt = 265;
        private readonly int maxVisibleRows = 7;

        internal WowRadarOptions()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            foreach (ObjectToFind i in settings.WoWRadarList)
            {
                dataGridViewObjects.Rows.Add(i.Enabled, i.Name, i.Interact, i.SoundAlarm);
            }
            dataGridViewObjects.RowsAdded += DataGridViewObjectsRowsAddedOrRemove;
            dataGridViewObjects.RowsRemoved += DataGridViewObjectsRowsAddedOrRemove;
            dataGridViewObjects.CellValueChanged += DataGridViewObjectsOnCellValueChanged;
            dataGridViewObjects.CellMouseClick += DataGridViewObjectsOnCellMouseClick;
            dataGridViewObjects.CellContentClick += DataGridViewObjectsOnCellContentClick;
            dataGridViewObjects.ColumnHeaderMouseClick += DataGridViewObjectsColumnHeaderMouseClick;
            dataGridViewObjects.Columns[1].Width = dataGridViewObjects.Rows.Count > maxVisibleRows ? nameColumnWidth : nameColumnWidthExt;
            metroCheckBoxShowPlayersClasses.Checked = settings.WoWRadarShowPlayersClasses;
            metroCheckBoxShowNpcsNames.Checked = settings.WoWRadarShowNPCsNames;
            metroCheckBoxShowObjectsNames.Checked = settings.WoWRadarShowObjectsNames;
            metroStyleManager1.Style = settings.StyleColor;
            metroTabControl1.SelectedIndex = 0;
            BeginInvoke((MethodInvoker) delegate
            {
                int maxWidth = Screen.PrimaryScreen.WorkingArea.Width;
                WowRadar pRadar = Utils.FindForm<WowRadar>();
                if (pRadar != null)
                {
                    Location = maxWidth - pRadar.Location.X - pRadar.Size.Width - 20 > Size.Width
                        ? new Point(pRadar.Location.X + pRadar.Size.Width + 20, pRadar.Location.Y)
                        : new Point(pRadar.Location.X - Size.Width - 20, pRadar.Location.Y);
                }
                OnActivated(EventArgs.Empty);
            });
        }

        private void RebuildKOSList()
        {
            List<ObjectToFind> list = new List<ObjectToFind>();
            foreach (DataGridViewRow i in dataGridViewObjects.Rows)
            {
                list.Add(new ObjectToFind((bool)i.Cells[0].Value, (string)i.Cells[1].Value, (bool)i.Cells[2].Value, (bool)i.Cells[3].Value));
            }
            settings.WoWRadarList = list;
        }

        private void MetroCheckBoxShowPlayersClassesCheckedChanged(object sender, EventArgs e)
        {
            settings.WoWRadarShowPlayersClasses = metroCheckBoxShowPlayersClasses.Checked;
        }

        private void MetroCheckBoxShowNpcsNamesCheckedChanged(object sender, EventArgs e)
        {
            settings.WoWRadarShowNPCsNames = metroCheckBoxShowNpcsNames.Checked;
        }

        private void MetroCheckBoxShowObjectsNamesCheckedChanged(object sender, EventArgs e)
        {
            settings.WoWRadarShowObjectsNames = metroCheckBoxShowObjectsNames.Checked;
        }

        private void DataGridViewObjectsRowsAddedOrRemove(object sender, EventArgs e)
        {
            RebuildKOSList();
            dataGridViewObjects.Columns[1].Width = dataGridViewObjects.Rows.Count > maxVisibleRows ? nameColumnWidth : nameColumnWidthExt;
        }
        private void DataGridViewObjectsOnCellValueChanged(object sender, DataGridViewCellEventArgs dataGridViewCellEventArgs)
        {
            RebuildKOSList();
        }

        private void DataGridViewObjectsColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            RebuildKOSList();
        }

        private void DataGridViewObjectsOnCellContentClick(object sender, DataGridViewCellEventArgs dataGridViewCellEventArgs)
        {
            dataGridViewObjects.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DataGridViewObjectsOnCellMouseClick(object sender, DataGridViewCellMouseEventArgs dataGridViewCellMouseEventArgs)
        {
            if (dataGridViewCellMouseEventArgs.Button == MouseButtons.Right && dataGridViewCellMouseEventArgs.RowIndex >= 0)
            {
                dataGridViewObjects.Rows.RemoveAt(dataGridViewCellMouseEventArgs.RowIndex);
            }
        }

        private void buttonAddUnknown_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(metroTextBoxAddNew.Text))
            {
                dataGridViewObjects.Rows.Add(true, metroTextBoxAddNew.Text, true, true);
                if (dataGridViewObjects.Rows.Count > 0)
                {
                    dataGridViewObjects.FirstDisplayedScrollingRowIndex = dataGridViewObjects.RowCount - 1;
                }
            }
            else
            {
                this.ShowTaskDialog("Object name cannot be empty!", "Please enter an object name in the textbox", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void buttonAddNPC_Click(object sender, EventArgs e)
        {
            if (comboboxNPCs.SelectedIndex != -1 && comboboxNPCs.SelectedItem != null && !string.IsNullOrWhiteSpace(comboboxNPCs.SelectedItem.ToString()))
            {
                dataGridViewObjects.Rows.Add(true, comboboxNPCs.SelectedItem.ToString(),false, true);
                if (dataGridViewObjects.Rows.Count > 0)
                {
                    dataGridViewObjects.FirstDisplayedScrollingRowIndex = dataGridViewObjects.RowCount - 1;
                }
                comboboxNPCs.SelectedIndex = -1;
                comboboxNPCs.Invalidate();
            }
            else
            {
                this.ShowTaskDialog("NPC name cannot be empty!", "Please select NPC from the combobox", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void buttonAddObject_Click(object sender, EventArgs e)
        {
            if (comboboxObjects.SelectedIndex != -1 && comboboxObjects.SelectedItem != null && !string.IsNullOrWhiteSpace(comboboxObjects.SelectedItem.ToString()))
            {
                dataGridViewObjects.Rows.Add(true, comboboxObjects.SelectedItem.ToString(), true, true);
                if (dataGridViewObjects.Rows.Count > 0)
                {
                    dataGridViewObjects.FirstDisplayedScrollingRowIndex = dataGridViewObjects.RowCount - 1;
                }
                comboboxObjects.SelectedIndex = -1;
                comboboxObjects.Invalidate();
            }
            else
            {
                this.ShowTaskDialog("Object name cannot be empty!", "Please select object from the combobox", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void comboboxNPCs_Click(object sender, EventArgs e)
        {
            comboboxNPCs.Items.Clear();
            try
            {
                List<WowNpc> npcList = new List<WowNpc>();
                WoWPlayerMe localPlayer = ObjectMgr.Pulse(npcList);
                List<WowNpc> npcsWithUniqueNames = npcList.DistinctBy(i => i.Name).ToList();
                npcsWithUniqueNames.Sort(delegate(WowNpc o1, WowNpc o2)
                {
                    double distance1 = o1.Location.Distance(localPlayer.Location);
                    double distance2 = o2.Location.Distance(localPlayer.Location);
                    return distance1.CompareTo(distance2);
                });
                comboboxNPCs.Items.AddRange(npcsWithUniqueNames.Select(i => i.Name).Cast<object>().ToArray());
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [WoWRadarOptions] Error: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message), true);
            }
        }

        private void comboboxObjects_Click(object sender, EventArgs e)
        {
            comboboxObjects.Items.Clear();
            try
            {
                List<WowObject> objectList = new List<WowObject>();
                WoWPlayerMe localPlayer = ObjectMgr.Pulse(objectList);
                List<WowObject> objectsWithUniqueNames = objectList.DistinctBy(i => i.Name).ToList();
                objectsWithUniqueNames.Sort(delegate(WowObject wo1, WowObject wo2)
                {
                    double distance1 = wo1.Location.Distance(localPlayer.Location);
                    double distance2 = wo2.Location.Distance(localPlayer.Location);
                    return distance1.CompareTo(distance2);
                });
                comboboxObjects.Items.AddRange(objectsWithUniqueNames.Select(i => i.Name).Cast<object>().ToArray());
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [WoWRadarOptions] Error: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message), true);
            }
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                Utils.CheckCreateDir();
                openFileDialog.Filter = @"JSON file|*.json";
                openFileDialog.InitialDirectory = Globals.UserfilesPath;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<ObjectToFind> list = JsonConvert.DeserializeObject<List<ObjectToFind>>(File.ReadAllText(openFileDialog.FileName, Encoding.UTF8));
                    dataGridViewObjects.Rows.Clear();
                    foreach (ObjectToFind i in list)
                    {
                        dataGridViewObjects.Rows.Add(i.Enabled, i.Name, i.Interact, i.SoundAlarm);
                    }
                }
            }
            if (dataGridViewObjects.Rows.Count > 0)
            {
                dataGridViewObjects.FirstDisplayedScrollingRowIndex = dataGridViewObjects.RowCount - 1;
            }
        }

        private void buttonSaveFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                Utils.CheckCreateDir();
                saveFileDialog.Filter = @"JSON file|*.json";
                saveFileDialog.InitialDirectory = Globals.UserfilesPath;
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string json = JsonConvert.SerializeObject(settings.WoWRadarList);
                    File.WriteAllText(saveFileDialog.FileName, json, Encoding.UTF8);
                }
            }
        }
    
    }
}
