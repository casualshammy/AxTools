using AxTools.WoW.Helpers;
using System;
using System.Drawing;

namespace AxTools.WoW.PluginSystem
{
    public interface IPlugin3
    {
        /// <summary>
        ///
        /// </summary>
        bool ConfigAvailable { get; }

        /// <summary>
        /// Array of plugin names required for this plugin to work
        /// </summary>
        string[] Dependencies { get; }

        /// <summary>
        ///     Description to display on the plugin interface
        /// </summary>
        string Description { get; }

        /// <summary>
        ///
        /// </summary>
        bool DontCloseOnWowShutdown { get; }

        /// <summary>
        ///     The name of the plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Icon for tray context menu
        /// </summary>
        Image TrayIcon { get; }

        /// <summary>
        ///     Current version of this plugin
        /// </summary>
        Version Version { get; }

        /// <summary>
        ///     Work or configuration windows to display when the user presses the "config" button
        /// </summary>
        void OnConfig();

        /// <summary>
        ///     Work to be done when the plugin is loaded by the bot on startup
        /// </summary>
        void OnStart(GameInterface game);

        /// <summary>
        ///     Work to be done when the bot is shutdown/closed
        /// </summary>
        void OnStop();
    }
}