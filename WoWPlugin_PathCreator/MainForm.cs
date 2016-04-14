using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AxTools.WoW.Internals;
using System.Runtime.Serialization;
using AxTools.WoW.PluginSystem.API;

namespace WoWPlugin_PathCreator
{
    public partial class MainForm : Form
    {
        private readonly PathCreator pathCreator;
        private readonly string profileFilepath = string.Format("{0}\\pluginsSettings\\PathCreator\\{1:yyyyMMdd_HHmmss}.json", Application.StartupPath, DateTime.UtcNow);
        private readonly List<DoAction> list = new List<DoAction>(); 

        public MainForm(PathCreator pathCreatorInstance)
        {
            InitializeComponent();
            pathCreator = pathCreatorInstance;
        }

        private void WriteJSON()
        {
            pathCreator.SaveSettingsJSON(list, profileFilepath);
        }

        private void buttonCreateWaypoint_Click(object sender, EventArgs e)
        {
            WoWPlayerMe me = ObjMgr.Pulse();
            if (me != null)
            {
                list.Add(new DoAction {ActionType = DoActionType.Move, WowPoint = me.Location});
                WriteJSON();
            }
            else
            {
                MessageBox.Show("Player isn't online");
            }
        }

        private void buttonInteract_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction {ActionType = DoActionType.Interact, Data = textBoxInteract.Text});
            WriteJSON();
        }

        private void buttonDialogOption_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction { ActionType = DoActionType.SelectGossipOption, Data = textBoxDialogOption.Text });
            WriteJSON();
        }

        private void buttonSendChat_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction {ActionType = DoActionType.SendChat, Data = textBoxSendChat.Text});
            WriteJSON();
        }

        private void buttonStopProfile_Click(object sender, EventArgs e)
        {
            list.Add(new DoAction {ActionType = DoActionType.DisableCTM});
            WriteJSON();
            Close();
        }

    }

    [DataContract]
    internal class DoAction
    {
        [DataMember(Name = "ActionType")]
        internal DoActionType ActionType;

        [DataMember(Name = "Data")]
        internal string Data;

        [DataMember(Name = "WowPoint")]
        internal WowPoint WowPoint;
    }

    internal enum DoActionType
    {
        Move,
        Interact,
        DisableCTM,
        SelectGossipOption,
        RunMacro,
        RunLua,
        SendChat
    }
}
