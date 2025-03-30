using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    // Collection of all bullet effect prefabs
    public class BulletEffectPrefabsCollection : ScriptableObject
    {
        public List<BulletEffectPrefabs> effectPrefabs = new();

        public BulletEffectPrefabs GetEffectByIndex(int index)
        {
            if (index >= 0 && index < effectPrefabs.Count)
            {
                return effectPrefabs[index];
            }

            return null;
        }
    }
}
