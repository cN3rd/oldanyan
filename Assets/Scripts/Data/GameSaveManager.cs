using System.IO;
using UnityEngine;

namespace Game.Data
{
    public class GameSaveManager
    {
        private readonly string _savePath;

        private GameSaveManager(GameSave save, string savePath)
        {
            ActualSave = save;
            _savePath = savePath;
        }

        public GameSave ActualSave { get; }

        public static GameSaveManager FromSaveSlot(int slot = 0)
        {
            string savePath = $"{Application.persistentDataPath}/gamesave_{slot}.save";
            GameSave save = null;
            if (File.Exists(savePath))
            {
                string saveText = File.ReadAllText(savePath);
                save = JsonUtility.FromJson<GameSave>(saveText);
            }

            save ??= new GameSave();
            return new GameSaveManager(save, savePath);
        }

        public void SaveGame() => File.WriteAllText(_savePath, JsonUtility.ToJson(ActualSave));
    }
}
