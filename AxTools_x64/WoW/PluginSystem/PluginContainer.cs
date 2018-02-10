namespace AxTools.WoW.PluginSystem
{
    internal class PluginContainer
    {
        internal IPlugin2 Plugin;
        internal bool IsRunning;

        internal PluginContainer(IPlugin2 plugin)
        {
            Plugin = plugin;
            IsRunning = false;
        }

    }
}
