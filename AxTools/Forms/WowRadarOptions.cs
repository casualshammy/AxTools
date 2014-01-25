using WindowsFormsAero.TaskDialog;
using AxTools.Classes;
using AxTools.Classes.WoW;
using AxTools.Components;
using MetroFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace AxTools.Forms
{
    internal partial class WowRadarOptions : BorderedMetroForm
    {
        internal WowRadarOptions()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            foreach (ObjectToFind i in WowRadar.RadarKOSGeneral)
            {
                dataGridViewObjects.Rows.Add(i.Enabled, i.Name, i.Interact, i.SoundAlarm);
            }
            dataGridViewObjects.RowsAdded += DataGridViewObjectsRowsAdded;
            dataGridViewObjects.RowsRemoved += DataGridViewObjectsRowsRemoved;
            dataGridViewObjects.CellValueChanged += DataGridViewObjectsOnCellValueChanged;
            dataGridViewObjects.CellMouseClick += DataGridViewObjectsOnCellMouseClick;
            dataGridViewObjects.CellContentClick += DataGridViewObjectsOnCellContentClick;
            dataGridViewObjects.ColumnHeaderMouseClick += DataGridViewObjectsColumnHeaderMouseClick;
            dataGridViewObjects.Columns[1].Width = dataGridViewObjects.Rows.Count > 7 ? 220 : 237;
            metroCheckBoxShowPlayersClasses.Checked = Settings.RadarShowPlayersClasses;
            metroCheckBoxShowNpcsNames.Checked = Settings.RadarShowNpcsNames;
            metroCheckBoxShowObjectsNames.Checked = Settings.RadarShowObjectsNames;
            metroStyleManager1.Style = Settings.NewStyleColor;
        }

        private readonly List<WowObject> wowObjects = new List<WowObject>();
        private readonly List<WowNpc> wowNpcs = new List<WowNpc>();

        private void RadarNpcObjectSelectionLoad(object sender, EventArgs e)
        {
            int maxWidth = Screen.PrimaryScreen.WorkingArea.Width;
            WowRadar pRadar = Utils.FindForm<WowRadar>();
            if (pRadar != null)
            {
                Location = maxWidth - pRadar.Location.X - pRadar.Size.Width - 20 > Size.Width
                               ? new Point(pRadar.Location.X + pRadar.Size.Width + 20, pRadar.Location.Y)
                               : new Point(pRadar.Location.X - Size.Width - 20, pRadar.Location.Y);
            }
        }
        
        private void ButtonAddObjectOrNpcToListClick(object sender, EventArgs e)
        {
            if (comboBoxSelectObjectOrNpc.SelectedIndex != -1 && comboBoxSelectObjectOrNpc.SelectedItem != null && !string.IsNullOrWhiteSpace(comboBoxSelectObjectOrNpc.SelectedItem.ToString()))
            {
                dataGridViewObjects.Rows.Add(true, comboBoxSelectObjectOrNpc.SelectedItem.ToString(),
                    comboBoxSelectObjectOrNpc.SelectedIndex + 1 <= wowObjects.Count, true);
                if (dataGridViewObjects.Rows.Count > 0)
                {
                    dataGridViewObjects.FirstDisplayedScrollingRowIndex = dataGridViewObjects.RowCount - 1;
                }
            }
            else
            {
                this.ShowTaskDialog("Object name cannot be empty!", "Please select an object from the combobox", TaskDialogButton.OK, TaskDialogIcon.Stop);
            }
        }

        private void MetroButtonAddNewClick(object sender, EventArgs e)
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

        private void RebuildKOSList()
        {
            List<ObjectToFind> list = new List<ObjectToFind>();
            foreach (DataGridViewRow i in dataGridViewObjects.Rows)
            {
                list.Add(new ObjectToFind((bool)i.Cells[0].Value, (string)i.Cells[1].Value, (bool)i.Cells[2].Value, (bool)i.Cells[3].Value));
            }
            WowRadar.RadarKOSGeneral = list;
        }

        private void ComboBoxSelectObjectOrNpcClick(object sender, EventArgs e)
        {
            comboBoxSelectObjectOrNpc.Items.Clear();
            try
            {
                List<WowObject> tempObjectList = new List<WowObject>();
                List<WowNpc> tempNpcList = new List<WowNpc>();
                WoW.Pulse(tempObjectList, tempNpcList);

                wowObjects.Clear();
                foreach (WowObject i in tempObjectList.Where(i => wowObjects.All(l => l.Name != i.Name)))
                {
                    wowObjects.Add(i);
                }
                wowObjects.Sort(WowObject.SortByDistance);
                foreach (WowObject i in wowObjects)
                {
                    comboBoxSelectObjectOrNpc.Items.Add(i.Name);
                }

                wowNpcs.Clear();
                foreach (WowNpc i in tempNpcList.Where(i => wowNpcs.All(l => l.Name != i.Name)))
                {
                    wowNpcs.Add(i);
                }
                wowNpcs.Sort(WowNpc.SortByDistance);
                foreach (WowNpc i in wowNpcs)
                {
                    comboBoxSelectObjectOrNpc.Items.Add(i.Name);
                }
            }
            catch (Exception ex)
            {
                Log.Print(string.Format("{0}:{1} :: [WoWRadarOptions] Error: {2}", WoW.WProc.ProcessName, WoW.WProc.ProcessID, ex.Message), true);
            }
        }

        private void MetroCheckBoxShowPlayersClassesCheckedChanged(object sender, EventArgs e)
        {
            Settings.RadarShowPlayersClasses = metroCheckBoxShowPlayersClasses.Checked;
        }

        private void MetroCheckBoxShowNpcsNamesCheckedChanged(object sender, EventArgs e)
        {
            Settings.RadarShowNpcsNames = metroCheckBoxShowNpcsNames.Checked;
        }

        private void MetroCheckBoxShowObjectsNamesCheckedChanged(object sender, EventArgs e)
        {
            Settings.RadarShowObjectsNames = metroCheckBoxShowObjectsNames.Checked;
        }

        private void DataGridViewObjectsRowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            RebuildKOSList();
            dataGridViewObjects.Columns[1].Width = dataGridViewObjects.Rows.Count > 7 ? 220 : 237;
        }
        private void DataGridViewObjectsRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            RebuildKOSList();
            dataGridViewObjects.Columns[1].Width = dataGridViewObjects.Rows.Count > 7 ? 220 : 237;
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

        private void PictureBoxSaveFileClick(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                Utils.CheckCreateDir();
                saveFileDialog.Filter = @"Text file|*.txt";
                saveFileDialog.InitialDirectory = Globals.UserfilesPath;
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        new DataContractJsonSerializer(WowRadar.RadarKOSGeneral.GetType()).WriteObject(memoryStream, WowRadar.RadarKOSGeneral);
                        File.WriteAllBytes(saveFileDialog.FileName, memoryStream.ToArray());
                    }
                }
            }
        }
        private void PictureBoxOpenFileClick(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                Utils.CheckCreateDir();
                openFileDialog.Filter = @"Text file|*.txt";
                openFileDialog.InitialDirectory = Globals.UserfilesPath;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                    if (bytes.Length > 0)
                    {
                        dataGridViewObjects.Rows.Clear();
                        using (MemoryStream memoryStream = new MemoryStream(bytes))
                        {
                            List<ObjectToFind> list = (List<ObjectToFind>) new DataContractJsonSerializer(WowRadar.RadarKOSGeneral.GetType()).ReadObject(memoryStream);
                            foreach (ObjectToFind i in list)
                            {
                                dataGridViewObjects.Rows.Add(i.Enabled, i.Name, i.Interact, i.SoundAlarm);
                            }
                        }
                    }
                }
            }
            if (dataGridViewObjects.Rows.Count > 0)
            {
                dataGridViewObjects.FirstDisplayedScrollingRowIndex = dataGridViewObjects.RowCount - 1;
            }
        }
        private void PictureBoxDeleteLastLineClick(object sender, EventArgs e)
        {
            if (dataGridViewObjects.Rows.Count > 0)
            {
                dataGridViewObjects.Rows.RemoveAt(dataGridViewObjects.Rows.Count - 1);
                if (dataGridViewObjects.Rows.Count > 0)
                {
                    dataGridViewObjects.FirstDisplayedScrollingRowIndex = dataGridViewObjects.RowCount - 1;
                }
            }
        }

    }
}
