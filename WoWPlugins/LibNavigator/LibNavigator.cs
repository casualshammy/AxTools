using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace LibNavigator
{
    public class LibNavigator : IPlugin
    {

        #region Info

        public string Name => "LibNavigator";
        public Version Version => new Version(1, 0);
        public string Description => "Follows routes";
        public Image TrayIcon => null;
        public bool ConfigAvailable => false;

        #endregion

        #region IPlugin methods

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart()
        {
            throw new InvalidOperationException("You should not explicitly start this library");
        }

        public void OnStop()
        {
            throw new InvalidOperationException("You should not explicitly stop this library");
        }

        #endregion

        #region Methods

        public void Go(WowPoint dest, float precision)
        {
            if (isRunning)
            {
                throw new InvalidOperationException("Script is already running");
            }
            else
            {
                //unstuckDictionary = new Dictionary<DateTime, WowPoint>();
                isRunning = true;
                precision2D = precision;
                loopPath = false;
                startFromNearestPoint = false;
                counter = 0;
                actionsList = new List<DoAction> { new DoAction() { ActionType = DoActionType.Move, WowPoint = dest } };
                DoAction();
                isRunning = false;
            }
        }

        public bool LoadScriptData(string data)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                actionsList = this.LoadJSON<List<DoAction>>(data);
                loadedScriptHash = CalculateHashForScriptData(data);
                this.LogPrint($"Script is loaded, hash: {loadedScriptHash}, JSON loading time: {stopwatch.ElapsedMilliseconds}ms");
                return true;
            }
            catch (Exception ex)
            {
                this.LogPrint($"Error while loading script: {ex.Message}");
                return false;
            }
        }

        public string CalculateHashForScriptData(string data)
        {
            using (MD5 md5hash = MD5.Create())
            {
                byte[] hash = md5hash.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hash)
                {
                    stringBuilder.Append(b.ToString("X2"));
                }
                return stringBuilder.ToString();
            }
        }

        public void StartLoadedScript()
        {
            if (isRunning)
            {
                throw new InvalidOperationException("Script is already running");
            }
            else
            {
                unstuckDictionary = new Dictionary<DateTime, WowPoint>();
                isRunning = true;
                precision2D = 3f;
                loopPath = false;
                startFromNearestPoint = false;
                int tCounter = 0;
                DoActionType[] predefinedSettings = new DoActionType[] { DoActionType.SetLoopPath, DoActionType.SetStartFromNearestPoint };
                while (predefinedSettings.Contains(actionsList[tCounter].ActionType))
                {
                    switch (actionsList[tCounter].ActionType)
                    {
                        case DoActionType.SetLoopPath:
                            loopPath = bool.Parse(actionsList[tCounter].Data);
                            break;
                        case DoActionType.SetStartFromNearestPoint:
                            startFromNearestPoint = bool.Parse(actionsList[tCounter].Data);
                            break;
                    }
                    tCounter++;
                }
                this.LogPrint($"<Loop path> is set to {loopPath}");
                this.LogPrint($"<StartFromNearestPoint> is set to {startFromNearestPoint}");
                if (startFromNearestPoint)
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
        }

        public void StopLoadedScript()
        {
            timer.Dispose();
            isRunning = false;
        }

        private void DoAction()
        {
            List<WowObject> wowObjects = new List<WowObject>();
            List<WowNpc> wowNpcs = new List<WowNpc>();
            WoWPlayerMe me = ObjMgr.Pulse(wowObjects, null, wowNpcs);
            if (me != null)
            {
                UnstuckIfNeeded(me.Location, actionsList[counter].ActionType);
                switch (actionsList[counter].ActionType)
                {
                    case DoActionType.Move:
                        double distance2D = me.Location.Distance2D(actionsList[counter].WowPoint);
                        double distance3D = me.Location.Distance(actionsList[counter].WowPoint);
                        if (me.IsFlying && (distance3D > 10f || (distance3D <= 10f && GetNextAction().ActionType != DoActionType.Move && me.IsMoving)))
                        {
                            this.LogPrint(string.Format("Flying to point --> [{0}]; distance: {1}", actionsList[counter].WowPoint, distance3D));
                            MoveMgr.Move3D(actionsList[counter].WowPoint, 8f, 3f, 1000, true);
                        }
                        else if (!me.IsFlying && (distance2D > precision2D || (distance2D <= precision2D && GetNextAction().ActionType != DoActionType.Move && me.IsMoving)))
                        {
                            this.LogPrint(string.Format("Moving to point --> [{0}]; my loc: [{3}]; distance2D: {1}; speed: {2}", actionsList[counter].WowPoint, distance2D, me.Speed, me.Location));
                            MoveMgr.Move2D(actionsList[counter].WowPoint, precision2D, 1000, true, GetNextAction().ActionType == DoActionType.Move);
                        }
                        else
                        {
                            IncreaseCounterAndDoAction();
                        }
                        break;
                    case DoActionType.StopProfile:
                        if (loopPath)
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
                    case DoActionType.SetPrecision2D:
                        if (!float.TryParse(actionsList[counter].Data, out precision2D))
                        {
                            precision2D = 3f;
                        }
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.SetPrecision3D:
                        if (!float.TryParse(actionsList[counter].Data, out precision3D))
                        {
                            precision3D = 8f;
                        }
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.WaitWhile:
                        if (!GameFunctions.Lua.IsTrue(actionsList[counter].Data))
                        {
                            IncreaseCounterAndDoAction();
                        }
                        break;
                    case DoActionType.SendToChatWhile:
                        string[] p = actionsList[counter].Data.Split(new string[] { "##@##" }, StringSplitOptions.RemoveEmptyEntries);
                        string action = p[0];
                        string condition = p[1];
                        if (GameFunctions.Lua.IsTrue(condition))
                        {
                            GameFunctions.SendToChat(action);
                        }
                        else
                        {
                            IncreaseCounterAndDoAction();
                        }
                        break;
                    case DoActionType.StopProfileIf:
                        if (GameFunctions.Lua.IsTrue(actionsList[counter].Data))
                        {
                            counter = actionsList.Count - 1;
                        }
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.NotifyUser:
                        this.ShowNotify(actionsList[counter].Data, false, false);
                        IncreaseCounterAndDoAction();
                        break;
                    case DoActionType.NotifyUserIf:
                        if (GameFunctions.Lua.IsTrue(actionsList[counter].AdditionalData))
                        {
                            this.ShowNotify(actionsList[counter].Data, false, false);
                        }
                        IncreaseCounterAndDoAction();
                        break;
                    default:
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
                    if (loopPath)
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
                return loopPath ? actionsList[0] : new DoAction { ActionType = DoActionType.StopProfile };
            }
            return actionsList[counter + 1];
        }

        private void UnstuckIfNeeded(WowPoint playerLoc, DoActionType currentAction)
        {
            if (currentAction == DoActionType.Move)
            {
                unstuckDictionary.Add(DateTime.UtcNow, playerLoc);
                if (unstuckDictionary.Count >= 2)
                {
                    if (unstuckDictionary.Count > 100) // sufficiently big value (until this method is called once per second)
                    {
                        unstuckDictionary.Remove(unstuckDictionary.Keys.First());
                    }
                    KeyValuePair<DateTime, WowPoint> last = unstuckDictionary.Last();
                    KeyValuePair<DateTime, WowPoint> first = unstuckDictionary.LastOrDefault(l => (last.Key - l.Key).TotalSeconds >= 5);
                    if (!first.Equals(default(KeyValuePair<DateTime, WowPoint>)))
                    {
                        if (last.Value.Distance(first.Value) < 1f)
                        {
                            this.LogPrint($"We are stuck at {playerLoc}. Trying to unstuck...");
                            MoveMgr.Jump();
                        }
                    }
                }
            }
            else
            {
                unstuckDictionary.Clear();
            }
        }

        #endregion

        #region Fields

        private List<DoAction> actionsList;
        private string loadedScriptHash;
        private int counter;
        private bool loopPath;
        private bool startFromNearestPoint;
        private SafeTimer timer;
        private float precision2D = 3f;
        private float precision3D = 8f;
        private volatile bool isRunning = false;
        private const int RESOLUTION_INTERVAL = 50;
        private Dictionary<DateTime, WowPoint> unstuckDictionary = new Dictionary<DateTime, WowPoint>();

        #endregion

    }
}
