using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Tips", menuName = "Game/Tips Collection", order = 1)]
    public class GameTips : ScriptableObject
    {
        public string[] tips;
    }
}
