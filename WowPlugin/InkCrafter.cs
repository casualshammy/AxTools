using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.Classes.WoW.PluginSystem;
using AxTools.Classes.WoW.PluginSystem.API;

namespace TestPlugin
{
    public class InkCrafter : IPlugin
    {

        #region Info

        public string Name
        {
            get { return "InkCrafter"; }
        }

        public Version Version
        {
            get { return new Version(1, 0); }
        }

        public string Author
        {
            get { return "CasualShammy"; }
        }

        public string Description
        {
            get
            {
                return "Crafts all inks";
            }
        }

        private Image trayIcon;
        public Image TrayIcon
        {
            get { return trayIcon ?? (trayIcon = new Bitmap(Application.StartupPath + "\\plugins\\InkCrafter\\inv_inscription_ink_starlight.jpg")); }
        }

        public int Interval
        {
            get { return 1000; }
        }

        public string WowIcon
        {
            get { return "Interface\\\\Icons\\\\inv_inscription_ink_starlight"; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            throw new NotImplementedException();
        }

        public void OnStart()
        {
            
        }

        public void OnPulse()
        {
            EnvironmentObjects.Pulse();
            if (EnvironmentObjects.Me.CastingSpellID == 0 && EnvironmentObjects.Me.ChannelSpellID == 0)
            {
                Lua.LuaDoString(craft);
            }
        }

        public void OnStop()
        {
            
        }

        #endregion

        private readonly string craft = "local t = {" +
                                        "    \"Чернила звездного света\"," +
                                        "    \"Чернила снов\"," +
                                        "    \"Чернила Преисподней\"," +
                                        "    \"Мрачно-коричневые чернила\"," +
                                        "    \"Чернила снегопада\"," +
                                        "    \"Чернила моря\"," +
                                        "    \"Чернила черного огня\"," +
                                        "    \"Астральные чернила\"," +
                                        "    \"Небесные чернила\"," +
                                        "    \"Мерцающие чернила\"," +
                                        "    \"Огненные чернила\"," +
                                        "    \"Астрономические чернила\"," +
                                        "    \"Королевские чернила\"," +
                                        "    \"Чернила Нефритового Пламени\"," +
                                        "    \"Чернила утренней звезды\"," +
                                        "    \"Чернила царя зверей\"," +
                                        "    \"Чернила охотника\"," +
                                        "    \"Полуночные чернила\"," +
                                        "    \"Чернила лунного сияния\"," +
                                        "    \"Бежевые чернила\"," +
                                        "    \"Великолепная шкура\"," +
                                        "}; " +
                                        "local numTradeSkills = GetNumTradeSkills(); " +
                                        "if (numTradeSkills > 0) then" +
                                        "    for i = 1, numTradeSkills do" +
                                        "        local skillName, skillType, numAvailable, isExpanded, serviceType, numSkillUps, indentLevel, showProgressBar, currentRank, maxRank, startingRank = GetTradeSkillInfo(i);" +
                                        "        if (tContains(t, skillName) and numAvailable > 0) then" +
                                        "            DoTradeSkill(i, numAvailable);" +
                                        "            return;" +
                                        "        end" +
                                        "    end" +
                                        "end " +
                                        "print(\"Nothing to craft!\");";

    }
}
