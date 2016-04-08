using AxTools.Helpers;
using AxTools.Properties;
using AxTools.WoW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.Forms.Helpers;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem.API;
using Settings = AxTools.Helpers.Settings;

namespace AxTools.Forms
{
    internal partial class WowRadar : Form, IWoWModule
    {

        #region Fields

        private bool processMouseWheelEvents;

        private static readonly HashSet<string> RadarKOSFind = new HashSet<string>();
        private static readonly HashSet<string> RadarKOSFindAlarm = new HashSet<string>();
        private static readonly HashSet<string> RadarKOSFindInteract = new HashSet<string>();

        private readonly Settings settings = Settings.Instance;
        private readonly Pen friendPen = new Pen(Settings.Instance.WoWRadarFriendColor, 1f);
        private readonly Pen enemyPen = new Pen(Settings.Instance.WoWRadarEnemyColor, 1f);
        private readonly Pen npcPen = new Pen(Settings.Instance.WoWRadarNPCColor, 1f);
        private readonly Pen objectPen = new Pen(Settings.Instance.WoWRadarObjectColor, 1f);
        private readonly Pen whitePen = new Pen(Color.White, 1f);
        private readonly Pen grayPen = new Pen(Color.Gray, 1f);
        private readonly SolidBrush friendBrush = new SolidBrush(Settings.Instance.WoWRadarFriendColor);
        private readonly SolidBrush enemyBrush = new SolidBrush(Settings.Instance.WoWRadarEnemyColor);
        private readonly SolidBrush npcBrush = new SolidBrush(Settings.Instance.WoWRadarNPCColor);
        private readonly SolidBrush objectBrush = new SolidBrush(Settings.Instance.WoWRadarObjectColor);
        private readonly SolidBrush whiteBrush = new SolidBrush(Color.White);
        private readonly SolidBrush grayBrush = new SolidBrush(Color.Gray);
        private Point tmpPoint = Point.Empty;
        private Point oldPoint = Point.Empty;
        private bool isDragging;
        private float zoomR = 0.5F;
        private readonly int halfOfPictureboxSize;
        private readonly List<WowObject> wowObjects = new List<WowObject>();
        private readonly List<WowPlayer> wowPlayers = new List<WowPlayer>();
        private readonly List<WowNpc> wowNpcs = new List<WowNpc>();
        private readonly Dictionary<WoWGUID, Point> objectsPointsInRadarCoords = new Dictionary<WoWGUID, Point>();

        private WoWPlayerMe localPlayer;
        private WowPlayer[] friends;
        private WowPlayer[] enemies;
        private WowObject[] objects;
        private WowNpc[] npcs;
        private readonly Thread thread;
        private volatile bool isRunning;
        private volatile bool shouldDrawObjects;
        private bool flicker;

        #endregion

        public WowRadar()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            settings.WoWRadarListChanged += WoWRadarListChanged;
            UpdateCaches();
            checkBoxFriends.ForeColor = settings.WoWRadarFriendColor;
            checkBoxEnemies.ForeColor = settings.WoWRadarEnemyColor;
            checkBoxNpcs.ForeColor = settings.WoWRadarNPCColor;
            checkBoxObjects.ForeColor = settings.WoWRadarObjectColor;
            halfOfPictureboxSize = pictureBoxMain.Width / 2;
            try
            {
                byte[] p = BitConverter.GetBytes(settings.WoWRadarShowMode);
                checkBoxFriends.Checked = p[0] == 1;
                checkBoxEnemies.Checked = p[1] == 1;
                checkBoxNpcs.Checked = p[2] == 1;
                checkBoxObjects.Checked = p[3] == 1;
                checkBoxCorpses.Checked = p[4] == 1;
                zoomR = p[5] * 0.25F;
                if (zoomR > 2F || zoomR < 0.25F)
                {
                    zoomR = 0.5F;
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("{0}:{1} :: [Radar] Can't load radar settings: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message));
            }

            checkBoxFriends.CheckedChanged += SaveCheckBoxes;
            checkBoxEnemies.CheckedChanged += SaveCheckBoxes;
            checkBoxNpcs.CheckedChanged += SaveCheckBoxes;
            checkBoxObjects.CheckedChanged += SaveCheckBoxes;
            checkBoxCorpses.CheckedChanged += SaveCheckBoxes;

            thread = new Thread(Redraw);
            MouseWheel += OnMouseWheel;
            pictureBoxMain.MouseWheel += OnMouseWheel;

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                BeginInvoke((MethodInvoker) (() => { labelHint.Visible = false; }));
            });

            Log.Info(string.Format("{0} [Radar] Loaded", WoWManager.WoWProcess));
        }

        private void Redraw()
        {
            Action refreshRadar = pictureBoxMain.Invalidate;
            bool soundAlarmPrevState = false;
            Stopwatch stopwatch = new Stopwatch();
            while (isRunning)
            {
                stopwatch.Restart();
                if (!WoWManager.Hooked || !GameFunctions.IsInGame)
                {
                    try
                    {
                        shouldDrawObjects = false;
                        BeginInvoke(refreshRadar);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Format("{0} [Radar] OOG drawing error: {1}", WoWManager.WoWProcess, ex.Message));
                    }
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    localPlayer = ObjectMgr.Pulse(wowObjects, wowPlayers, wowNpcs);
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("{0} [Radar] Pulsing error: {1}", WoWManager.WoWProcess, ex.Message));
                    shouldDrawObjects = false;
                    BeginInvoke(refreshRadar);
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    friends = wowPlayers.Where(i => i.Faction == localPlayer.Faction).ToArray();
                    enemies = wowPlayers.Except(friends).ToArray();
                    objects = wowObjects.Where(i => RadarKOSFind.Contains(i.Name)).ToArray();
                    npcs = wowNpcs.Where(i => RadarKOSFind.Contains(i.Name)).ToArray();
                    if (!GameFunctions.IsLooting && localPlayer.CastingSpellID == 0 && localPlayer.ChannelSpellID == 0 && localPlayer.Alive)
                    {
                        WoWGUID interactGUID = WoWGUID.Zero;
                        double interactDistance = 11;
                        string interactName = string.Empty;
                        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                        foreach (WowObject i in objects.Where(i => RadarKOSFindInteract.Contains(i.Name)))
                        {
                            double distance = i.Location.Distance(localPlayer.Location);
                            if (distance < interactDistance)
                            {
                                interactGUID = i.GUID;
                                interactDistance = distance;
                                interactName = i.Name;
                            }
                        }
                        foreach (WowNpc i in npcs.Where(i => RadarKOSFindInteract.Contains(i.Name)))
                        {
                            double distance = i.Location.Distance(localPlayer.Location);
                            if (distance < interactDistance)
                            {
                                interactGUID = i.GUID;
                                interactDistance = distance;
                                interactName = i.Name;
                            }
                        }
                        if (interactGUID != WoWGUID.Zero)
                        {
                            GameFunctions.Interact(interactGUID);
                            Log.Info(string.Format("{0} [Radar] Interacted with {1} {2}", WoWManager.WoWProcess, interactName, interactGUID));
                        }
                    }
                    bool soundAlarm = objects.Any(i => RadarKOSFindAlarm.Contains(i.Name)) || npcs.Any(i => RadarKOSFindAlarm.Contains(i.Name) && i.Alive);
                    if (!soundAlarmPrevState && soundAlarm)
                    {
                        Task.Factory.StartNew(PlayAlarmFile);
                    }
                    soundAlarmPrevState = soundAlarm;
                    shouldDrawObjects = true;
                    BeginInvoke(refreshRadar);
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("{0} [Radar] Prepainting error: {1}", WoWManager.WoWProcess, ex.Message));
                    shouldDrawObjects = false;
                    BeginInvoke(refreshRadar);
                    Thread.Sleep(100);
                    continue;
                }
                int counter = 100 - (int) stopwatch.ElapsedMilliseconds;
                if (counter > 0 && isRunning)
                {
                    Thread.Sleep(counter);
                }
            }
            Log.Info(string.Format("{0} [Radar] Redraw task is finishing...", WoWManager.WoWProcess));
        }

        private void PictureBox1Paint(object sender, PaintEventArgs e)
        {
            if (shouldDrawObjects && isRunning)
            {
                try
                {
                    flicker = !flicker;
                    int friendsCountAlive = friends.Count(i => i.Alive);
                    checkBoxFriends.Text = string.Concat("F: ", friendsCountAlive.ToString(), "/", friends.Length.ToString());
                    int enemiesCountAlive = enemies.Count(i => i.Alive);
                    checkBoxEnemies.Text = string.Concat("E: ", enemiesCountAlive.ToString(), "/", enemies.Length.ToString());
                    checkBoxObjects.Text = string.Concat("Objects: ", objects.Length.ToString());
                    int npcsCountAlive = npcs.Count(i => i.Alive);
                    checkBoxNpcs.Text = string.Concat("N: ", npcsCountAlive.ToString(), "/", npcs.Length.ToString());

                    objectsPointsInRadarCoords.Clear();

                    Point point = new Point();
                    Point point2 = new Point();
                    Graphics graphics = e.Graphics;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    double localPlayerLocationX = localPlayer.Location.X;
                    double localPlayerLocationY = localPlayer.Location.Y;
                    double var2X;
                    double var2Y;
                    double num2;

                    #region Draw local player

                    double d = -localPlayer.Rotation + 4.71238898038469;
                    point.X = halfOfPictureboxSize;
                    point.Y = halfOfPictureboxSize;
                    graphics.FillRectangle(whiteBrush, point.X - 2, point.Y - 2, 4, 4);
                    point2.X = point.X + (int) (15.0*Math.Cos(d));
                    point2.Y = point.Y + (int) (15.0*Math.Sin(d));
                    graphics.DrawLine(whitePen, point, point2);
                    graphics.DrawEllipse(whitePen, point.X - 40*zoomR, point.Y - 40*zoomR, 80*zoomR, 80*zoomR);

                    #endregion

                    #region Draw friends

                    if (checkBoxFriends.Checked)
                    {
                        foreach (WowPlayer i in friends)
                        {
                            if (checkBoxCorpses.Checked || i.Alive)
                            {
                                var2X = i.Location.X;
                                var2Y = i.Location.Y;
                                num2 = Math.Atan2(var2Y - localPlayerLocationY, var2X - localPlayerLocationX) + 3.1415926535897931 + 1.5707963267948966;
                                var2X = localPlayerLocationX - var2X;
                                var2Y = localPlayerLocationY - var2Y;
                                var2X = (int) (zoomR*var2X);
                                var2Y = (int) (zoomR*var2Y);
                                double num3 = Math.Sqrt(var2X*var2X + var2Y*var2Y);
                                point.X = (int) Math.Round(halfOfPictureboxSize + Math.Abs(num3)*Math.Cos(num2 + 3.1415926535897931));
                                point.Y = (int) Math.Round(halfOfPictureboxSize + Math.Abs(num3)*Math.Sin(num2));
                                Pen pen;
                                SolidBrush solidBrush;
                                if (i.Alive)
                                {
                                    pen = friendPen;
                                    solidBrush = friendBrush;
                                }
                                else
                                {
                                    pen = grayPen;
                                    solidBrush = grayBrush;
                                }
                                Point[] pts;
                                float zDiff = i.Location.Z - localPlayer.Location.Z;
                                if (zDiff >= 10)
                                {
                                    pts = new[] {new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y + 2), new Point(point.X - 2, point.Y + 2)};
                                }
                                else if (zDiff <= -10)
                                {
                                    pts = new[] {new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y - 2), new Point(point.X, point.Y + 2)};
                                }
                                else
                                {
                                    pts = new[] {new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y), new Point(point.X, point.Y + 2), new Point(point.X - 2, point.Y)};
                                }
                                graphics.FillPolygon(solidBrush, pts);
                                if (!flicker || i.TargetGUID != localPlayer.GUID)
                                {
                                    graphics.DrawPolygon(pen, pts);
                                }
                                objectsPointsInRadarCoords.Add(i.GUID, point);
                                point.X += 3;
                                point.Y += 3;
                                if (settings.WoWRadarShowPlayersClasses)
                                {
                                    if (i.Level == localPlayer.Level)
                                    {
                                        graphics.DrawString(i.Class.ToString(), DefaultFont, solidBrush, point); // do not use TextRenderer.DrawText, it's slower
                                    }
                                    else if (i.Level > localPlayer.Level)
                                    {
                                        graphics.DrawString(string.Concat(i.Class.ToString(), "+"), DefaultFont, solidBrush, point); // do not use TextRenderer.DrawText, it's slower
                                    }
                                    else
                                    {
                                        graphics.DrawString(string.Concat(i.Class.ToString(), "-"), DefaultFont, solidBrush, point); // do not use TextRenderer.DrawText, it's slower
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region Draw enemies

                    if (checkBoxEnemies.Checked)
                    {
                        foreach (WowPlayer i in enemies)
                        {
                            if (checkBoxCorpses.Checked || i.Alive)
                            {
                                var2X = i.Location.X;
                                var2Y = i.Location.Y;
                                num2 = Math.Atan2(var2Y - localPlayerLocationY, var2X - localPlayerLocationX) + 3.1415926535897931 + 1.5707963267948966;
                                var2X = localPlayerLocationX - var2X;
                                var2Y = localPlayerLocationY - var2Y;
                                var2X = (int) (zoomR*var2X);
                                var2Y = (int) (zoomR*var2Y);
                                double num3 = Math.Sqrt(var2X*var2X + var2Y*var2Y);
                                point.X = (int) Math.Round(halfOfPictureboxSize + Math.Abs(num3)*Math.Cos(num2 + 3.1415926535897931));
                                point.Y = (int) Math.Round(halfOfPictureboxSize + Math.Abs(num3)*Math.Sin(num2));
                                Pen pen;
                                SolidBrush solidBrush;
                                if (i.Alive)
                                {
                                    pen = enemyPen;
                                    solidBrush = enemyBrush;
                                }
                                else
                                {
                                    pen = grayPen;
                                    solidBrush = grayBrush;
                                }
                                Point[] pts;
                                float zDiff = i.Location.Z - localPlayer.Location.Z;
                                if (zDiff >= 10)
                                {
                                    pts = new[] {new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y + 2), new Point(point.X - 2, point.Y + 2)};
                                }
                                else if (zDiff <= -10)
                                {
                                    pts = new[] {new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y - 2), new Point(point.X, point.Y + 2)};
                                }
                                else
                                {
                                    pts = new[] {new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y), new Point(point.X, point.Y + 2), new Point(point.X - 2, point.Y)};
                                }
                                graphics.FillPolygon(solidBrush, pts);
                                if (!flicker || i.TargetGUID != localPlayer.GUID)
                                {
                                    graphics.DrawPolygon(pen, pts);
                                }
                                objectsPointsInRadarCoords.Add(i.GUID, point);
                                point.X += 3;
                                point.Y += 3;
                                if (settings.WoWRadarShowPlayersClasses)
                                {
                                    if (i.Level == localPlayer.Level)
                                    {
                                        graphics.DrawString(i.Class.ToString(), DefaultFont, solidBrush, point);
                                    }
                                    else if (i.Level > localPlayer.Level)
                                    {
                                        graphics.DrawString(string.Concat(i.Class.ToString(), "+"), DefaultFont, solidBrush, point);
                                    }
                                    else
                                    {
                                        graphics.DrawString(string.Concat(i.Class.ToString(), "-"), DefaultFont, solidBrush, point);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region Draw objects

                    if (checkBoxObjects.Checked)
                    {
                        foreach (WowObject i in objects)
                        {
                            var2X = i.Location.X;
                            var2Y = i.Location.Y;
                            num2 = Math.Atan2(var2Y - localPlayerLocationY, var2X - localPlayerLocationX) + 3.1415926535897931 + 1.5707963267948966;
                            var2X = localPlayerLocationX - var2X;
                            var2Y = localPlayerLocationY - var2Y;
                            var2X = (int) (zoomR*var2X);
                            var2Y = (int) (zoomR*var2Y);
                            double num4 = Math.Sqrt(var2X*var2X + var2Y*var2Y);
                            point.X = (int) Math.Round(halfOfPictureboxSize + Math.Abs(num4)*Math.Cos(num2 + 3.1415926535897931));
                            point.Y = (int) Math.Round(halfOfPictureboxSize + Math.Abs(num4)*Math.Sin(num2));
                            Point[] pts;
                            float zDiff = i.Location.Z - localPlayer.Location.Z;
                            if (zDiff >= 10)
                            {
                                pts = new[] {new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y + 2), new Point(point.X - 2, point.Y + 2)};
                            }
                            else if (zDiff <= -10)
                            {
                                pts = new[] {new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y - 2), new Point(point.X, point.Y + 2)};
                            }
                            else
                            {
                                pts = new[] {new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y), new Point(point.X, point.Y + 2), new Point(point.X - 2, point.Y)};
                            }
                            graphics.DrawPolygon(objectPen, pts);
                            graphics.FillPolygon(objectBrush, pts);
                            objectsPointsInRadarCoords.Add(i.GUID, point);
                            if (settings.WoWRadarShowObjectsNames)
                            {
                                point.X += 3;
                                point.Y += 3;
                                graphics.DrawString(i.Name, DefaultFont, objectBrush, point);
                            }
                        }
                    }

                    #endregion

                    #region Draw NPCs

                    if (checkBoxNpcs.Checked)
                    {
                        foreach (WowNpc i in npcs)
                        {
                            if (checkBoxCorpses.Checked || i.Alive)
                            {
                                var2X = i.Location.X;
                                var2Y = i.Location.Y;
                                num2 = Math.Atan2(var2Y - localPlayerLocationY, var2X - localPlayerLocationX) + 3.1415926535897931 + 1.5707963267948966;
                                var2X = localPlayerLocationX - var2X;
                                var2Y = localPlayerLocationY - var2Y;
                                var2X = (int) (zoomR*var2X);
                                var2Y = (int) (zoomR*var2Y);
                                double num4 = Math.Sqrt(var2X*var2X + var2Y*var2Y);
                                point.X = (int) Math.Round(halfOfPictureboxSize + Math.Abs(num4)*Math.Cos(num2 + 3.1415926535897931));
                                point.Y = (int) Math.Round(halfOfPictureboxSize + Math.Abs(num4)*Math.Sin(num2));
                                Point[] pts;
                                float zDiff = i.Location.Z - localPlayer.Location.Z;
                                if (zDiff >= 10)
                                {
                                    pts = new[] {new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y + 2), new Point(point.X - 2, point.Y + 2)};
                                }
                                else if (zDiff <= -10)
                                {
                                    pts = new[] {new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y - 2), new Point(point.X, point.Y + 2)};
                                }
                                else
                                {
                                    pts = new[] {new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y), new Point(point.X, point.Y + 2), new Point(point.X - 2, point.Y)};
                                }
                                graphics.DrawPolygon(i.Alive ? npcPen : grayPen, pts);
                                graphics.FillPolygon(i.Alive ? npcBrush : grayBrush, pts);
                                objectsPointsInRadarCoords.Add(i.GUID, point);
                                if (settings.WoWRadarShowNPCsNames)
                                {
                                    point.X += 3;
                                    point.Y += 3;
                                    graphics.DrawString(i.Name, DefaultFont, i.Alive ? npcBrush : grayBrush, point);
                                }
                            }
                        }
                    }

                    #endregion

                    point = MousePosition;
                    point.X -= Location.X;
                    point.Y -= Location.Y;
                    if (point.X >= 0 && point.Y > 0 && point.X <= pictureBoxMain.Width && point.Y <= pictureBoxMain.Height)
                    {
                        MeasureTooltip(point);
                        processMouseWheelEvents = true;
                    }
                    else
                    {
                        processMouseWheelEvents = false;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("{0}:{1} :: Radar: Drawing error: {2}", WoWManager.WoWProcess.ProcessName, WoWManager.WoWProcess.ProcessID, ex.Message));
                }
            }
            else
            {
                checkBoxFriends.Text = "F: 0/0";
                checkBoxEnemies.Text = "E: 0/0";
                checkBoxObjects.Text = "Objects: 0";
                checkBoxNpcs.Text = "N: 0/0";
            }
        }

        private void PlayAlarmFile()
        {
            string path = Globals.ResourcesPath + "\\alarm.wav";
            if (File.Exists(path))
            {
                using (SoundPlayer pPlayer = new SoundPlayer(path))
                {
                    pPlayer.PlaySync();
                }
            }
        }

        private void UpdateCaches()
        {
            RadarKOSFind.Clear();
            RadarKOSFindAlarm.Clear();
            RadarKOSFindInteract.Clear();
            foreach (RadarObject i in settings.WoWRadarList.Where(i => i.Enabled))
            {
                RadarKOSFind.Add(i.Name);
                if (i.Interact)
                {
                    RadarKOSFindInteract.Add(i.Name);
                }
                if (i.SoundAlarm)
                {
                    RadarKOSFindAlarm.Add(i.Name);
                }
            }
        }

        private void WoWRadarListChanged(object sender, EventArgs eventArgs)
        {
            UpdateCaches();
        }

        private void PictureBox2Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RadarMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                oldPoint = e.Location;
            }
        }

        private void RadarMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                tmpPoint.X += e.X - oldPoint.X;
                tmpPoint.Y += e.Y - oldPoint.Y;
                Location = tmpPoint;
            }
        }

        private void RadarMouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            settings.WoWRadarLocation = Location;
        }

        private void RadarFormClosing(object sender, FormClosingEventArgs e)
        {
            // ReSharper disable once DelegateSubtraction
            settings.WoWRadarListChanged -= WoWRadarListChanged;
            whitePen.Dispose();
            enemyPen.Dispose();
            friendPen.Dispose();
            grayPen.Dispose();
            objectPen.Dispose();
            npcPen.Dispose();
            whiteBrush.Dispose();
            objectBrush.Dispose();
            friendBrush.Dispose();
            npcBrush.Dispose();
            enemyBrush.Dispose();
            grayBrush.Dispose();

            MouseWheel -= OnMouseWheel;
            pictureBoxMain.MouseWheel -= OnMouseWheel;

            isRunning = false;
            if (!thread.Join(5000))
            {
                Log.Error(string.Format("{0} [Radar] Redraw task termination error, status: {1}", WoWManager.WoWProcess, thread.ThreadState));
            }
            else
            {
                Log.Info(string.Format("{0} [Radar] Redraw task has been successfully ended", WoWManager.WoWProcess));
            }
            Log.Info(string.Format("{0} [Radar] Closed", WoWManager.WoWProcess));
        }

        private void RadarLoad(object sender, EventArgs e)
        {
            Location = settings.WoWRadarLocation;
            isRunning = true;
            thread.Start();
        }

        private void PictureBoxRadarSettingsClick(object sender, EventArgs e)
        {
            new WowRadarOptions().ShowDialog();
        }

        private void SaveCheckBoxes(object sender, EventArgs e)
        {
            byte[] p = new byte[8];
            p[0] = (byte) (checkBoxFriends.Checked ? 1 : 0);
            p[1] = (byte) (checkBoxEnemies.Checked ? 1 : 0);
            p[2] = (byte) (checkBoxNpcs.Checked ? 1 : 0);
            p[3] = (byte) (checkBoxObjects.Checked ? 1 : 0);
            p[4] = (byte) (checkBoxCorpses.Checked ? 1 : 0);
            p[5] = (byte) (zoomR/0.25F);
            settings.WoWRadarShowMode = BitConverter.ToUInt64(p, 0);
        }

        private void MeasureTooltip(Point mousePosition)
        {
            foreach (KeyValuePair<WoWGUID, Point> pair in objectsPointsInRadarCoords)
            {
                if (Math.Abs(pair.Value.X - mousePosition.X) < 4 && Math.Abs(pair.Value.Y - mousePosition.Y) < 4)
                {
                    WowPlayer unit = wowPlayers.FirstOrDefault(i => i.GUID == pair.Key);
                    if (unit != null)
                    {
                        DrawTooltip(mousePosition,
                            string.Concat("   ", unit.Name, "  \r\n   (", unit.Class.ToString(), "*", unit.Level.ToString(), ") ",
                                ((uint) (unit.Health/(float) unit.HealthMax*100)).ToString(), "%"), unit.Class);
                        return;
                    }
                    WowNpc npc = wowNpcs.FirstOrDefault(i => i.GUID == pair.Key);
                    if (npc != null)
                    {
                        DrawTooltip(mousePosition, string.Concat("   ", npc.Name, "  \r\n   ", ((uint) (npc.Health/(float) npc.HealthMax*100)).ToString(), "%"), WowPlayerClass.War);
                        return;
                    }
                    WowObject _object = wowObjects.FirstOrDefault(i => i.GUID == pair.Key);
                    if (_object != null)
                    {
                        DrawTooltip(mousePosition, string.Concat("   ", _object.Name, "  "), WowPlayerClass.Rog);
                        return;
                    }
                }
            }
            textBoxDetailedInfo.Visible = false;
        }
        private void DrawTooltip(Point e, string text, WowPlayerClass _class)
        {
            textBoxDetailedInfo.Text = text;
            textBoxDetailedInfo.Width = TextRenderer.MeasureText(text, textBoxDetailedInfo.Font).Width;
            if (e.X < halfOfPictureboxSize)
            {
                textBoxDetailedInfo.Location = new Point(e.X + 5, e.Y);
                textBoxDetailedInfo.TextAlign = HorizontalAlignment.Left;
            }
            else
            {
                textBoxDetailedInfo.Location = new Point(e.X - textBoxDetailedInfo.Size.Width - 5, e.Y);
                textBoxDetailedInfo.TextAlign = HorizontalAlignment.Right;
            }
            switch (_class)
            {
                case WowPlayerClass.DK:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Deathknight;
                    break;
                case WowPlayerClass.Dru:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Druid;
                    break;
                case WowPlayerClass.Hun:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Hunter;
                    break;
                case WowPlayerClass.Mg:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Mage;
                    break;
                case WowPlayerClass.Mnk:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Monk;
                    break;
                case WowPlayerClass.Pal:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Paladin;
                    break;
                case WowPlayerClass.Pri:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Priest;
                    break;
                case WowPlayerClass.Rog:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Rogue;
                    break;
                case WowPlayerClass.Sha:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Shaman;
                    break;
                case WowPlayerClass.WL:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Warlock;
                    break;
                case WowPlayerClass.War:
                    textBoxDetailedInfo.BackColor = WowRaidClassColors.Warrior;
                    break;
            }
            textBoxDetailedInfo.Visible = true;
        }

        private void PictureBoxMainMouseLeave(object sender, EventArgs e)
        {
            textBoxDetailedInfo.Visible = false;
        }

        private void PictureBoxMainMouseClick(object sender, MouseEventArgs e)
        {
            foreach (KeyValuePair<WoWGUID, Point> pair in objectsPointsInRadarCoords)
            {
                if (Math.Abs(pair.Value.X - e.X) < 4 && Math.Abs(pair.Value.Y - e.Y) < 4)
                {
                    WowPlayer unit = wowPlayers.FirstOrDefault(i => i.GUID == pair.Key);
                    if (unit != null)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            unit.Target();
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            MoveToWrapper(unit.Location);
                        }
                        break;
                    }
                    WowNpc npc = wowNpcs.FirstOrDefault(i => i.GUID == pair.Key);
                    if (npc != null)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            npc.Target();
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            MoveToWrapper(npc.Location);
                        }
                        break;
                    }
                    WowObject wowObject = wowObjects.FirstOrDefault(i => i.GUID == pair.Key);
                    if (wowObject != null)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            GameFunctions.Interact(wowObject.GUID);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            MoveToWrapper(wowObject.Location);
                        }
                        break;
                    }
                }
            }
        }

        private void CheckBoxFriendsMouseClickExtended(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.AllowFullOpen = true;
                    colorDialog.AnyColor = true;
                    colorDialog.Color = settings.WoWRadarFriendColor;
                    colorDialog.FullOpen = true;
                    colorDialog.SolidColorOnly = true;
                    if (colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        settings.WoWRadarFriendColor = colorDialog.Color;
                        friendPen.Color = colorDialog.Color;
                        friendBrush.Color = colorDialog.Color;
                        checkBoxFriends.ForeColor = settings.WoWRadarFriendColor;
                    }
                }
            }
        }
        private void CheckBoxEnemiesMouseClickExtended(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.AllowFullOpen = true;
                    colorDialog.AnyColor = true;
                    colorDialog.Color = settings.WoWRadarEnemyColor;
                    colorDialog.FullOpen = true;
                    colorDialog.SolidColorOnly = true;
                    if (colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        settings.WoWRadarEnemyColor = colorDialog.Color;
                        enemyPen.Color = colorDialog.Color;
                        enemyBrush.Color = colorDialog.Color;
                        checkBoxEnemies.ForeColor = settings.WoWRadarEnemyColor;
                    }
                }
            }
        }
        private void CheckBoxNpcsMouseClickExtended(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.AllowFullOpen = true;
                    colorDialog.AnyColor = true;
                    colorDialog.Color = settings.WoWRadarNPCColor;
                    colorDialog.FullOpen = true;
                    colorDialog.SolidColorOnly = true;
                    if (colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        settings.WoWRadarNPCColor = colorDialog.Color;
                        npcPen.Color = colorDialog.Color;
                        npcBrush.Color = colorDialog.Color;
                        checkBoxNpcs.ForeColor = settings.WoWRadarNPCColor;
                    }
                }
            }
        }
        private void CheckBoxObjectsMouseClickExtended(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.AllowFullOpen = true;
                    colorDialog.AnyColor = true;
                    colorDialog.Color = settings.WoWRadarObjectColor;
                    colorDialog.FullOpen = true;
                    colorDialog.SolidColorOnly = true;
                    if (colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        settings.WoWRadarObjectColor = colorDialog.Color;
                        objectPen.Color = colorDialog.Color;
                        objectBrush.Color = colorDialog.Color;
                        checkBoxObjects.ForeColor = settings.WoWRadarObjectColor;
                    }
                }
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            if (processMouseWheelEvents)
            {
                if (mouseEventArgs.Delta > 0)
                {
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (zoomR == 0.125F) return;
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                    zoomR = zoomR / 2;
                    SaveCheckBoxes(null, EventArgs.Empty);
                }
                else
                {
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (zoomR == 2F) return;
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                    zoomR = zoomR * 2;
                    SaveCheckBoxes(null, EventArgs.Empty);
                }
            }
        }

        private void MoveToWrapper(WowPoint point)
        {
            Task.Run(() =>
            {
                GameFunctions.Move2D(point, 3f, 5000, false);
            });
        }

    }
}
