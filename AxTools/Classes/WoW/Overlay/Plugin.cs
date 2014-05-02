//!CompilerOption:AddRef:SlimDx.dll

using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Bots.BGBuddy;
using Bots.DungeonBuddy;
using Bots.DungeonBuddy.Profiles;
using Bots.Quest;
using SlimDX.Direct3D9;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.AreaManagement;
using Styx.CommonBot.Profiles;
using Styx.Localization;
using Styx.Pathing;
using Styx.Plugins;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Vector3 = SlimDX.Vector3;
using AeroOverlayHonorbuddy.Overlay;
using AvoidanceManager = Bots.DungeonBuddy.Avoidance.AvoidanceManager;
using Profile = Styx.CommonBot.Profiles.Profile;
using ProfileManager = Styx.CommonBot.Profiles.ProfileManager;

namespace AeroOverlayHonorbuddy
{
    public class Plugin : HBPlugin
    {
        private Thread _th;
        private RenderForm _renderForm;

        private void DoInit()
        {
            IntPtr hwnd = StyxWoW.Memory.Process.MainWindowHandle;

            _renderForm = new RenderForm(hwnd);
            OverlayManager.SetCameraProvider(new WoWCameraProvider());
            OverlayManager.OnDrawing += OnDrawing;

            Application.Run(_renderForm);
        }

        public override void Pulse()
        {
        }

        public override void OnEnable()
        {
            if (_th == null)
            {
                _th = new Thread(DoInit)
                {
                    IsBackground = true,
                    Name = "GameOverlayInitThread"
                };
                _th.Start();
            }
        }

        public override void OnDisable()
        {
            if (_renderForm != null)
            {
                OverlayManager.OnDrawing -= OnDrawing;
                _renderForm.Close();
            }
        }

        public override string Name { get { return "Honorbuddy Aero Overlay"; } }

        public override string Author { get { return "Nesox"; } }

        public override Version Version { get { return new Version(1, 0, 9, 26); } }

        public override string ButtonText { get { return "Settings"; } }

        public override bool WantButton { get { return true; } }

        private SettingsForm _settingsForm;
        public override void OnButtonPress()
        {
            if (_settingsForm == null)
                _settingsForm = new SettingsForm();

            _settingsForm.ShowDialog();
        }

        private void OnDrawing(Device device)
        {
            try
            {
                OverlaySettings settings = OverlaySettings.Instance;

                if (settings.OnlyDrawInForeground &&
                    Imports.GetForegroundWindow() != StyxWoW.Memory.Process.MainWindowHandle)
                    return;

                if (!StyxWoW.IsInGame)
                {
                    DrawHelper.DrawShadowedText("Not in game!",
                        settings.GameStatsPositionX,
                        settings.GameStatsPositionY,
                        settings.GameStatsForegroundColor,
                        settings.GameStatsShadowColor, 
                        settings.GameStatsFontSize
                        );
                    return;
                }

                if (!TreeRoot.IsRunning)
                    ObjectManager.Update();

                WoWPoint mypos = StyxWoW.Me.Location;
                Vector3 vecStart = new Vector3(mypos.X, mypos.Y, mypos.Z);
                int myLevel = StyxWoW.Me.Level;

                if (settings.DrawGameStats)
                {
                    StringBuilder sb = new StringBuilder();

                    WoWUnit currentTarget = StyxWoW.Me.CurrentTarget;

                    if (currentTarget != null)
                    {
                        sb.AppendLine("Current Target: " + currentTarget.Name + ", Distance: " + Math.Round(currentTarget.Distance, 3));

                        WoWPoint end = currentTarget.Location;
                        Vector3 vecEnd = new Vector3(end.X, end.Y, end.Z);

                        DrawHelper.DrawLine(vecStart, vecEnd, 2f, Color.FromArgb(150, Color.Black));
                    }

                    sb.AppendLine("My Position: " + StyxWoW.Me.Location);
                    sb.AppendLine("");
                    sb.AppendLine(string.Format(Globalization.XP_HR___0_, GameStats.XPPerHour.ToString("F0")));
                    sb.AppendLine(string.Format(Globalization.Kills___0____1__hr_, GameStats.MobsKilled, GameStats.MobsPerHour.ToString("F0")));
                    sb.AppendLine(string.Format(Globalization.Deaths___0____1__hr_, GameStats.Deaths, GameStats.DeathsPerHour.ToString("F0")));
                    sb.AppendLine(string.Format(Globalization.Loots___0____1__hr_, GameStats.Loots, GameStats.LootsPerHour.ToString("F0")));
                  
                    if (BotManager.Current is BGBuddy)
                    {
                        sb.AppendLine(string.Format("Honor Gained: {0} ({1}/hr)", GameStats.HonorGained, GameStats.HonorPerHour.ToString("F0")));
                        sb.AppendLine(string.Format("BGs Won: {0} Lost: {1} Total: {2} ({3}/hr)", GameStats.BGsWon, GameStats.BGsLost, GameStats.BGsCompleted, GameStats.BGsPerHour.ToString("F0")));
                    }

                    if (myLevel < 90)
                        sb.AppendLine(string.Format("Time to Level: {0}", GameStats.TimeToLevel));

                    sb.AppendLine(string.Format("TPS: {0}", GameStats.TicksPerSecond.ToString("F2")));

                    sb.AppendLine();
                    if (!string.IsNullOrEmpty(TreeRoot.GoalText))
                        sb.AppendLine(string.Format(Globalization.Goal___0_, TreeRoot.GoalText));

                    if (settings.UseShadowedText)
                    {
                        DrawHelper.DrawShadowedText(sb.ToString(),
                            settings.GameStatsPositionX,
                            settings.GameStatsPositionY,
                            settings.GameStatsForegroundColor,
                            settings.GameStatsShadowColor,
                            settings.GameStatsFontSize
                            );
                    }
                    else
                    {
                        DrawHelper.DrawText(sb.ToString(),
                            settings.GameStatsPositionX,
                            settings.GameStatsPositionY,
                            settings.GameStatsForegroundColor,
                            settings.GameStatsFontSize
                            );
                    }
                }

                if (settings.DrawHostilityBoxes || settings.DrawUnitLines || settings.DrawGameObjectBoxes || settings.DrawGameObjectLines)
                {
                    foreach (WoWObject obj in ObjectManager.GetObjectsOfType<WoWObject>(true))
                    {
                        string name = obj.Name;
                        WoWPoint objLoc = obj.Location;
                        Vector3 vecCenter = new Vector3(objLoc.X, objLoc.Y, objLoc.Z);

                        WoWGameObject gobject = obj as WoWGameObject;
                        if (gobject != null)
                        {
                            Color color = Color.FromArgb(150, Color.Blue);

                            if (gobject.IsMineral)
                                color = Color.FromArgb(150, Color.DarkGray);
                            if (gobject.IsHerb)
                                color = Color.FromArgb(150, Color.Fuchsia);

                            if (settings.DrawGameObjectNames)
                                DrawHelper.Draw3DText(name, vecCenter);

                            if (settings.DrawGameObjectBoxes)
                            {
                                DrawHelper.DrawOutlinedBox(vecCenter,
                                    2.0f,
                                    2.0f,
                                    2.0f,
                                    Color.FromArgb(150, color)
                                    );
                            }
                            if (settings.DrawGameObjectLines)
                            {
                                bool inLos = gobject.InLineOfSight;

                                if (settings.DrawGameObjectLinesLos && inLos)
                                    DrawHelper.DrawLine(vecStart, vecCenter, 2f, Color.FromArgb(150, color));
                                else
                                {
                                    if (!settings.DrawGameObjectLinesLos)
                                        DrawHelper.DrawLine(vecStart, vecCenter, 2f, Color.FromArgb(150, color));
                                }
                            }
                        }

                        WoWPlayer player = obj as WoWPlayer;
                        if (player != null)
                        {
                            if (OverlaySettings.Instance.DrawPlayerNames)
                                DrawHelper.Draw3DText(name, vecCenter);
                        }

                        WoWUnit u = obj as WoWUnit;
                        if (u != null)
                        {
                            Color hostilityColor = Color.FromArgb(150, Color.Green);

                            if (u.IsHostile)
                            {
                                hostilityColor = Color.FromArgb(150, Color.Red);

                                if (settings.DrawAggroRangeCircles)
                                    DrawHelper.DrawCircle(vecCenter, u.MyAggroRange, 16, Color.FromArgb(75, Color.DeepSkyBlue));
                            }

                            if (u.IsNeutral)
                                hostilityColor = Color.FromArgb(150, Color.Yellow);

                            if (u.IsFriendly)
                                hostilityColor = Color.FromArgb(150, Color.Green);

                            if (settings.DrawHostilityBoxes)
                            {
                                float boundingHeight = u.BoundingHeight;
                                float boundingRadius = u.BoundingRadius;

                                DrawHelper.DrawOutlinedBox(vecCenter,
                                    boundingRadius,
                                    boundingRadius,
                                    boundingHeight,
                                    hostilityColor
                                    );

                                //DrawHelper.DrawSphere(vecCenter, 1f, 5, 5, hostilityColor);
                            }

                            if (OverlaySettings.Instance.DrawUnitNames)
                                DrawHelper.Draw3DText(name, vecCenter);

                            if (settings.DrawUnitLines)
                            {
                                vecCenter.Z += u.BoundingHeight / 2;
                                bool inLos = u.InLineOfSight;

                                if (settings.DrawUnitLinesLos && inLos)
                                    DrawHelper.DrawLine(vecStart, vecCenter, 2f, hostilityColor);
                                else
                                {
                                    if (!settings.DrawUnitLinesLos)
                                        DrawHelper.DrawLine(vecStart, vecCenter, 2f, hostilityColor);
                                }

                            }
                        }
                    }   
                }

                if (settings.DrawCurrentPath)
                {
                    MeshNavigator navigator = Navigator.NavigationProvider as MeshNavigator;
                    if (navigator != null && navigator.CurrentMovePath != null)
                    {
                        Tripper.Tools.Math.Vector3[] points = navigator.CurrentMovePath.Path.Points;
                        for (int i = 0; i < points.Length; i++)
                        {
                            Vector3 vecEnd = new Vector3(points[i].X, points[i].Y, points[i].Z);

                            if (i - 1 >= 0)
                            {
                                Tripper.Tools.Math.Vector3 prevPoint = points[i - 1];
                                Vector3 vecPreviousPoint = new Vector3(prevPoint.X, prevPoint.Y, prevPoint.Z);
                                DrawHelper.DrawLine(vecPreviousPoint, vecEnd, 2f, Color.FromArgb(150, Color.Black));
                            }

                            DrawHelper.DrawBox(vecEnd, 1.0f, 1.0f, 1.0f, Color.FromArgb(150, Color.BlueViolet));
                        }
                    }
                }

                if (settings.DrawBgMapboxes && BGBuddy.Instance != null)
                {
                    Battleground curBg = BGBuddy.Instance.Battlegrounds.FirstOrDefault(bg => bg.MapId == StyxWoW.Me.MapId);
                    if (curBg != null)
                    {
                        BgBotProfile curBgProfile = curBg.Profile;
                        if (curBgProfile != null)
                        {
                            foreach (var box in curBgProfile.Boxes[curBg.Side])
                            {
                                float width = box.BottomRight.X - box.TopLeft.X;
                                float height = box.BottomRight.Z - box.TopLeft.Z;
                                var c = box.Center;

                                Vector3 vecCenter = new Vector3(c.X, c.Y, c.Z);
                                DrawHelper.DrawOutlinedBox(vecCenter, width, width, height, Color.FromArgb(150, Color.Gold));
                            }

                            foreach (Blackspot bs in curBgProfile.Blackspots)
                            {
                                var p = bs.Location;
                                Vector3 vec = new Vector3(p.X, p.Y, p.X);
                                DrawHelper.DrawCircle(vec, bs.Radius, 32, Color.FromArgb(200, Color.Black));

                                if (!string.IsNullOrWhiteSpace(bs.Name))
                                    DrawHelper.Draw3DText("Blackspot: " + bs.Name, vec);
                                else
                                {
                                    DrawHelper.Draw3DText("Blackspot", vec);
                                }
                            }
                        }
                    }
                }

                Profile curProfile = ProfileManager.CurrentProfile;
                if (curProfile != null)
                {
                    if (settings.DrawHotspots)
                    {
                        GrindArea ga = QuestState.Instance.CurrentGrindArea;
                        if (ga != null)
                        {
                            if (ga.Hotspots != null)
                            {
                                foreach (Hotspot hs in ga.Hotspots)
                                {
                                    var p = hs.Position;
                                    Vector3 vec = new Vector3(p.X, p.Y, p.Z);
                                    DrawHelper.DrawCircle(vec, 10.0f, 32, Color.FromArgb(200, Color.Red));

                                    if (!string.IsNullOrWhiteSpace(hs.Name))
                                        DrawHelper.Draw3DText("Hotspot: " + hs.Name, vec);
                                    else
                                    {
                                        DrawHelper.Draw3DText("Hotspot", vec);
                                    }
                                }
                            }
                        }

                        // This is only used by grind profiles.
                        if (curProfile.HotspotManager != null)
                        {
                            foreach (WoWPoint p in curProfile.HotspotManager.Hotspots)
                            {
                                Vector3 vec = new Vector3(p.X, p.Y, p.Z);
                                DrawHelper.DrawCircle(vec, 10.0f, 32, Color.FromArgb(200, Color.Red));
                                DrawHelper.Draw3DText("Hotspot", vec);
                            }
                        }
                    }

                    if (settings.DrawBlackspots)
                    {
                        if (curProfile.Blackspots != null)
                        {
                            foreach (Blackspot bs in curProfile.Blackspots)
                            {
                                var p = bs.Location;
                                Vector3 vec = new Vector3(p.X, p.Y, p.Z);
                                DrawHelper.DrawCircle(vec, bs.Radius, 32, Color.FromArgb(200, Color.Black));

                                if (!string.IsNullOrWhiteSpace(bs.Name))
                                    DrawHelper.Draw3DText("Blackspot: " + bs.Name, vec);
                                else
                                {
                                    DrawHelper.Draw3DText("Blackspot: " + vec, vec);
                                }
                            }
                        }
                    }
                }

                if (settings.DrawDbAvoidObjects)
                {
                    foreach (var avoid in AvoidanceManager.Avoids)
                    {
                        var c = avoid.Location;
                        var center = new Vector3(c.X, c.Y, c.Z);

                        DrawHelper.DrawCircle(center, avoid.Radius, 32, Color.FromArgb(200, Color.LightBlue));

                        center.Z += 1.0f;
                        DrawHelper.Draw3DText("Db Avoid Object", center);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
