using Eflatun.SceneReference;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Static Data", menuName = "Game/Static Data", order = 1)]
    public class StaticGameData : ScriptableObject
    {
        public SceneReference mainMenuScene;
        public SceneReference[] levels;
    }
}
