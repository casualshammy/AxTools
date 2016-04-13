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
                GameFunctions.SendToChat(string.Format("/run _G[\"{0}\"] = {{}};", randomTableName));
                foreach (string ink in inks)
                {
                    GameFunctions.SendToChat(string.Format("/run _G[\"{0}\"][\"{1}\"] = true;", randomTableName, ink));
                }
                (timer = this.CreateTimer(1000, OnPulse)).Start();
            });
        }

        public void OnPulse()
        {
            WoWPlayerMe me = ObjMgr.Pulse();
            if (me.CastingSpellID == 0 && me.ChannelSpellID == 0)
            {
                GameFunctions.SendToChat("/run local o=GetNumTradeSkills();if(o>0)then for i=1,o do local n,_,a=GetTradeSkillInfo(i);if(_G[\"" + randomTableName + "\"][n] and a>0)then DoTradeSkill(i,a);return;end end end");
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

        private SingleThreadTimer timer;
        internal InkCrafterSettings SettingsInstance;
        private readonly string randomTableName = Utilities.GetRandomString(6);
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
            //"Чернила разжигателя войны",
        };

    }
}
