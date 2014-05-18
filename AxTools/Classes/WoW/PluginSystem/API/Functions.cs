namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class Functions
    {
        public static bool Interact(ulong guid)
        {
            return WoW.Interact(guid);
        }

        public static bool MoveTo(WowPoint point)
        {
            return WoW.MoveTo(point);
        }
    }
}
