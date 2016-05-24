using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxTools.WoW.Helpers;
using AxTools.WoW.Internals;
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
            startupTask = Task.Run(() =>
            {
                this.ShowNotify("Please wait while plugin set up inks lists...", false, false);
                GameFunctions.SendToChat(string.Format("/run {0} = {{}};", tableNames));
                GameFunctions.SendToChat(string.Format("/run {0} = {{}};", tableAvailable));
                GameFunctions.SendToChat(string.Format("/run {0} = {{}};", tableIndexes));
                GameFunctions.SendToChat(string.Format("/run {0} = {{}};", tableRemain));
                foreach (string ink in inks)
                {
                    GameFunctions.SendToChat(string.Format("/run {0}[\"{1}\"] = true;", tableNames, ink));
                    GameFunctions.SendToChat(string.Format("/run {0}[\"{1}\"] = {2};", tableRemain, ink, ink == "Чернила разжигателя войны" ? SettingsInstance.WarbindersInkCount : 0));
                }
                this.ShowNotify("Completed, starting to craft", false, false);
                (timer = this.CreateTimer(1000, OnPulse)).Start();
            });
        }

        public void OnPulse()
        {
            WoWPlayerMe me = ObjMgr.Pulse();
            if (me != null && me.CastingSpellID == 0 && me.ChannelSpellID == 0)
            {
                GameFunctions.SendToChat(string.Format("/run local o=GetNumTradeSkills();if(o>0)then for i=1,o do local n,_,a=GetTradeSkillInfo(i);if({0}[n])then {1}[n]=a;{2}[n]=i;end end end", tableNames, tableAvailable, tableIndexes));
                GameFunctions.SendToChat(string.Format("/run for i in pairs({0})do if({1}[i])then local a={1}[i]-{2}[i];if(a>0)then DoTradeSkill({3}[i],a);return;end end end", tableNames, tableAvailable, tableRemain, tableIndexes));
            }
            else
            {
                Thread.Sleep(500);
            }
        }

        public void OnStop()
        {
            startupTask.Wait();
            startupTask.Dispose();
            timer.Dispose();
        }

        #endregion

        private SafeTimer timer;
        internal InkCrafterSettings SettingsInstance;
        private readonly string tableNames = string.Format("_G[\"{0}\"]", Utilities.GetRandomString(6));
        private readonly string tableAvailable = string.Format("_G[\"{0}\"]", Utilities.GetRandomString(6));
        private readonly string tableIndexes = string.Format("_G[\"{0}\"]", Utilities.GetRandomString(6));
        private readonly string tableRemain = string.Format("_G[\"{0}\"]", Utilities.GetRandomString(6));
        private Task startupTask;

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
            "Чернила разжигателя войны",
        };

    }
}
