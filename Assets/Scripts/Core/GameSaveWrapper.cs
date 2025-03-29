using System.IO;
using UnityEngine;
using Game.Data;

namespace Game.Core
{
    public class GameSaveWrapper
    {
        private readonly string _savePath;

        private GameSaveWrapper(GameSave save, string savePath)
        {
            ActualSave = save;
            _savePath = savePath;
        }

        public GameSave ActualSave { get; }

        public static GameSaveWrapper FromSaveSlot(int slot = 0)
        {
            string savePath = $"{Application.persistentDataPath}/gamesave_{slot}.save";
            GameSave save = null;
            if (File.Exists(savePath))
            {
                string saveText = File.ReadAllText(savePath);
                save = JsonUtility.FromJson<GameSave>(saveText);
            }

            save ??= new GameSave();
            return new GameSaveWrapper(save, savePath);
        }

        public static GameSaveWrapper CreateNewGame()
        {
            int slot = GamePreferencesManager.Instance.Preferences.lastUsedSlot++;
            GamePreferencesManager.Instance.UpdateGamePreferences();
            return FromSaveSlot(slot);
        }

        public void SaveGame() => File.WriteAllText(_savePath, JsonUtility.ToJson(ActualSave));
    }
}
