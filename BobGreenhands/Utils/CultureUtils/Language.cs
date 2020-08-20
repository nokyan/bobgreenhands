using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System;
using Nez.Persistence;
using NLog;


namespace BobGreenhands.Utils.CultureUtils
{
    /// <summary>
    /// Provisory localization """framework""" until I've figured out how Monogame and .resxs works with that.
    /// </summary>
    public sealed class Language
    {

        public static Dictionary<string, string> LanguageDict
        {
            get;
            private set;
        }

        public static CultureInfo CultureInfo
        {
            get;
            private set;
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static readonly string LanguageFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Content", "lang"));

        /// <summary>
        /// Call this when changing the language or starting the game.
        /// </summary>
        public static void Initialize(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
            // automatically assume that en_US is always complete. because it is.
            string en_US_lang = File.ReadAllText(Path.Combine(LanguageFolder, "en-US.json"));
            // create a dictionary with String → String and use Nez's JSON library to deserialize en_US.json
            Dictionary<string, object> en_US_dict_temp = Json.FromJson(en_US_lang) as Dictionary<string, object>;
            Dictionary<string, string> en_US_dict = en_US_dict_temp.ToDictionary(k => k.Key, k => k.Value.ToString());
            if (cultureInfo.ToString() == "en_US")
            {
                // if the user-given language is English (US), our job is done
                LanguageDict = en_US_dict;
                return;
            }
            // but if it isn't, we now have to take the i18n file for the user-given language, "lay it on top of en_US", and set this as our supreme master god lang dictionary
            string customLanguage;
            try
            {
                customLanguage = File.ReadAllText(Path.Combine(LanguageFolder, cultureInfo.ToString() + ".json"));
            }
            catch (FileNotFoundException)
            {
                _log.Warn("Language file for " + cultureInfo.ToString() + " not found, using English (US) instead.");
                CultureInfo = new CultureInfo("en-US");
                LanguageDict = en_US_dict;
                return;
            }
            Dictionary<string, object> customLanguageDict_temp = Json.FromJson(customLanguage) as Dictionary<string, object>;
            Dictionary<string, string> customLanguageDict = customLanguageDict_temp.ToDictionary(k => k.Key, k => k.Value.ToString());
            // finalDict is the dictionary that we will return
            Dictionary<string, string> finalDict = new Dictionary<string, string>();
            foreach (string key in en_US_dict.Keys.ToArray())
            {
                if (customLanguageDict.ContainsKey(key))
                {
                    finalDict.Add(key, customLanguageDict[key]);
                }
                else
                {
                    finalDict.Add(key, en_US_dict[key]);
                }
            }
            LanguageDict = finalDict;
            _log.Info("Finished initializing language " + cultureInfo.ToString());
        }

        /// <summary>
        /// Translate the given translatable string into either English (US), the user-given language or the translatable string because the translatable is invalid.
        /// </summary>
        public static string Translate(string translatable, string[] strings = null)
        {
            if (LanguageDict.ContainsKey(translatable))
            {
                // if the additional string array is null, don't use String.Format()
                return strings == null ? LanguageDict[translatable] : String.Format(LanguageDict[translatable], strings);
            }
            else
            {
                return translatable;
            };
        }

        public static string Translate(string translatable, string language, string[] strings = null)
        {
            string lang = File.ReadAllText(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Content", "lang", language + ".json")));
            Dictionary<string, object> langDict_tmp = Json.FromJson(lang) as Dictionary<string, object>;
            Dictionary<string, string> langDict = langDict_tmp.ToDictionary(k => k.Key, k => k.Value.ToString());
            if (langDict.ContainsKey(translatable))
            {
                // if the additional string array is null, don't use String.Format()
                return strings == null ? langDict[translatable] : String.Format(langDict[translatable], strings);
            }
            else
            {
                return translatable;
            };
        }
    }
}