using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadMilkman.Ini;
using OnTheGoPlayer.Bl;

namespace OnTheGoPlayer.Helpers
{
    internal class Settings
    {
        #region Private Fields

        private static readonly IniFile iniFile = new IniFile();

        private static readonly string iniFilePath = Constants.GetDataPath("config.ini");

        #endregion Private Fields

        #region Public Constructors

        static Settings()
        {
            using (CultureScope.Create(CultureInfo.InvariantCulture))
            {
                if (!File.Exists(iniFilePath))
                {
                    iniFile.Sections.Add("Default").Serialize(new Settings());
                    iniFile.Save(iniFilePath);
                }

                try
                {
                    iniFile.Load(iniFilePath);
                    Default = iniFile.Sections["Default"].Deserialize<Settings>();
                }
                catch (Exception e)
                {
                    Debug.Fail(e.Message, e.ToString());
                }
            }
        }

        public Settings()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public static Settings Default { get; } = new Settings();

        public string LastLoadedPlaylistContainerFile { get; set; } = string.Empty;

        public RepeatMode RepeatMode { get; set; } = RepeatMode.Off;

        public bool Shuffle { get; set; } = false;

        public float Volume { get; set; } = 0.5f;

        #endregion Public Properties

        #region Public Methods

        public void Save()
        {
            using (CultureScope.Create(CultureInfo.InvariantCulture))
            {
                var iniSection = iniFile.Sections["Default"];
                iniSection.Keys.Clear();
                iniSection.Serialize(this);
                iniFile.Save(iniFilePath);
            }
        }

        #endregion Public Methods
    }
}