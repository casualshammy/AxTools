namespace AxTools.WoW.PluginSystem
{
    internal class PluginContainer
    {
        internal IPlugin3 Plugin;
        internal bool IsRunning;

        internal PluginContainer(IPlugin3 plugin)
        {
            Plugin = plugin;
            IsRunning = false;
        }

    }
}
