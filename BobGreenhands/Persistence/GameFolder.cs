using System;
using System.Collections.Generic;
using System.IO;
using NLog;


namespace BobGreenhands.Persistence
{

    /// <summary>
    /// Represents the game's main folder (duh). Containes save files, executables and settings.
    /// </summary>
    public class GameFolder
    {

        public Settings Settings
        {
            get;
            private set;
        }

        public List<Savegame> Savegames = new List<Savegame>();
        
        public string FolderPath
        {
            get;
            private set;
        }

        public string LogFolder
        {
            get;
            private set;
        }

        public string SavegamesFolder
        {
            get;
            private set;
        }

        public string CrashlogsFolder
        {
            get;
            private set;
        }

        public string ScreenshotsFolder
        {
            get;
            private set;
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public GameFolder(string path)
        {
            FolderPath = path;
            Directory.CreateDirectory(FolderPath);
            LogFolder = Path.Combine(path, "logs");
            Directory.CreateDirectory(LogFolder);
            SavegamesFolder = Path.Combine(path, "savegames");
            Directory.CreateDirectory(SavegamesFolder);
            CrashlogsFolder = Path.Combine(path, "crashlogs");
            Directory.CreateDirectory(CrashlogsFolder);
            ScreenshotsFolder = Path.Combine(path, "screenshots");
            Directory.CreateDirectory(ScreenshotsFolder);
            Settings = new Settings(this);
            RefreshSavegameList();
        }

        public void RefreshSavegameList()
        {
            Savegames.Clear();
            // find all the savegames
            foreach (string s in Directory.GetFiles(SavegamesFolder, "*.bgs", SearchOption.TopDirectoryOnly))
            {
                _log.Info(String.Format("Found a savegame: {0}", s));
                Savegames.Add(new Savegame(s));
            }
            Savegames.Sort();
        }
    }
}