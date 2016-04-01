using System;
using System.Drawing;
using System.Windows.Forms;
using AxTools.WoW.Management.ObjectManager;
using AxTools.WoW.PluginSystem;
using AxTools.WoW.PluginSystem.API;

namespace InkCrafter
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

        public string WowIcon
        {
            get { return "Interface\\\\Icons\\\\inv_inscription_ink_starlight"; }
        }

        public bool ConfigAvailable
        {
            get { return true; }
        }

        #endregion

        #region Events

        public void OnConfig()
        {
            if (SettingsInstance == null)
            {
                SettingsInstance = this.LoadSettingsJSON<InkCrafterSettings>();
            }
            InkCrafterConfig.Open(SettingsInstance);
            this.SaveSettingsJSON(SettingsInstance);
        }

        public void OnStart()
        {
            SettingsInstance = this.LoadSettingsJSON<InkCrafterSettings>();
            GameFunctions.SendToChat(string.Format("/run _G[\"{0}\"] = {{}};", randomTableName));
            foreach (string ink in inks)
            {
                GameFunctions.SendToChat(string.Format("/run _G[\"{0}\"][\"{1}\"] = true;", randomTableName, ink));
            }

            //craft = "local t = {\r\n" +
            //        "    \"Чернила звездного света\",\r\n" +
            //        "    \"Чернила снов\",\r\n" +
            //        "    \"Чернила Преисподней\",\r\n" +
            //        "    \"Мрачно-коричневые чернила\",\r\n" +
            //        "    \"Чернила снегопада\",\r\n" +
            //        "    \"Чернила моря\",\r\n" +
            //        "    \"Чернила черного огня\",\r\n" +
            //        "    \"Астральные чернила\",\r\n" +
            //        "    \"Небесные чернила\",\r\n" +
            //        "    \"Мерцающие чернила\",\r\n" +
            //        "    \"Огненные чернила\",\r\n" +
            //        "    \"Астрономические чернила\",\r\n" +
            //        "    \"Королевские чернила\",\r\n" +
            //        "    \"Чернила Нефритового Пламени\",\r\n" +
            //        "    \"Чернила утренней звезды\",\r\n" +
            //        "    \"Чернила царя зверей\",\r\n" +
            //        "    \"Чернила охотника\",\r\n" +
            //        "    \"Полуночные чернила\",\r\n" +
            //        "    \"Чернила лунного сияния\",\r\n" +
            //        "    \"Бежевые чернила\",\r\n" +
            //        "    \"Великолепная шкура\",\r\n" +
            //        "    \"Толстая борейская кожа\",\r\n" +
            //        "    \"Чернила разжигателя войны\",\r\n" +
            //        "};\r\n" +
            //        "local numTradeSkills = GetNumTradeSkills();\r\n" +
            //        "if (numTradeSkills > 0) then\r\n" +
            //        "    for i = 1, numTradeSkills do\r\n" +
            //        "        local skillName, skillType, numAvailable, isExpanded, serviceType, numSkillUps, indentLevel, showProgressBar, currentRank, maxRank, startingRank = GetTradeSkillInfo(i);\r\n" +
            //        "        if (tContains(t, skillName) and numAvailable > 0) then\r\n" +
            //        "			if (skillName == \"Чернила разжигателя войны\") then\r\n" +
            //        "				if (numAvailable > " + (SettingsInstance.WarbindersInkCount + 1) + ") then\r\n" + // +1 is neccessary, don't touch
            //        "        			DoTradeSkill(i, numAvailable - " + (SettingsInstance.WarbindersInkCount + 1) + ");\r\n" + // +1 is neccessary, don't touch
            //        "        			return;\r\n" +
            //        "				end\r\n" +
            //        "			else\r\n" +
            //        "        		DoTradeSkill(i, numAvailable);\r\n" +
            //        "        		return;\r\n" +
            //        "			end\r\n" +
            //        "        end\r\n" +
            //        "    end\r\n" +
            //        "end\r\n" +
            //        "print(\"Nothing to craft!\");";
            (timer = this.CreateTimer(1000, OnPulse)).Start();
        }

        public void OnPulse()
        {
            WoWPlayerMe me = ObjMgr.Pulse();
            if (me.CastingSpellID == 0 && me.ChannelSpellID == 0)
            {
                GameFunctions.SendToChat("/run local o=GetNumTradeSkills();if(o>0)then for i=1,o do local n,_,a=GetTradeSkillInfo(i);if(_G[\"" + randomTableName + "\"][n] and a>0)then DoTradeSkill(i,a);return;end end end");
            }
        }

        public void OnStop()
        {
            timer.Dispose();
        }

        #endregion

        private SingleThreadTimer timer;
        internal InkCrafterSettings SettingsInstance;
        private readonly string randomTableName = Utilities.GetRandomString(6);

        private readonly string[] inks =
        {
            "Чернила звездного света",
            "Чернила снов",
            "Чернила Преисподней",
            "Мрачно-коричневые чернила",
            //"Чернила снегопада",
            "Чернила моря",
            "Чернила черного огня",
            "Астральные чернила",
            //"Небесные чернила",
            "Мерцающие чернила",
            "Огненные чернила",
            "Астрономические чернила",
            //"Королевские чернила",
            "Чернила Нефритового Пламени",
            //"Чернила утренней звезды",
            "Чернила царя зверей",
            //"Чернила охотника",
            "Полуночные чернила",
            "Чернила лунного сияния",
            "Бежевые чернила",
            "Великолепная шкура",
            "Толстая борейская кожа",
            //"Чернила разжигателя войны",
        };

    }
}
