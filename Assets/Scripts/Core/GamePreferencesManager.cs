using System;
using System.IO;
using UnityEngine;
using Game.Data;

namespace Game.Core
{
    public sealed class GamePreferencesManager
    {
        private static readonly Lazy<GamePreferencesManager> _lazy =
            new(() => new GamePreferencesManager());

        private GamePreferences _preferences;

        private string _prefsPath;
        private GamePreferencesManager() => ReadPreferences();

        public static GamePreferencesManager Instance => _lazy.Value;

        public GamePreferences Preferences => _preferences;

        private void ReadPreferences()
        {
            _prefsPath = Application.persistentDataPath + "/game_prefs.json";
            string prefsData = File.Exists(_prefsPath) ? File.ReadAllText(_prefsPath) : string.Empty;
            _preferences = JsonUtility.FromJson<GamePreferences>(prefsData) ?? new GamePreferences();
        }

        public void UpdateGamePreferences() =>
            File.WriteAllText(_prefsPath, JsonUtility.ToJson(_preferences));
    }
}
