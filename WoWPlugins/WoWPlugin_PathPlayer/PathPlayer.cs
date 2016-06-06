using System.Linq;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;

namespace PathPlayer
{
    internal class PathPlayer : IPlugin
	{

		#region Info

        public string Name { get { return "PathPlayer"; } }

		public Version Version { get { return new Version(1, 0); } }

		public string Description { get { return string.Format("Follows path (resolution = {0} ms)", RESOLUTION_INTERVAL); } }

	    private Image trayIcon;
        public Image TrayIcon { get { return trayIcon ?? (trayIcon = Image.FromFile(string.Format("{0}\\plugins\\{1}\\plainicon.com-50064-256px-4c1.png", Application.StartupPath, Name))); } }

		public string WowIcon { get { return string.Empty; } }

		public bool ConfigAvailable { get { return true; } }

		#endregion

        #region Methods

        public void OnConfig()
        {
            if (SettingsInstance == null)
            {
                SettingsInstance = this.LoadSettingsJSON<Settings>();
            }
            SettingsForm.Open(SettingsInstance);
            this.SaveSettingsJSON(SettingsInstance);
        }

		public void OnStart()
		{
		    SettingsInstance = this.LoadSettingsJSON<Settings>();
            actionsList.Clear();
		    try
		    {
                if (SettingsInstance.Path.Contains(".json"))
                {
                    ParseJSONFile();
                }
                else
                {
                    ParsePlainFile();
                }
		    }
		    catch (Exception ex)
		    {
		        this.ShowNotify("Can't parse profile:\r\n" + ex.Message, true, true);
                this.LogPrint("Can't parse profile: " + ex.Message);
		        return;
		    }
		    if (SettingsInstance.StartFromNearestPoint)
		    {
		        WoWPlayerMe localPlayer = ObjMgr.Pulse();
		        if (localPlayer != null)
		        {
		            DoAction nearestPoint = actionsList.Aggregate((minItem, nextItem) => localPlayer.Location.Distance(minItem.WowPoint) < localPlayer.Location.Distance(nextItem.WowPoint) ? minItem : nextItem);
                    counter = actionsList.IndexOf(nearestPoint);
		        }
		        else
		        {
		            counter = 0;
		        }
		    }
		    else
		    {
		        counter = 0;
		    }
            (timer = this.CreateTimer(RESOLUTION_INTERVAL, DoAction)).Start();
		}

        public void OnStop()
        {
            timer.Dispose();
        }

	    private void DoAction()
	    {
            List<WowObject> wowObjects = new List<WowObject>();
            List<WowNpc> wowNpcs = new List<WowNpc>();
            WoWPlayerMe me = ObjMgr.Pulse(wowObjects, wowNpcs);
            if (me != null)
            {
                switch (actionsList[counter].ActionType)
                {
                    case DoActionType.Move:
                        float tolerance2D = 3f;
                        double distance2D = me.Location.Distance2D(actionsList[counter].WowPoint);
                        double distance3D = me.Location.Distance(actionsList[counter].WowPoint);
                        if (me.IsFlying && (distance3D > 10f || (distance3D <= 10f && GetNextAction().ActionType != DoActionType.Move && me.IsMoving)))
                        {
                            this.LogPrint(string.Format("Flying to point --> [{0}]; distance: {1}", actionsList[counter].WowPoint, distance3D));
                            GameFunctions.Move3D(actionsList[counter].WowPoint, 8f, 3f, 1000, true);
                        }
                        else if (!me.IsFlying && (distance2D > tolerance2D || (distance2D <= tolerance2D && GetNextAction().ActionType != DoActionType.Move && me.IsMoving)))
                        {
                            this.LogPrint(string.Format("Moving to point --> [{0}]; my loc: [{3}]; distance2D: {1}; speed: {2}", actionsList[counter].WowPoint, distance2D, me.Speed, me.Location));
                            GameFunctions.Move2D(actionsList[counter].WowPoint, tolerance2D, 1000, true, GetNextAction().ActionType == DoActionType.Move);
                        }
                        else
                        {
                            IncreaseCounterAndDoAction();
                        }
                        break;
                    case DoActionType.DisableCTM:
                    case DoActionType.StopProfile:
                        if (SettingsInstance.LoopPath)
                        {
                            IncreaseCounterAndDoAction();
                        }
                        break;
                    case DoActionType.RunLua:
                        Thread.Sleep(500); // player should be stopped before interact
                        GameFunctions.SendToChat("/run " + string.Concat(actionsList[counter].Data.TakeWhile(l => l != '\r' && l != '\n')));
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.SendChat:
                        Thread.Sleep(500); // player should be stopped before interact
                        GameFunctions.SendToChat(actionsList[counter].Data);
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.SelectGossipOption:
                        Thread.Sleep(1000); // player should be stopped before interact
                        GameFunctions.SelectDialogOption(actionsList[counter].Data);
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.Interact:
                        WowObject[] objectsWithCorrectName = wowObjects.Where(l => l.Name == actionsList[counter].Data).ToArray();
                        if (objectsWithCorrectName.Length > 0)
                        {
                            WowObject nearestObject = objectsWithCorrectName.Aggregate((minItem, nextItem) => me.Location.Distance(minItem.Location) < me.Location.Distance(nextItem.Location) ? minItem : nextItem);
                            Thread.Sleep(500); // player should be stopped before interact
                            nearestObject.Interact();
                        }
                        WowNpc[] npcsWithCorrectName = wowNpcs.Where(l => l.Name == actionsList[counter].Data).ToArray();
                        if (npcsWithCorrectName.Length > 0)
                        {
                            WowNpc nearestNpc = npcsWithCorrectName.Aggregate((minItem, nextItem) => me.Location.Distance(minItem.Location) < me.Location.Distance(nextItem.Location) ? minItem : nextItem);
                            Thread.Sleep(500); // player should be stopped before interact
                            nearestNpc.Interact();
                        }
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.Wait:
                        int timeToWait = int.Parse(actionsList[counter].Data);
                        while (timeToWait > 0 && timer.IsRunning)
                        {
                            Thread.Sleep(100);
                            timeToWait -= 100;
                        }
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.SetClicker:
                        Utilities.ClickerEnabled = bool.Parse(actionsList[counter].Data);
                        IncreaseCounterAndDoAction();
                        break;
                }
            }
	    }

	    private void IncreaseCounterAndDoAction()
	    {
	        if (timer.IsRunning)
	        {
                if (counter >= actionsList.Count - 1)
                {
                    if (SettingsInstance.LoopPath)
                    {
                        counter = 0;
                        DoAction();
                    }
                }
                else
                {
                    counter++;
                    DoAction();
                }
	        }
	    }

        private DoAction GetNextAction()
        {
            if (counter >= actionsList.Count - 1)
            {
                return SettingsInstance.LoopPath ? actionsList[0] : new DoAction {ActionType = DoActionType.StopProfile};
            }
            return actionsList[counter + 1];
        }

        private void ParsePlainFile()
	    {
            string[] pathFile = File.ReadAllLines(SettingsInstance.Path);
            foreach (string s in pathFile)
            {
                string[] ords = s.Split(',');
                if (ords.Length == 3)
                {
                    float x = float.Parse(ords[0], CultureInfo.InvariantCulture);
                    float y = float.Parse(ords[1], CultureInfo.InvariantCulture);
                    float z = float.Parse(ords[2], CultureInfo.InvariantCulture);
                    actionsList.Add(new DoAction {ActionType = DoActionType.Move, Data = null, WowPoint = new WowPoint(x, y, z)});
                }
            }
            //todo
	        string jsonPath = SettingsInstance.Path.Replace(".txt", ".json");
            if (!File.Exists(jsonPath))
	        {
                this.SaveSettingsJSON(actionsList, jsonPath);
	        }
	    }

	    private void ParseJSONFile()
	    {
            Stopwatch stopwatch = Stopwatch.StartNew();
	        actionsList = this.LoadSettingsJSON<List<DoAction>>(SettingsInstance.Path);
	        this.LogPrint(string.Format("JSON loading time: {0}ms", stopwatch.ElapsedMilliseconds));
	    }

        #endregion

        private int counter;
        private List<DoAction> actionsList = new List<DoAction>();
        internal Settings SettingsInstance;
	    private SafeTimer timer;
	    private const int RESOLUTION_INTERVAL = 50;

	}
}
