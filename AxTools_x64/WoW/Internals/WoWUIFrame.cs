using System;
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
                    byte[] bytes = WoWManager.WoWProcess.Memory.ReadBytes(WoWManager.WoWProcess.Memory.Read<IntPtr>(address + WowBuildInfoX64.UIEditBoxText), 255*2); // 255 - max string length; 2 - utf8 char length
                    cachedEditboxText = Encoding.UTF8.GetString(bytes.TakeWhile(l => l != 0).ToArray());
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
            IntPtr @base = WoWManager.WoWProcess.Memory.Read<IntPtr>(WoWManager.WoWProcess.Memory.ImageBase + WowBuildInfoX64.UIFrameBase);
            IntPtr currentFrame = WoWManager.WoWProcess.Memory.Read<IntPtr>(@base + WowBuildInfoX64.UIFirstFrame);
            while (currentFrame != IntPtr.Zero)
            {
                try
                {
                    WoWUIFrame f = new WoWUIFrame(currentFrame);
                    //File.AppendAllText(Application.StartupPath + "\\frames.txt", string.Format("New frame: {0}; Visible: {1}\r\n", f.GetName, f.IsVisible));
                    if (f.GetName == name)
                    {
                        if (stopwatch.ElapsedMilliseconds > 500)
                        {
                            Log.Error(string.Format("WoWUIFrame.GetFrameByName: frame name: {0}; search time: {1}ms", name, stopwatch.ElapsedMilliseconds));
                        }
                        return f;
                    }
                }
                catch
                {
                    //
                }
                finally
                {
                    currentFrame = WoWManager.WoWProcess.Memory.Read<IntPtr>(currentFrame + WoWManager.WoWProcess.Memory.Read<int>(@base + WowBuildInfoX64.UINextFrame) + 8);
                }
            }
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                Log.Error(string.Format("WoWUIFrame.GetFrameByName exec time: {0}ms", stopwatch.ElapsedMilliseconds));
            }
            return null;
        }
    
    }
}
