using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace WoWPlugin_PathCreator
{
    public partial class MainForm : Form
    {
        private readonly PathCreator pathCreator;
        private readonly string profileFilepath;
        private readonly List<DoAction> list = new List<DoAction>();
        private readonly GameInterface game;

        public MainForm(PathCreator pathCreatorInstance, GameInterface game)
        {
            this.game = game;
            InitializeComponent();
            pathCreator = pathCreatorInstance;
            profileFilepath = $"{pathCreator.GetPluginSettingsDir()}\\{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")}.json";
        }

        private void WriteJSON()
        {
            pathCreator.SaveSettingsJSON(list, profileFilepath);
        }

        private void buttonCreateWaypoint_Click(object sender, EventArgs e)
        {
            WoWPlayerMe me = game.GetGameObjects();
            if (me != null)
            {
                list.Add(new DoAction { ActionType = DoActionType.Move, WowPoint = me.Location });
                WriteJSON();
            }
            else
            {
                MessageBox.Show("Player isn't online");
            }
        }

        private void buttonInteract_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.Interact, Data = textBoxInteract.Text });
            WriteJSON();
        }

        private void buttonDialogOption_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.SelectGossipOption, Data = textBoxDialogOption.Text });
            WriteJSON();
        }

        private void buttonSendChat_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.SendChat, Data = textBoxSendChat.Text });
            WriteJSON();
        }

        private void buttonStopProfile_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.StopProfile });
            WriteJSON();
            Close();
        }

        private void buttonWait_Click(object sender, EventArgs e)
        {
            int time;
            if (int.TryParse(textBoxWait.Text, out time))
            {
                list.Add(new DoAction { ActionType = DoActionType.Wait, Data = textBoxWait.Text });
                WriteJSON();
            }
            else
            {
                MessageBox.Show("Value must be a number");
            }
        }

        private void buttonPrecision2D_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.SetPrecision2D, Data = numericPrecision2D.Value.ToString(CultureInfo.InvariantCulture) });
            WriteJSON();
        }

        private void buttonWaitWhile_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.WaitWhile, Data = textBoxWaitWhileLua.Text, AdditionalData = textBoxWaitWhileLag.Text });
            WriteJSON();
        }

        private void btnEnableLoopPath_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.SetLoopPath, Data = true.ToString() });
            WriteJSON();
        }

        private void btnStartFromNearestPoint_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.SetStartFromNearestPoint, Data = true.ToString() });
            WriteJSON();
        }
    }

    [DataContract]
    internal class DoAction
    {
        [DataMember(Name = "ActionType")]
        internal DoActionType ActionType;

        [DataMember(Name = "Data", EmitDefaultValue = false)]
        internal string Data;

        [DataMember(Name = "AdditionalData", EmitDefaultValue = false)]
        internal string AdditionalData;

        [DataMember(Name = "WowPoint", EmitDefaultValue = false)]
        internal WowPoint WowPoint;
    }

    [DataContract]
    internal class DoAction2
    {
        [DataMember(Name = "ActionType")]
        internal DoActionType ActionType;

        [DataMember(Name = "Location", EmitDefaultValue = false)]
        internal WowPoint Location;

        [DataMember(Name = "Name", EmitDefaultValue = false)]
        internal string Name;

        [DataMember(Name = "Text", EmitDefaultValue = false)]
        internal string Text;

        [DataMember(Name = "LuaCode", EmitDefaultValue = false)]
        internal string LuaCode;

        [DataMember(Name = "FloatValue", EmitDefaultValue = false)]
        internal float FloatValue;

        [DataMember(Name = "BoolValue", EmitDefaultValue = false)]
        internal bool BoolValue;

        [DataMember(Name = "TimeInMsBetweenEvaluations", EmitDefaultValue = false)]
        internal int TimeInMsBetweenEvaluations;
    }

    internal enum DoActionType
    {
        Move,
        Interact,
        SelectGossipOption,
        RunLua,
        SendChat,
        SendToChatWhile,
        StopProfile,
        StopProfileIf,
        WaitWhile,
        Wait,
        SetPrecision2D,
        SetPrecision3D,
        SetLoopPath,
        SetStartFromNearestPoint,
        NotifyUser,
        NotifyUserIf,
    }
}