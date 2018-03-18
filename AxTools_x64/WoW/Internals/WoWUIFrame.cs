using System;
using System.Collections.Concurrent;
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
        private static readonly Dictionary<int, ConcurrentDictionary<string, IntPtr>> cachedFrames = new Dictionary<int, ConcurrentDictionary<string, IntPtr>>();
        
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
        
        public static WoWUIFrame GetFrameByName(GameInterface game, string name)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (!cachedFrames.TryGetValue(game.wowProcess.ProcessID, out ConcurrentDictionary<string, IntPtr> dict))
                dict = (cachedFrames[game.wowProcess.ProcessID] = new ConcurrentDictionary<string, IntPtr>());
            WoWUIFrame frame = null;
            if (dict.TryGetValue(name, out IntPtr address))
            {
                try
                {
                    frame = new WoWUIFrame(address, game.wowProcess);
                    if (frame.GetName == name)
                        return frame;
                }
                catch { /* frame is moved in memory, ignore this exception and just rebuild the cache */ }
            }
            frame = GetAllFrames(game).FirstOrDefault(l => l.GetName == name);
            if (stopwatch.ElapsedMilliseconds > 500)
                log.Error(string.Format("GetFrameByName exec time: {0}ms", stopwatch.ElapsedMilliseconds));
            return frame;
        }
        
        private static IEnumerable<WoWUIFrame> GetAllFrames(GameInterface game)
        {
            if (game.IsInGame && game.wowProcess.IsValidBuild)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var dict = cachedFrames[game.wowProcess.ProcessID];
                IntPtr @base = game.wowProcess.Memory.Read<IntPtr>(game.wowProcess.Memory.ImageBase + WowBuildInfoX64.UIFrameBase);
                IntPtr currentFrame = game.wowProcess.Memory.Read<IntPtr>(@base + WowBuildInfoX64.UIFirstFrame);
                while (currentFrame != IntPtr.Zero)
                {
                    WoWUIFrame f = null;
                    bool shouldExit = false;
                    try
                    {
                        WoWUIFrame temp = new WoWUIFrame(currentFrame, game.wowProcess);
                        if (!string.IsNullOrWhiteSpace(temp.GetName))
                        {
                            dict.AddOrUpdate(temp.GetName, currentFrame, (key, oldValue) => currentFrame);
                            f = temp;
                        }
                    }
                    catch { }
                    finally
                    {
                        try
                        {
                            currentFrame = game.wowProcess.Memory.Read<IntPtr>(currentFrame + game.wowProcess.Memory.Read<int>(@base + WowBuildInfoX64.UINextFrame) + 8);
                        }
                        catch
                        {
                            shouldExit = true;
                        }
                    }
                    if (f != null)
                        yield return f;
                    if (shouldExit)
                        break;
                }
                if (stopwatch.ElapsedMilliseconds > 500)
                    log.Error(string.Format("GetAllFrames exec time: {0}ms", stopwatch.ElapsedMilliseconds));
            }
        }
        
    }
    
}
