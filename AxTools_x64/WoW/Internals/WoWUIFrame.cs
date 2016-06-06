using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AxTools.Helpers;

namespace AxTools.WoW.Internals
{
    public class WoWUIFrame
    {
        private readonly IntPtr address;
        private string cachedName;
        private string cachedEditboxText;

        internal WoWUIFrame(IntPtr address)
        {
            this.address = address;
        }

        public string GetName
        {
            get
            {
                if (cachedName == null)
                {
                    byte[] bytes = WoWManager.WoWProcess.Memory.ReadBytes(WoWManager.WoWProcess.Memory.Read<IntPtr>(address + WowBuildInfoX64.UIFrameName), 132 * 2);
                    cachedName = Encoding.UTF8.GetString(bytes.TakeWhile(l => l != 0).ToArray());
                }
                return cachedName;
            }
        }

        public string EditboxText
        {
            get
            {
                if (cachedEditboxText == null)
                {
                    try
                    {
                        byte[] bytes = WoWManager.WoWProcess.Memory.ReadBytes(WoWManager.WoWProcess.Memory.Read<IntPtr>(address + WowBuildInfoX64.UIEditBoxText), 255 * 2); // 255 - max string length; 2 - utf8 char length
                        cachedEditboxText = Encoding.UTF8.GetString(bytes.TakeWhile(l => l != 0).ToArray());
                    }
                    catch
                    {
                        return "";
                    }
                }
                return cachedEditboxText;
            }
        }

        public bool IsVisible
        {
            get { return ((WoWManager.WoWProcess.Memory.Read<int>(address + WowBuildInfoX64.UIFrameVisible) >> WowBuildInfoX64.UIFrameVisible1) & WowBuildInfoX64.UIFrameVisible2) == 1; }
        }

        /// <summary>
        /// Gets the name of the frame by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static WoWUIFrame GetFrameByName(string name)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            WoWUIFrame frame = GetAllFrames().FirstOrDefault(l => l.GetName == name);
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                Log.Error(string.Format("WoWUIFrame.GetFrameByName exec time: {0}ms", stopwatch.ElapsedMilliseconds));
            }
            return frame;
        }

        public static IEnumerable<WoWUIFrame> GetAllFrames()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IntPtr @base = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.UIFrameBase);
            IntPtr currentFrame = WoWManager.WoWProcess.Memory.Read<IntPtr>(@base + WowBuildInfoX64.UIFirstFrame);
            while (currentFrame != IntPtr.Zero)
            {
                WoWUIFrame f = null;
                bool shouldExit = false;
                try
                {
                    WoWUIFrame temp = new WoWUIFrame(currentFrame);
                    //File.AppendAllText(Application.StartupPath + "\\frames.txt", string.Format("New frame: {0}; Visible: {1}\r\n", f.GetName, f.IsVisible));
                    if (!string.IsNullOrWhiteSpace(temp.GetName))
                    {
                        f = temp;
                    }
                }
                catch
                {
                    //
                }
                finally
                {
                    try
                    {
                        currentFrame = WoWManager.WoWProcess.Memory.Read<IntPtr>(currentFrame + WoWManager.WoWProcess.Memory.Read<int>(@base + WowBuildInfoX64.UINextFrame) + 8);
                    }
                    catch
                    {
                        shouldExit = true;
                    }
                }
                if (f != null)
                {
                    yield return f;
                }
                if (shouldExit)
                {
                    break;
                }
            }
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                Log.Error(string.Format("WoWUIFrame.GetAllFrames exec time: {0}ms", stopwatch.ElapsedMilliseconds));
            }
        }

    }
}
