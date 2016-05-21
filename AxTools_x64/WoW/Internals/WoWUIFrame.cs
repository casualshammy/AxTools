using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AxTools.Helpers;

namespace AxTools.WoW.Internals
{
    public class WoWUIFrame
    {
        private readonly IntPtr Address;
        private string cachedName;

        internal WoWUIFrame(IntPtr address)
        {
            Address = address;
        }

        public string GetName
        {
            get
            {
                if (cachedName == null)
                {
                    byte[] bytes = WoWManager.WoWProcess.Memory.ReadBytes(WoWManager.WoWProcess.Memory.Read<IntPtr>(Address + WowBuildInfoX64.UIFrameName), 132 * 2);
                    cachedName = Encoding.UTF8.GetString(bytes.TakeWhile(l => l != 0).ToArray());
                }
                return cachedName;
            }
        }

        public bool IsVisible
        {
            get { return ((WoWManager.WoWProcess.Memory.Read<int>(Address + WowBuildInfoX64.UIFrameVisible) >> WowBuildInfoX64.UIFrameVisible1) & WowBuildInfoX64.UIFrameVisible2) == 1; }
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
                        Log.Info(string.Format("WoWUIFrame.GetFrameByName exec time: {0}ms", stopwatch.ElapsedMilliseconds));
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
            Log.Info(string.Format("WoWUIFrame.GetFrameByName exec time: {0}ms", stopwatch.ElapsedMilliseconds));
            return null;
        }
    }
}
