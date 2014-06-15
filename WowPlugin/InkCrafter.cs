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

        private readonly string craft = "local t = {\r\n" +
                                        "    \"Чернила звездного света\",\r\n" +
                                        "    \"Чернила снов\",\r\n" +
                                        "    \"Чернила Преисподней\",\r\n" +
                                        "    \"Мрачно-коричневые чернила\",\r\n" +
                                        "    \"Чернила снегопада\",\r\n" +
                                        "    \"Чернила моря\",\r\n" +
                                        "    \"Чернила черного огня\",\r\n" +
                                        "    \"Астральные чернила\",\r\n" +
                                        "    \"Небесные чернила\",\r\n" +
                                        "    \"Мерцающие чернила\",\r\n" +
                                        "    \"Огненные чернила\",\r\n" +
                                        "    \"Астрономические чернила\",\r\n" +
                                        "    \"Королевские чернила\",\r\n" +
                                        "    \"Чернила Нефритового Пламени\",\r\n" +
                                        "    \"Чернила утренней звезды\",\r\n" +
                                        "    \"Чернила царя зверей\",\r\n" +
                                        "    \"Чернила охотника\",\r\n" +
                                        "    \"Полуночные чернила\",\r\n" +
                                        "    \"Чернила лунного сияния\",\r\n" +
                                        "    \"Бежевые чернила\",\r\n" +
                                        "    \"Великолепная шкура\",\r\n" +
                                        "};\r\n" +
                                        "local numTradeSkills = GetNumTradeSkills();\r\n" +
                                        "if (numTradeSkills > 0) then\r\n" +
                                        "    for i = 1, numTradeSkills do\r\n" +
                                        "        local skillName, skillType, numAvailable, isExpanded, serviceType, numSkillUps, indentLevel, showProgressBar, currentRank, maxRank, startingRank = GetTradeSkillInfo(i);\r\n" +
                                        "        if (tContains(t, skillName) and numAvailable > 0) then\r\n" +
                                        "            DoTradeSkill(i, numAvailable);\r\n" +
                                        "            return;\r\n" +
                                        "        end\r\n" +
                                        "    end\r\n" +
                                        "end\r\n" +
                                        "print(\"Nothing to craft!\");";

    }
}
