using System;
using System.IO;
using UnityEngine;

namespace Game.Data
{
    public sealed class GamePreferencesManager
    {
        private static readonly Lazy<GamePreferencesManager> _lazy =
            new(() => new GamePreferencesManager());

        private GamePreferences _preferences;

        private string _prefsPath;
        private GamePreferencesManager() => ReadPreferences();

        public static GamePreferencesManager Instance => _lazy.Value;

        private void ReadPreferences()
        {
            _prefsPath = Application.persistentDataPath + "/game_prefs.json";
            string prefsData = File.Exists(_prefsPath) ? File.ReadAllText(_prefsPath) : string.Empty;
            _preferences = JsonUtility.FromJson<GamePreferences>(prefsData) ?? new GamePreferences();
        }

        public GameSaveManager GetManagerForCurrentSave() =>
            GameSaveManager.FromSaveSlot(_preferences.lastUsedSlot);

        public void UpdateGamePreferences() =>
            File.WriteAllText(_prefsPath, JsonUtility.ToJson(_preferences));
    }
}
