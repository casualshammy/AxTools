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
    public class LibNavigator : IPlugin3
    {
        #region Info

        public string Name => "LibNavigator";
        public Version Version => new Version(1, 0);
        public string Description => "Follows routes";
        public Image TrayIcon => null;
        public bool ConfigAvailable => false;
        public string[] Dependencies => null;
        public bool DontCloseOnWowShutdown => false;

        #endregion Info

        #region IPlugin2 methods

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart(GameInterface game)
        {
            throw new InvalidOperationException("You should not explicitly start this library");
        }

        public void OnStop()
        {
            throw new InvalidOperationException("You should not explicitly stop this library");
        }

        #endregion IPlugin2 methods

        #region Methods

        public void Go(WowPoint dest, float precision, GameInterface game)
        {
            if (isRunning)
            {
                throw new InvalidOperationException("Script is already running");
            }
            else
            {
                //unstuckDictionary = new Dictionary<DateTime, WowPoint>();
                this.game = game;
                isRunning = true;
                precision2D = precision;
                precision3D = precision * 3;
                loopPath = false;
                startFromNearestPoint = false;
                counter = 0;
                actionsList = new List<DoAction> { new DoAction() { ActionType = DoActionType.Move, WowPoint = dest } };
                this.LogPrint($"Go(): destination: {dest}; precision2D: {precision2D}; precision3D: {precision3D}");
                DoAction();
                isRunning = false;
            }
        }

        public void GoWait(WowPoint dest, float precision, GameInterface game, int timeoutMs = 30000)
        {
            Stopwatch stopwatch = new Stopwatch();
            WoWPlayerMe me = game.GetGameObjects();
            while (me != null && me.Location.Distance(dest) > precision && timeoutMs > 0)
            {
                stopwatch.Restart();
                Go(dest, precision, game);
                timeoutMs -= (int)stopwatch.ElapsedMilliseconds;
            }
        }

        public void GoPath(WowPoint[] dest, float precision, GameInterface game)
        {
            if (isRunning)
            {
                throw new InvalidOperationException("Script is already running");
            }
            else
            {
                //counter = 0;
                //this.game = game;
                //isRunning = true;

                precision2D = precision;
                precision3D = precision * 3;
                loopPath = false;
                startFromNearestPoint = false;
                EndOfActionsListIsReached = false;
                actionsList = dest.Select(wowpoint => new DoAction() { ActionType = DoActionType.Move, WowPoint = wowpoint }).ToList();
                this.LogPrint($"GoPath(): total points: {actionsList.Count}; final point: {actionsList.Last().WowPoint}; precision2D: {precision2D}; precision3D: {precision3D}");
                StartLoadedScript(game);
                while (!EndOfActionsListIsReached)
                {
                    Thread.Sleep(5);
                }
                StopLoadedScript();
                //isRunning = false;
            }
        }

        public bool IsRunning => isRunning;

        public bool LoadScriptData(string data)
        {
            if (isRunning)
            {
                throw new InvalidOperationException("Cannot set script data if another script is running");
            }
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

        public void StartLoadedScript(GameInterface game)
        {
            if (isRunning)
            {
                throw new InvalidOperationException("Script is already running");
            }
            else
            {
                this.game = game;
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
                    WoWPlayerMe localPlayer = game.GetGameObjects();
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
                (timer = this.CreateTimer(RESOLUTION_INTERVAL, game, DoAction)).Start();
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
            WoWPlayerMe me = game.GetGameObjects(wowObjects, null, wowNpcs);
            if (me != null)
            {
                DoAction action = actionsList[counter];
                if (action.ActionType != DoActionType.Move)
                {
                    unstuckDictionary.Clear();
                }
                switch (action.ActionType)
                {
                    case DoActionType.Move:
                        double distance2D = me.Location.Distance2D(action.WowPoint);
                        double distance3D = me.Location.Distance(action.WowPoint);
                        if (me.IsFlying && (distance3D > precision3D || (distance3D <= precision3D && GetNextAction().ActionType != DoActionType.Move && me.IsMoving)))
                        {
                            UnstuckIfNeeded(me.Location, action.ActionType);
                            this.LogPrint(string.Format("Flying to point --> [{0}]; distance: {1}", action.WowPoint, distance3D));
                            game.Move3D(action.WowPoint, precision3D, precision3D, 1000, true, GetNextAction().ActionType == DoActionType.Move);
                        }
                        else if (!me.IsFlying && (distance2D > precision2D || (distance2D <= precision2D && GetNextAction().ActionType != DoActionType.Move && me.IsMoving)))
                        {
                            UnstuckIfNeeded(me.Location, action.ActionType);
                            this.LogPrint(string.Format("Moving to point --> [{0}]; my loc: [{3}]; distance2D: {1}; speed: {2}", action.WowPoint, distance2D, me.Speed, me.Location));
                            game.Move2D(action.WowPoint, precision2D, 1000, true, GetNextAction().ActionType == DoActionType.Move);
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
                        game.SendToChat("/run " + string.Concat(action.Data.TakeWhile(l => l != '\r' && l != '\n')));
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.SendChat:
                        Thread.Sleep(500); // player should be stopped before interact
                        game.SendToChat(action.Data);
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.SelectGossipOption:
                        Thread.Sleep(1000); // player should be stopped before interact
                        game.SelectDialogOption(action.Data);
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.Interact:
                        WowObject[] objectsWithCorrectName = wowObjects.Where(l => l.Name == action.Data).ToArray();
                        if (objectsWithCorrectName.Length > 0)
                        {
                            WowObject nearestObject = objectsWithCorrectName.Aggregate((minItem, nextItem) => me.Location.Distance(minItem.Location) < me.Location.Distance(nextItem.Location) ? minItem : nextItem);
                            Thread.Sleep(500); // player should be stopped before interact
                            nearestObject.Interact();
                        }
                        WowNpc[] npcsWithCorrectName = wowNpcs.Where(l => l.Name == action.Data).ToArray();
                        if (npcsWithCorrectName.Length > 0)
                        {
                            WowNpc nearestNpc = npcsWithCorrectName.Aggregate((minItem, nextItem) => me.Location.Distance(minItem.Location) < me.Location.Distance(nextItem.Location) ? minItem : nextItem);
                            Thread.Sleep(500); // player should be stopped before interact
                            nearestNpc.Interact();
                        }
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.Wait:
                        int timeToWait = int.Parse(action.Data);
                        while (timeToWait > 0 && timer.IsRunning)
                        {
                            Thread.Sleep(100);
                            timeToWait -= 100;
                        }
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.SetPrecision2D:
                        if (!float.TryParse(action.Data, out precision2D))
                        {
                            precision2D = 3f;
                        }
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.SetPrecision3D:
                        if (!float.TryParse(action.Data, out precision3D))
                        {
                            precision3D = 8f;
                        }
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.WaitWhile:
                        if (!game.LuaIsTrue(action.Data))
                        {
                            IncreaseCounterAndDoAction();
                        }
                        else if (!string.IsNullOrWhiteSpace(action.AdditionalData) && int.TryParse(action.AdditionalData, out int lag))
                        {
                            Thread.Sleep(lag);
                        }
                        break;

                    case DoActionType.SendToChatWhile:
                        string[] p = action.Data.Split(new string[] { "##@##" }, StringSplitOptions.RemoveEmptyEntries);
                        string _action = p[0];
                        string condition = p[1];
                        if (game.LuaIsTrue(condition))
                        {
                            game.SendToChat(_action);
                        }
                        else
                        {
                            IncreaseCounterAndDoAction();
                        }
                        break;

                    case DoActionType.StopProfileIf:
                        if (game.LuaIsTrue(action.Data))
                        {
                            counter = actionsList.Count - 1;
                        }
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.NotifyUser:
                        this.ShowNotify(action.Data, false, false);
                        IncreaseCounterAndDoAction();
                        break;

                    case DoActionType.NotifyUserIf:
                        if (game.LuaIsTrue(action.AdditionalData))
                        {
                            this.ShowNotify(action.Data, false, false);
                        }
                        IncreaseCounterAndDoAction();
                        break;

                    default:
                        IncreaseCounterAndDoAction();
                        break;
                }
            }
            else
            {
                this.LogPrint("Local player is null");
            }
        }

        private void IncreaseCounterAndDoAction()
        {
            if (timer != null && timer.IsRunning)
            {
                if (counter >= actionsList.Count - 1)
                {
                    if (loopPath)
                    {
                        counter = 0;
                        DoAction();
                    }
                    else
                    {
                        EndOfActionsListIsReached = true;
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
                            game.Jump();
                        }
                    }
                }
            }
            else
            {
                unstuckDictionary.Clear();
            }
        }

        #endregion Methods

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
        private GameInterface game;
        private bool EndOfActionsListIsReached = false;

        #endregion Fields
    }
}