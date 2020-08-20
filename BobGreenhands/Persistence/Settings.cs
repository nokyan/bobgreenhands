using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using Nez.Persistence;


namespace BobGreenhands.Persistence
{
    /// <settings>
    /// An abstraction layer to basically just a JSON file. Said JSON file contains all the global game settings such as fullscreen mode, resolution, sound settings etc.
    /// </settings>
    public class Settings
    {

        public static readonly string SettingsFilename = "settings.json";

        public static readonly Dictionary<string, object> FallbackDictionary = new Dictionary<string, object>() {
            {"width", 1280},
            {"height", 720},
            {"fullscreen", false},
            {"vsync", true},
            {"language", System.Globalization.CultureInfo.CurrentCulture.ToString()},
            {"discordRpc", true},
            {"totalVolume", 1.0},
            {"musicVolume", 0.4},
            {"soundVolume", 0.9},
            {"vignette", true},
        };

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private Dictionary<string, object> _settingsDictionary = new Dictionary<string, object>();

        private string _settingsPath;

        private GameFolder _gameFolder;

        public Settings(GameFolder gameFolder)
        {
            _gameFolder = gameFolder;
            _settingsPath = Path.Combine(_gameFolder.FolderPath, SettingsFilename);
            if (File.Exists(_settingsPath))
            {
                _settingsDictionary = Json.FromJson(File.ReadAllText(_settingsPath)) as Dictionary<string, object>;
            }
            else
            {
                _settingsDictionary = FallbackDictionary;
                try
                {
                    // save the fallback settings into the new settings file
                    File.WriteAllText(_settingsPath, Json.ToJson(FallbackDictionary));
                    _log.Info("Created a fresh settings.json");
                }
                catch (Exception e)
                {
                    _log.Error(e, "Unable to create settings file! Using fallback settings!");
                }
            }
        }

        public object Get(string setting)
        {
            if (_settingsDictionary.ContainsKey(setting))
            {
                return _settingsDictionary[setting];
            }
            else
            {
                if (FallbackDictionary.ContainsKey(setting))
                {
                    _log.Warn(String.Format("Returning fallback value for {0}", setting));
                    return FallbackDictionary[setting];
                }
                else
                {
                    throw new ArgumentException(String.Format("Invalid setting {0}", setting));
                }
            }
        }

        public int GetInt(string setting)
        {
            return Convert.ToInt32(Get(setting));
        }

        public short GetShort(string setting)
        {
            return Convert.ToInt16(Get(setting));
        }

        public char GetChar(string setting)
        {
            return Convert.ToChar(Get(setting));
        }

        public float GetFloat(string setting)
        {
            return Convert.ToSingle(Get(setting));
        }

        public double GetDouble(string setting)
        {
            return Convert.ToDouble(Get(setting));
        }

        public bool GetBool(string setting)
        {
            return Convert.ToBoolean(Get(setting));
        }

        public byte GetByte(string setting)
        {
            return Convert.ToByte(Get(setting));
        }

        public long GetLong(string setting)
        {
            return Convert.ToInt64(Get(setting));
        }

        public string? GetString(string setting)
        {
            return Convert.ToString(Get(setting));    
        }

        public void Set(string setting, object value)
        {
            _settingsDictionary[setting] = value;
        }

        // TODO: maybe async?
        public void Save()
        {
            File.WriteAllText(_settingsPath, Json.ToJson(_settingsDictionary, true));
        }
    }
}