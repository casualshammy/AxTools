using AxTools.WinAPI;
using AxTools.WoW.Internals;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Radar
{
    public partial class Radar : Form, IPlugin3
    {
        #region IPlugin3 info

        public new string Name => nameof(Radar);

        public bool ConfigAvailable => false;

        public string[] Dependencies => new string[0];

        public string Description => "Displays players/NPCs/objects on radar";

        public bool DontCloseOnWowShutdown => false;

        public Image TrayIcon => ImagesBase64.Icon;

        public Version Version => new Version(1, 0);

        #endregion IPlugin3 info

        #region IPlugin3 methods

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart(GameInterface game)
        {
            Utilities.InvokeInGUIThread(delegate
            {
                actualWindow = new Radar(game);
                actualWindow.Show();
            });
        }

        public void OnStop()
        {
            Utilities.InvokeInGUIThread(delegate
            {
                if (actualWindow != null)
                {
                    var alreadyClosing = actualWindow.ClosingProcessStarted;
                    actualWindow.ClosingProcessStarted = true;
                    if (!alreadyClosing)
                    {
                        actualWindow.optionsWindow?.Close();
                        actualWindow.Close();
                    }
                }
            });
        }

        #endregion IPlugin3 methods

        #region Fields

        private readonly GameInterface info;
        private Radar actualWindow;
        private OptionsWindow optionsWindow;
        private bool ClosingProcessStarted;

        private bool processMouseWheelEvents;

        private static readonly HashSet<string> RadarKOSFind = new HashSet<string>();
        private static readonly HashSet<string> RadarKOSFindAlarm = new HashSet<string>();
        private static readonly HashSet<string> RadarKOSFindInteract = new HashSet<string>();

        private readonly Settings settings;
        private Pen friendPen, enemyPen, npcPen, objectPen, whitePen, grayPen;
        private SolidBrush friendBrush, enemyBrush, npcBrush, objectBrush, whiteBrush, grayBrush;
        private Point tmpPoint = Point.Empty;
        private Point oldPoint = Point.Empty;
        private bool isDragging;
        private float zoomR;
        private readonly int halfOfPictureboxSize;
        private readonly List<WowObject> wowObjects = new List<WowObject>();
        private readonly List<WowPlayer> wowPlayers = new List<WowPlayer>();
        private readonly List<WowNpc> wowNpcs = new List<WowNpc>();
        private readonly Dictionary<WoWGUID, Point> objectsPointsInRadarCoords = new Dictionary<WoWGUID, Point>();
        private Task<string> getPlayerNameTask;

        private readonly Dictionary<WowPlayerClass, Color> wowClassColors = new Dictionary<WowPlayerClass, Color>
        {
            {WowPlayerClass.DK, Color.FromArgb(196, 31, 59)},
            {WowPlayerClass.Dru, Color.FromArgb(255, 125, 10)},
            {WowPlayerClass.Hun, Color.FromArgb(171, 212, 115)},
            {WowPlayerClass.Mg, Color.FromArgb(105, 204, 240)},
            {WowPlayerClass.Mnk, Color.FromArgb(0, 255, 150)},
            {WowPlayerClass.Pal, Color.FromArgb(245, 140, 186)},
            {WowPlayerClass.Pri, Color.FromArgb(255, 255, 255)},
            {WowPlayerClass.Rog, Color.FromArgb(255, 245, 105)},
            {WowPlayerClass.Sha, Color.FromArgb(0, 112, 222)},
            {WowPlayerClass.WL, Color.FromArgb(148, 130, 201)},
            {WowPlayerClass.War, Color.FromArgb(199, 156, 110)},
            {WowPlayerClass.DeH, Color.FromArgb(163, 48, 201)},
        };

        private WoWPlayerMe localPlayer;
        private WowPlayer[] friends;
        private WowPlayer[] enemies;
        private WowObject[] objects;
        private WowNpc[] npcs;
        private readonly Thread thread;
        private volatile bool isRunning;
        private volatile bool shouldDrawObjects;
        private bool flicker;
        private Point lastMouseLocation = Point.Empty;

        #endregion Fields

        public Radar() { }

        internal Radar(GameInterface game)
        {
            InitializeComponent();
            using (Bitmap bitmap = new Bitmap(ImagesBase64.Icon))
            {
                Icon = Icon.FromHandle(bitmap.GetHicon());
            }
            info = game;
            settings = this.LoadSettingsJSON<Settings>();
            SetupBrushes(settings);
            settings.List.CollectionChanged += WoWRadarListChanged;
            UpdateCaches();
            checkBoxFriends.ForeColor = settings.FriendColor;
            checkBoxEnemies.ForeColor = settings.EnemyColor;
            checkBoxNpcs.ForeColor = settings.NPCColor;
            checkBoxObjects.ForeColor = settings.ObjectColor;
            halfOfPictureboxSize = pictureBoxMain.Width / 2;

            checkBoxFriends.Checked = settings.DisplayFriends;
            checkBoxEnemies.Checked = settings.DisplayEnemies;
            checkBoxNpcs.Checked = settings.DisplayNpcs;
            checkBoxObjects.Checked = settings.DisplayObjects;
            checkBoxCorpses.Checked = settings.DisplayCorpses;
            zoomR = settings.Zoom;

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
                Thread.Sleep(6000);
                BeginInvoke((MethodInvoker)(() => { labelHint.Visible = false; }));
            });

            this.LogPrint("Loaded");
        }

        private void SetupBrushes(Settings settings)
        {
            friendPen = new Pen(settings.FriendColor, 1f);
            enemyPen = new Pen(settings.EnemyColor, 1f);
            npcPen = new Pen(settings.NPCColor, 1f);
            objectPen = new Pen(settings.ObjectColor, 1f);
            whitePen = new Pen(Color.White, 1f);
            grayPen = new Pen(Color.Gray, 1f);
            friendBrush = new SolidBrush(settings.FriendColor);
            enemyBrush = new SolidBrush(settings.EnemyColor);
            npcBrush = new SolidBrush(settings.NPCColor);
            objectBrush = new SolidBrush(settings.ObjectColor);
            whiteBrush = new SolidBrush(Color.White);
            grayBrush = new SolidBrush(Color.Gray);
        }

        private void Redraw()
        {
            Action refreshRadar = pictureBoxMain.Invalidate;
            var soundAlarmPrevState = false;
            var stopwatch = new Stopwatch();
            while (isRunning)
            {
                stopwatch.Restart();
                if (!info.IsInGame || info.IsLoadingScreen)
                {
                    try
                    {
                        shouldDrawObjects = false;
                        BeginInvoke(refreshRadar);
                    }
                    catch (Exception ex)
                    {
                        this.LogError($"OOG drawing error: {ex.Message}");
                    }
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    localPlayer = info.GetGameObjects(wowObjects, wowPlayers, wowNpcs);
                }
                catch (Exception ex)
                {
                    if (ex is Win32Exception win32ex && win32ex.NativeErrorCode == (int)Win32Error.ERROR_PARTIAL_COPY) // it's rarely happening, dunno why
                    {
                        this.LogPrint($"Pulsing error: {ex.Message}");
                    }
                    else
                    {
                        this.LogError($"Pulsing error: {ex.Message}");
                    }
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
                    Redraw_Interact();
                    Redraw_Alarm(ref soundAlarmPrevState);
                    shouldDrawObjects = true;
                    BeginInvoke(refreshRadar);
                }
                catch (Exception ex)
                {
                    this.LogError($"Prepainting error: {ex.Message}");
                    shouldDrawObjects = false;
                    BeginInvoke(refreshRadar);
                    Thread.Sleep(100);
                    continue;
                }
                var counter = 100 - (int)stopwatch.ElapsedMilliseconds;
                if (counter > 0 && isRunning)
                {
                    Thread.Sleep(counter);
                }
            }
            this.LogPrint("Redraw task is finishing...");
        }

        private void Redraw_Interact()
        {
            if (!info.IsLooting && localPlayer.CastingSpellID == 0 && localPlayer.ChannelSpellID == 0 && localPlayer.Alive)
            {
                WoWObjectBase whatToInteract = null;
                double interactDistance = 11;
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                foreach (WowObject i in objects.Where(i => RadarKOSFindInteract.Contains(i.Name)))
                {
                    var distance = i.Location.Distance(localPlayer.Location);
                    if (distance < interactDistance)
                    {
                        interactDistance = distance;
                        whatToInteract = i;
                    }
                }
                foreach (WowNpc i in npcs.Where(i => RadarKOSFindInteract.Contains(i.Name)))
                {
                    var distance = i.Location.Distance(localPlayer.Location);
                    if (distance < interactDistance)
                    {
                        interactDistance = distance;
                        whatToInteract = i;
                    }
                }
                if (whatToInteract != null)
                {
                    whatToInteract.Interact();
                    this.LogPrint($"Interacted with {whatToInteract.Name} {whatToInteract.GUID}");
                }
            }
        }

        private void Redraw_Alarm(ref bool alarmState)
        {
            string newPOIName = null;
            var @object = objects.FirstOrDefault(i => RadarKOSFindAlarm.Contains(i.Name));
            if (@object != null)
            {
                newPOIName = @object.Name;
            }
            else
            {
                var npc = npcs.FirstOrDefault(i => RadarKOSFindAlarm.Contains(i.Name) && i.Alive);
                if (npc != null)
                {
                    newPOIName = npc.Name;
                }
            }
            if (!alarmState && newPOIName != null)
            {
                Task.Factory.StartNew(PlayAlarmFile);
                Task.Run(() =>
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        labelHint.Text = newPOIName;
                        labelHint.Show();
                    }));
                    Thread.Sleep(5000);
                    BeginInvoke(new MethodInvoker(labelHint.Hide));
                });
            }
            alarmState = newPOIName != null;
        }

        private void PictureBox1Paint(object sender, PaintEventArgs e)
        {
            if (shouldDrawObjects && isRunning)
            {
                try
                {
                    flicker = !flicker;
                    var friendsCountAlive = friends.Count(i => i.Alive);
                    checkBoxFriends.Text = string.Concat("F: ", friendsCountAlive.ToString(), "/", friends.Length.ToString());
                    var enemiesCountAlive = enemies.Count(i => i.Alive);
                    checkBoxEnemies.Text = string.Concat("E: ", enemiesCountAlive.ToString(), "/", enemies.Length.ToString());
                    checkBoxObjects.Text = string.Concat("Objects: ", objects.Length.ToString());
                    var npcsCountAlive = npcs.Count(i => i.Alive);
                    checkBoxNpcs.Text = string.Concat("N: ", npcsCountAlive.ToString(), "/", npcs.Length.ToString());

                    objectsPointsInRadarCoords.Clear();

                    var point = new Point();
                    var point2 = new Point();
                    var graphics = e.Graphics;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    double localPlayerLocationX = localPlayer.Location.X;
                    double localPlayerLocationY = localPlayer.Location.Y;
                    double var2X;
                    double var2Y;
                    double num2;

                    #region Draw local player

                    if (!settings.ShowLocalPlayerRotationArrowOnTop)
                    {
                        var d = -localPlayer.Rotation + 3 * Math.PI / 2;
                        point.X = halfOfPictureboxSize;
                        point.Y = halfOfPictureboxSize;
                        graphics.FillRectangle(whiteBrush, point.X - 2, point.Y - 2, 4, 4);
                        point2.X = point.X + (int)(15.0 * Math.Cos(d));
                        point2.Y = point.Y + (int)(15.0 * Math.Sin(d));
                        graphics.DrawLine(whitePen, point, point2);
                        graphics.DrawEllipse(whitePen, point.X - 40 * zoomR, point.Y - 40 * zoomR, 80 * zoomR, 80 * zoomR);
                    }

                    #endregion Draw local player

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
                                var2X = (int)(zoomR * var2X);
                                var2Y = (int)(zoomR * var2Y);
                                var num3 = Math.Sqrt(var2X * var2X + var2Y * var2Y);
                                point.X = (int)Math.Round(halfOfPictureboxSize + Math.Abs(num3) * Math.Cos(num2 + 3.1415926535897931));
                                point.Y = (int)Math.Round(halfOfPictureboxSize + Math.Abs(num3) * Math.Sin(num2));
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
                                var zDiff = i.Location.Z - localPlayer.Location.Z;
                                if (zDiff >= 10)
                                {
                                    pts = new[] { new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y + 2), new Point(point.X - 2, point.Y + 2) };
                                }
                                else if (zDiff <= -10)
                                {
                                    pts = new[] { new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y - 2), new Point(point.X, point.Y + 2) };
                                }
                                else
                                {
                                    pts = new[] { new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y), new Point(point.X, point.Y + 2), new Point(point.X - 2, point.Y) };
                                }
                                graphics.FillPolygon(solidBrush, pts);
                                if (!flicker || i.TargetGUID != localPlayer.GUID)
                                {
                                    graphics.DrawPolygon(pen, pts);
                                }
                                objectsPointsInRadarCoords[i.GUID] = point;
                                point.X += 3;
                                point.Y += 3;
                                if (settings.ShowPlayersClasses)
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

                    #endregion Draw friends

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
                                var2X = (int)(zoomR * var2X);
                                var2Y = (int)(zoomR * var2Y);
                                var num3 = Math.Sqrt(var2X * var2X + var2Y * var2Y);
                                point.X = (int)Math.Round(halfOfPictureboxSize + Math.Abs(num3) * Math.Cos(num2 + 3.1415926535897931));
                                point.Y = (int)Math.Round(halfOfPictureboxSize + Math.Abs(num3) * Math.Sin(num2));
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
                                var zDiff = i.Location.Z - localPlayer.Location.Z;
                                if (zDiff >= 10)
                                {
                                    pts = new[] { new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y + 2), new Point(point.X - 2, point.Y + 2) };
                                }
                                else if (zDiff <= -10)
                                {
                                    pts = new[] { new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y - 2), new Point(point.X, point.Y + 2) };
                                }
                                else
                                {
                                    pts = new[] { new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y), new Point(point.X, point.Y + 2), new Point(point.X - 2, point.Y) };
                                }
                                graphics.FillPolygon(solidBrush, pts);
                                if (!flicker || i.TargetGUID != localPlayer.GUID)
                                {
                                    graphics.DrawPolygon(pen, pts);
                                }
                                objectsPointsInRadarCoords[i.GUID] = point;
                                point.X += 3;
                                point.Y += 3;
                                if (settings.ShowPlayersClasses)
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

                    #endregion Draw enemies

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
                            var2X = (int)(zoomR * var2X);
                            var2Y = (int)(zoomR * var2Y);
                            var num4 = Math.Sqrt(var2X * var2X + var2Y * var2Y);
                            point.X = (int)Math.Round(halfOfPictureboxSize + Math.Abs(num4) * Math.Cos(num2 + 3.1415926535897931));
                            point.Y = (int)Math.Round(halfOfPictureboxSize + Math.Abs(num4) * Math.Sin(num2));
                            Point[] pts;
                            var zDiff = i.Location.Z - localPlayer.Location.Z;
                            if (zDiff >= 10)
                            {
                                pts = new[] { new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y + 2), new Point(point.X - 2, point.Y + 2) };
                            }
                            else if (zDiff <= -10)
                            {
                                pts = new[] { new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y - 2), new Point(point.X, point.Y + 2) };
                            }
                            else
                            {
                                pts = new[] { new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y), new Point(point.X, point.Y + 2), new Point(point.X - 2, point.Y) };
                            }
                            graphics.DrawPolygon(objectPen, pts);
                            graphics.FillPolygon(objectBrush, pts);
                            objectsPointsInRadarCoords[i.GUID] = point;
                            if (settings.ShowObjectsNames)
                            {
                                point.X += 3;
                                point.Y += 3;
                                graphics.DrawString(i.Name, DefaultFont, objectBrush, point);
                            }
                        }
                    }

                    #endregion Draw objects

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
                                var2X = (int)(zoomR * var2X);
                                var2Y = (int)(zoomR * var2Y);
                                var num4 = Math.Sqrt(var2X * var2X + var2Y * var2Y);
                                point.X = (int)Math.Round(halfOfPictureboxSize + Math.Abs(num4) * Math.Cos(num2 + 3.1415926535897931));
                                point.Y = (int)Math.Round(halfOfPictureboxSize + Math.Abs(num4) * Math.Sin(num2));
                                Point[] pts;
                                var zDiff = i.Location.Z - localPlayer.Location.Z;
                                if (zDiff >= 10)
                                {
                                    pts = new[] { new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y + 2), new Point(point.X - 2, point.Y + 2) };
                                }
                                else if (zDiff <= -10)
                                {
                                    pts = new[] { new Point(point.X - 2, point.Y - 2), new Point(point.X + 2, point.Y - 2), new Point(point.X, point.Y + 2) };
                                }
                                else
                                {
                                    pts = new[] { new Point(point.X, point.Y - 2), new Point(point.X + 2, point.Y), new Point(point.X, point.Y + 2), new Point(point.X - 2, point.Y) };
                                }
                                graphics.DrawPolygon(i.Alive ? npcPen : grayPen, pts);
                                graphics.FillPolygon(i.Alive ? npcBrush : grayBrush, pts);
                                objectsPointsInRadarCoords[i.GUID] = point;
                                if (settings.ShowNPCsNames)
                                {
                                    point.X += 3;
                                    point.Y += 3;
                                    graphics.DrawString(i.Name, DefaultFont, i.Alive ? npcBrush : grayBrush, point);
                                }
                            }
                        }
                    }

                    #endregion Draw NPCs

                    #region Draw local player

                    if (settings.ShowLocalPlayerRotationArrowOnTop)
                    {
                        var d = -localPlayer.Rotation + 4.71238898038469;
                        point.X = halfOfPictureboxSize;
                        point.Y = halfOfPictureboxSize;
                        graphics.FillRectangle(whiteBrush, point.X - 2, point.Y - 2, 4, 4);
                        point2.X = point.X + (int)(15.0 * Math.Cos(d));
                        point2.Y = point.Y + (int)(15.0 * Math.Sin(d));
                        graphics.DrawLine(whitePen, point, point2);
                        graphics.DrawEllipse(whitePen, point.X - 40 * zoomR, point.Y - 40 * zoomR, 80 * zoomR, 80 * zoomR);
                    }

                    #endregion Draw local player

                    point = MousePosition;
                    point.X -= Location.X;
                    point.Y -= Location.Y;
                    if (point.X >= 0 && point.Y > 0 && point.X <= pictureBoxMain.Width && point.Y <= pictureBoxMain.Height)
                    {
                        MeasureTooltipAsync(point);
                        processMouseWheelEvents = true;
                    }
                    else
                    {
                        processMouseWheelEvents = false;
                    }
                }
                catch (Exception ex)
                {
                    this.LogError($"Drawing error: {ex.Message}");
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
            if (File.Exists(settings.AlarmSoundFile))
            {
                using (SoundPlayer pPlayer = new SoundPlayer(settings.AlarmSoundFile))
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
            foreach (RadarObject i in settings.List.Where(i => i.Enabled))
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

        private void WoWRadarListChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
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
            settings.Location = Location;
        }

        private void RadarFormClosing(object sender, FormClosingEventArgs e)
        {
            // ReSharper disable once DelegateSubtraction
            var alreadyClosing = ClosingProcessStarted;
            ClosingProcessStarted = true;
            settings.List.CollectionChanged -= WoWRadarListChanged;
            this.SaveSettingsJSON(settings);
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
                this.LogError($"Redraw task termination error, status: {thread.ThreadState}");
            }
            else
            {
                this.LogPrint("Redraw task has been successfully ended");
            }
            this.LogPrint("Closed");
            if (!alreadyClosing)
            {
                Utilities.RemovePluginFromRunning(Name);
            }
        }

        private void RadarLoad(object sender, EventArgs e)
        {
            Location = settings.Location;
            isRunning = true;
            thread.Start();
        }

        private void PictureBoxRadarSettingsClick(object sender, EventArgs e)
        {
            optionsWindow = new OptionsWindow(this, info, settings);
            optionsWindow.ShowDialog();
        }

        private void SaveCheckBoxes(object sender, EventArgs e)
        {
            settings.DisplayFriends = checkBoxFriends.Checked;
            settings.DisplayEnemies = checkBoxEnemies.Checked;
            settings.DisplayNpcs = checkBoxNpcs.Checked;
            settings.DisplayObjects = checkBoxObjects.Checked;
            settings.DisplayCorpses = checkBoxCorpses.Checked;
            settings.Zoom = zoomR;
        }

        private async void MeasureTooltipAsync(Point mousePosition)
        {
            foreach (KeyValuePair<WoWGUID, Point> pair in objectsPointsInRadarCoords)
            {
                if (Math.Abs(pair.Value.X - mousePosition.X) < 4 && Math.Abs(pair.Value.Y - mousePosition.Y) < 4)
                {
                    lastMouseLocation = mousePosition;
                    var unit = wowPlayers.FirstOrDefault(i => i.GUID == pair.Key);
                    if (unit != null)
                    {
                        DrawTooltip(mousePosition, $"   {await GetPlayerNameAsync(unit)}  \r\n   ({unit.Class}*{unit.Level}) {(uint)(unit.Health / (float)unit.HealthMax * 100)}%", unit.Class);
                        return;
                    }
                    var npc = wowNpcs.FirstOrDefault(i => i.GUID == pair.Key);
                    if (npc != null)
                    {
                        DrawTooltip(mousePosition, string.Concat("   ", npc.Name, "  \r\n   ", ((uint)(npc.Health / (float)npc.HealthMax * 100)).ToString(), "%"), WowPlayerClass.War);
                        return;
                    }
                    var _object = wowObjects.FirstOrDefault(i => i.GUID == pair.Key);
                    if (_object != null)
                    {
                        DrawTooltip(mousePosition, string.Concat("   ", _object.Name, "  "), WowPlayerClass.Rog);
                        return;
                    }
                }
            }
            if (lastMouseLocation != mousePosition)
            {
                textBoxDetailedInfo.Visible = false;
            }
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
            textBoxDetailedInfo.BackColor = wowClassColors[_class];
            textBoxDetailedInfo.Visible = true;
        }

        private Task<string> GetPlayerNameAsync(WowPlayer unit)
        {
            if (getPlayerNameTask == null)
            {
                return (getPlayerNameTask = Task.Run(() => unit.Name));
            }
            else if (getPlayerNameTask.Status == TaskStatus.Canceled || getPlayerNameTask.Status == TaskStatus.Faulted || getPlayerNameTask.Status == TaskStatus.RanToCompletion)
            {
                return (getPlayerNameTask = Task.Run(() => unit.Name));
            }
            else
            {
                return Task.Run(() => "Please wait...");
            }
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
                    var unit = wowPlayers.FirstOrDefault(i => i.GUID == pair.Key);
                    if (unit != null)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            Task.Run(() => unit.Target());
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            MoveToWrapper(unit.Location);
                        }
                        break;
                    }
                    var npc = wowNpcs.FirstOrDefault(i => i.GUID == pair.Key);
                    if (npc != null)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            Task.Run(() => npc.Target());
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            MoveToWrapper(npc.Location);
                        }
                        else if (e.Button == MouseButtons.Middle)
                        {
                            Task.Run((Action)npc.Interact);
                        }
                        break;
                    }
                    var wowObject = wowObjects.FirstOrDefault(i => i.GUID == pair.Key);
                    if (wowObject != null)
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            Task.Run(() => wowObject.Interact());
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
                    colorDialog.Color = settings.FriendColor;
                    colorDialog.FullOpen = true;
                    colorDialog.SolidColorOnly = true;
                    if (colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        settings.FriendColor = colorDialog.Color;
                        friendPen.Color = colorDialog.Color;
                        friendBrush.Color = colorDialog.Color;
                        checkBoxFriends.ForeColor = settings.FriendColor;
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
                    colorDialog.Color = settings.EnemyColor;
                    colorDialog.FullOpen = true;
                    colorDialog.SolidColorOnly = true;
                    if (colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        settings.EnemyColor = colorDialog.Color;
                        enemyPen.Color = colorDialog.Color;
                        enemyBrush.Color = colorDialog.Color;
                        checkBoxEnemies.ForeColor = settings.EnemyColor;
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
                    colorDialog.Color = settings.NPCColor;
                    colorDialog.FullOpen = true;
                    colorDialog.SolidColorOnly = true;
                    if (colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        settings.NPCColor = colorDialog.Color;
                        npcPen.Color = colorDialog.Color;
                        npcBrush.Color = colorDialog.Color;
                        checkBoxNpcs.ForeColor = settings.NPCColor;
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
                    colorDialog.Color = settings.ObjectColor;
                    colorDialog.FullOpen = true;
                    colorDialog.SolidColorOnly = true;
                    if (colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        settings.ObjectColor = colorDialog.Color;
                        objectPen.Color = colorDialog.Color;
                        objectBrush.Color = colorDialog.Color;
                        checkBoxObjects.ForeColor = settings.ObjectColor;
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
                info.Move2D(point, 3f, 5000, false, false);
            });
        }
    }
}