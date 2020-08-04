using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Compopulate
{
    public class SettingsObject
    {
        public bool checkBeforePlay = false;
        public bool warnIfNull = false;
    }

    public static class SettingsHandler
    {
        const string settingsPath = "UserSettings/Compopulate.settings";
        public static SettingsObject Load()
        {
            if (!File.Exists(settingsPath))
            {
                if (!Directory.Exists("UserSettings/"))
                    Directory.CreateDirectory("UserSettings/");

                File.WriteAllText(settingsPath, JsonUtility.ToJson(new SettingsObject()));
            }

            SettingsObject settings = null;

            try
            {
                settings = JsonUtility.FromJson<SettingsObject>(File.ReadAllText(settingsPath));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Weren't able to parse Compoopulate user settings at {settingsPath}.\n{ex}");
            }

            return settings;
        }

        public static void Write(SettingsObject settings)
        {
            if (!File.Exists(settingsPath))
            {
                if (!Directory.Exists("UserSettings/"))
                    Directory.CreateDirectory("UserSettings/");
            }

            File.WriteAllText(settingsPath, JsonUtility.ToJson(settings));
        }
    }
}