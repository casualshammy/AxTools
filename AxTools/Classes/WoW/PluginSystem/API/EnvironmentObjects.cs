using System.Collections.Generic;

namespace AxTools.Classes.WoW.PluginSystem.API
{
    public static class EnvironmentObjects
    {

        #region Pulse()

        public static void Pulse(List<WowObject> wowObjects, List<WowPlayer> wowUnits, List<WowNpc> wowNpcs)
        {
            WoW.Pulse(wowObjects, wowUnits, wowNpcs);
        }

        public static void Pulse(List<WowObject> wowObjects, List<WowNpc> wowNpcs)
        {
            WoW.Pulse(wowObjects, wowNpcs);
        }

        public static void Pulse(List<WowObject> wowObjects)
        {
            WoW.Pulse(wowObjects);
        }

        public static void Pulse(List<WowPlayer> wowPlayers)
        {
            WoW.Pulse(wowPlayers);
        }

        public static void Pulse()
        {
            WoW.Pulse();
            PlayerMe.GUID = Classes.WoW.LocalPlayer.GUID;
            PlayerMe.Location = Classes.WoW.LocalPlayer.Location;
            PlayerMe.Rotation = Classes.WoW.LocalPlayer.Rotation;
            PlayerMe.CastingSpellID = Classes.WoW.LocalPlayer.CastingSpellID;
            PlayerMe.ChannelSpellID = Classes.WoW.LocalPlayer.ChannelSpellID;
            PlayerMe.Health = Classes.WoW.LocalPlayer.Health;
            PlayerMe.HealthMax = Classes.WoW.LocalPlayer.HealthMax;
            PlayerMe.Level = Classes.WoW.LocalPlayer.Level;
            PlayerMe.TargetGUID = Classes.WoW.LocalPlayer.TargetGUID;
            PlayerMe.IsAlliance = Classes.WoW.LocalPlayer.IsAlliance;
        }

        #endregion

        #region Local player

        private static readonly LocalPlayer PlayerMe = new LocalPlayer();

        /// <summary>
        /// You should call Pulse(void) overload to refresh local player info
        /// </summary>
        public static LocalPlayer Me
        {
            get
            {
                return PlayerMe;
            }
        }

        #endregion
    
    }
}
