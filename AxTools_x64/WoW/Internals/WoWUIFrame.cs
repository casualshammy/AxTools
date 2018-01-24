using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AxTools.Helpers;
using AxTools.WoW.PluginSystem.API;

namespace AxTools.WoW.Internals
{
    public class WoWUIFrame
    {
        private readonly IntPtr address;
        private string cachedName;
        private string cachedEditboxText;
        private WowProcess process;
        private static readonly Log2 log = new Log2("WoWUIFrame");

        internal WoWUIFrame(IntPtr address)
        {
            this.address = address;
            process = WoWManager.WoWProcess;
        }

        internal WoWUIFrame(IntPtr address, WowProcess proc)
        {
            this.address = address;
            process = proc;
        }

        public string GetName
        {
            get
            {
                if (cachedName == null)
                {
                    byte[] bytes = process.Memory.ReadBytes(process.Memory.Read<IntPtr>(address + WowBuildInfoX64.UIFrameName), 132 * 2);
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
                        byte[] bytes = process.Memory.ReadBytes(process.Memory.Read<IntPtr>(address + WowBuildInfoX64.UIEditBoxText), 255 * 2); // 255 - max string length; 2 - utf8 char length
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
            get { return ((process.Memory.Read<int>(address + WowBuildInfoX64.UIFrameVisible) >> WowBuildInfoX64.UIFrameVisible1) & WowBuildInfoX64.UIFrameVisible2) == 1; }
        }

        /// <summary>
        /// Gets the name of the frame by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static WoWUIFrame GetFrameByName(string name)
        {
            return GetFrameByName(WoWManager.WoWProcess, name);
        }

        internal static WoWUIFrame GetFrameByName(WowProcess process, string name)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            WoWUIFrame frame = GetAllFrames(process).FirstOrDefault(l => l.GetName == name);
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                log.Error(string.Format("GetFrameByName exec time: {0}ms", stopwatch.ElapsedMilliseconds));
            }
            return frame;
        }

        public static IEnumerable<WoWUIFrame> GetAllFrames()
        {
            return GetAllFrames(WoWManager.WoWProcess);
        }

        internal static IEnumerable<WoWUIFrame> GetAllFrames(WowProcess process)
        {
            if (process.IsValidBuild && Info.IsProcessInGame(process))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                IntPtr @base = process.Memory.Read<IntPtr>(process.Memory.ImageBase + WowBuildInfoX64.UIFrameBase);
                IntPtr currentFrame = process.Memory.Read<IntPtr>(@base + WowBuildInfoX64.UIFirstFrame);
                while (currentFrame != IntPtr.Zero)
                {
                    WoWUIFrame f = null;
                    bool shouldExit = false;
                    try
                    {
                        WoWUIFrame temp = new WoWUIFrame(currentFrame, process);
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
                            currentFrame = process.Memory.Read<IntPtr>(currentFrame + process.Memory.Read<int>(@base + WowBuildInfoX64.UINextFrame) + 8);
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
                    log.Error(string.Format("GetAllFrames exec time: {0}ms", stopwatch.ElapsedMilliseconds));
                }
            }
        }
        
    }
}
