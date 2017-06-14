namespace AxTools.WoW.PluginSystem
{
    public interface IPlugin2 : IPlugin
    {

        /// <summary>
        /// Array of plugins names required for this plugin to work
        /// </summary>
        string[] Dependencies { get; }

    }
}
