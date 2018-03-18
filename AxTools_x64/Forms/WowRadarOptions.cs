using System.Globalization;
using AxTools.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AxTools.Forms.Helpers;
using AxTools.WoW;
using AxTools.WoW.Internals;
using Components.Forms;

namespace AxTools.Forms
{
    internal partial class WowRadarOptions : BorderedMetroForm, IWoWModule
    {
        private readonly WoWRadarSettings settings = WoWRadarSettings.Instance;
        private readonly Log2 log;
        private readonly WowProcess wowProcess;

        public int ProcessID { get; set; }

        internal WowRadarOptions(WowProcess wow)
        {
            InitializeComponent();
            wowProcess = wow;
            ProcessID = wow.ProcessID;
            log = new Log2($"WowRadarOptions - {wowProcess.ProcessID}");
            ShowInTaskbar = false;
            StyleManager.Style = Settings2.Instance.StyleColor;
            metroCheckBoxShowPlayersClasses.Checked = settings.ShowPlayersClasses;
            metroCheckBoxShowNpcsNames.Checked = settings.ShowNPCsNames;
            metroCheckBoxShowObjectsNames.Checked = settings.ShowObjectsNames;
            checkBoxPlayerArrowOnTop.Checked = settings.ShowLocalPlayerRotationArrowOnTop;
            textboxAlarmSound.Text = settings.AlarmSoundFile;
            textboxAlarmSound.TextChanged += TextboxAlarmSound_TextChanged;
            metroTabControl1.SelectedIndex = 0;
            BeginInvoke((MethodInvoker) delegate
            {
                int maxWidth = Screen.PrimaryScreen.WorkingArea.Width;
                WowRadar pRadar = Utils.FindForms<WowRadar>().FirstOrDefault();
                if (pRadar != null)
                {
                    Location = maxWidth - pRadar.Location.X - pRadar.Size.Width - 20 > Size.Width
                        ? new Point(pRadar.Location.X + pRadar.Size.Width + 20, pRadar.Location.Y)
                        : new Point(pRadar.Location.X - Size.Width - 20, pRadar.Location.Y);
                }
                OnActivated(EventArgs.Empty);
            });

            oListView.SetObjects(settings.List);
            oListView.CheckObjects(settings.List.Where(l => l.Enabled));
            oListView.BooleanCheckStateGetter = OListView_BooleanCheckStateGetter;
            oListView.BooleanCheckStatePutter = OListView_BooleanCheckStatePutter;
            oListView.KeyUp += OListView_OnKeyUp;
            oColumnInteract.AspectPutter = delegate(object rowObject, object value)
            {
                if (rowObject is RadarObject radarObject)
                {
                    RefreshRadarObject(radarObject, radarObject.Name, (bool)value, radarObject.SoundAlarm);
                }
            };
            oColumnSoundAlarm.AspectPutter = delegate(object rowObject, object value)
            {
                if (rowObject is RadarObject radarObject)
                {
                    RefreshRadarObject(radarObject, radarObject.Name, radarObject.Interact, (bool)value);
                }
            };
            oColumnName.AspectPutter = delegate(object rowObject, object value)
            {
                if (rowObject is RadarObject radarObject)
                {
                    RefreshRadarObject(radarObject, (string)value, radarObject.Interact, radarObject.SoundAlarm);
                }
            };
        }

        private void TextboxAlarmSound_TextChanged(object sender, EventArgs e)
        {
            settings.AlarmSoundFile = textboxAlarmSound.Text;
        }

        private bool OListView_BooleanCheckStateGetter(object rowObject)
        {
            RadarObject radarObject = rowObject as RadarObject;
            return radarObject != null && radarObject.Enabled;
        }

        private bool OListView_BooleanCheckStatePutter(object rowObject, bool value)
        {
            if (rowObject is RadarObject radarObject)
            {
                int indexOf = settings.List.IndexOf(radarObject);
                settings.List.RemoveAt(indexOf);
                radarObject.Enabled = value;
                settings.List.Insert(indexOf, radarObject);
            }
            return value;
        }

        private void OListView_OnKeyUp(object sender, KeyEventArgs args)
        {
            if (args.KeyCode == Keys.Delete && !oListView.IsCellEditing)
            {
                if (oListView.SelectedObject is RadarObject radarObject)
                {
                    settings.List.Remove(radarObject);
                    oListView.RemoveObject(radarObject);
                }
            }
        }

        private void MetroCheckBoxShowPlayersClassesCheckedChanged(object sender, EventArgs e)
        {
            settings.ShowPlayersClasses = metroCheckBoxShowPlayersClasses.Checked;
        }

        private void MetroCheckBoxShowNpcsNamesCheckedChanged(object sender, EventArgs e)
        {
            settings.ShowNPCsNames = metroCheckBoxShowNpcsNames.Checked;
        }

        private void MetroCheckBoxShowObjectsNamesCheckedChanged(object sender, EventArgs e)
        {
            settings.ShowObjectsNames = metroCheckBoxShowObjectsNames.Checked;
        }

        private void ButtonAddUnknown_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(metroTextBoxAddNew.Text))
            {
                if (!settings.List.Any(l => l.Name == metroTextBoxAddNew.Text))
                {
                    AddRadarObject(new RadarObject(true, metroTextBoxAddNew.Text, true, true));
                }
                else
                {
                    this.TaskDialog("Object with the same name is already exist!", "", NotifyUserType.Error);
                }
            }
            else
            {
                this.TaskDialog("Object name cannot be empty!", "Please enter an object name in the textbox", NotifyUserType.Error);
            }
        }

        private void ButtonAddNPC_Click(object sender, EventArgs e)
        {
            if (comboboxNPCs.SelectedIndex != -1 && comboboxNPCs.SelectedItem != null && !string.IsNullOrWhiteSpace(comboboxNPCs.SelectedItem.ToString()))
            {
                if (!settings.List.Any(l => l.Name == comboboxNPCs.SelectedItem.ToString()))
                {
                    AddRadarObject(new RadarObject(true, comboboxNPCs.SelectedItem.ToString(), false, true));
                }
                else
                {
                    this.TaskDialog("Object with the same name is already exist!", "", NotifyUserType.Error);
                }
                comboboxNPCs.SelectedIndex = -1;
                comboboxNPCs.Invalidate();
            }
            else
            {
                this.TaskDialog("NPC name cannot be empty!", "Please select NPC from the combobox", NotifyUserType.Error);
            }
        }

        private void ButtonAddObject_Click(object sender, EventArgs e)
        {
            if (comboboxObjects.SelectedIndex != -1 && comboboxObjects.SelectedItem != null && !string.IsNullOrWhiteSpace(comboboxObjects.SelectedItem.ToString()))
            {
                if (!settings.List.Any(l => l.Name == comboboxObjects.SelectedItem.ToString()))
                {
                    AddRadarObject(new RadarObject(true, comboboxObjects.SelectedItem.ToString(), true, true));
                }
                else
                {
                    this.TaskDialog("Object with the same name is already exist!", "", NotifyUserType.Error);
                } 
                comboboxObjects.SelectedIndex = -1;
                comboboxObjects.Invalidate();
            }
            else
            {
                this.TaskDialog("Object name cannot be empty!", "Please select object from the combobox", NotifyUserType.Error);
            }
        }

        private void ComboboxNPCs_Click(object sender, EventArgs e)
        {
            comboboxNPCs.Items.Clear();
            try
            {
                List<WowNpc> npcList = new List<WowNpc>();
                WoWPlayerMe localPlayer = ObjectMgr.Pulse(wowProcess, null, null, npcList);
                if (localPlayer != null)
                {
                    List<WowNpc> npcsWithUniqueNames = npcList.DistinctBy(i => i.Name).ToList();
                    npcsWithUniqueNames.Sort(delegate (WowNpc o1, WowNpc o2)
                    {
                        double distance1 = o1.Location.Distance(localPlayer.Location);
                        double distance2 = o2.Location.Distance(localPlayer.Location);
                        return distance1.CompareTo(distance2);
                    });
                    comboboxNPCs.Items.AddRange(npcsWithUniqueNames.Select(i => i.Name).Cast<object>().ToArray());
                }
                else
                {
                    log.Info("ComboboxNPCs_Click: local player is null");
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}", ex.Message));
            }
        }

        private void ComboboxObjects_Click(object sender, EventArgs e)
        {
            comboboxObjects.Items.Clear();
            try
            {
                List<WowObject> objectList = new List<WowObject>();
                WoWPlayerMe localPlayer = ObjectMgr.Pulse(wowProcess, objectList);
                List<WowObject> objectsWithUniqueNames = objectList.DistinctBy(i => i.Name).ToList();
                objectsWithUniqueNames.Sort(delegate(WowObject wo1, WowObject wo2)
                {
                    // ReSharper disable ImpureMethodCallOnReadonlyValueField
                    double distance1 = wo1.Location.Distance(localPlayer.Location);
                    double distance2 = wo2.Location.Distance(localPlayer.Location);
                    // ReSharper restore ImpureMethodCallOnReadonlyValueField
                    return distance1.CompareTo(distance2);
                });
                comboboxObjects.Items.AddRange(objectsWithUniqueNames.Select(i => i.Name).Cast<object>().ToArray());
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}", ex.Message));
            }
        }

        private void ButtonOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = @"JSON file|*.json";
                openFileDialog.InitialDirectory = AppFolders.UserfilesDir;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string rawText = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                    ObservableCollection<RadarObject> o = JsonConvert.DeserializeObject<ObservableCollection<RadarObject>>(rawText);
                    settings.List.Clear();
                    foreach (RadarObject radarObject in o)
                    {
                        settings.List.Add(radarObject);
                    }
                    oListView.SetObjects(settings.List);
                }
                oListView.Items[oListView.Items.Count - 1].EnsureVisible();
            }
        }

        private void ButtonSaveFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = @"JSON file|*.json";
                saveFileDialog.InitialDirectory = AppFolders.UserfilesDir;
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder(1024);
                    using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
                        {
                            JsonSerializer js = new JsonSerializer();
                            jsonWriter.Formatting = Formatting.Indented;
                            jsonWriter.IndentChar = ' ';
                            jsonWriter.Indentation = 4;
                            js.Serialize(jsonWriter, settings.List);
                        }
                    }
                    string json = sb.ToString();
                    File.WriteAllText(saveFileDialog.FileName, json, Encoding.UTF8);
                }
            }
        }

        private void AddRadarObject(RadarObject radarObject)
        {
            settings.List.Add(radarObject);
            oListView.AddObject(radarObject);
            oListView.Items[oListView.Items.Count - 1].EnsureVisible();
        }

        private void RefreshRadarObject(RadarObject radarObject, string name, bool interact, bool soundAlarm)
        {
            int indexOf = settings.List.IndexOf(radarObject);
            settings.List.RemoveAt(indexOf);
            radarObject.Name = name;
            radarObject.Interact = interact;
            radarObject.SoundAlarm = soundAlarm;
            settings.List.Insert(indexOf, radarObject);
        }

        private void CheckBoxPlayerArrowOnTop_CheckedChanged(object sender, EventArgs e)
        {
            settings.ShowLocalPlayerRotationArrowOnTop = checkBoxPlayerArrowOnTop.Checked;
        }

    }
}
