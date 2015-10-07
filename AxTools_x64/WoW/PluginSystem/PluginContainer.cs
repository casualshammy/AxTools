namespace AxTools.WoW.PluginSystem
{
    internal class PluginContainer
    {
        internal IPlugin Plugin;
        internal bool EnabledByUser;
        internal bool IsRunning;

        internal PluginContainer(IPlugin plugin, bool enabled)
        {
            Plugin = plugin;
            EnabledByUser = enabled;
            IsRunning = false;
        }

    }
}
