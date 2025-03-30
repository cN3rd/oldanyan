using UnityEngine;

namespace Game.Data
{
    public class BulletEffectPrefabs : ScriptableObject
    {
        public string effectName;
        public float speed;
        public bool isTargeting;
        public float rotSpeed;
        public float chargeParticleTime;

        public GameObject chargeParticlesPrefab;
        public GameObject startParticlesPrefab;
        public GameObject bulletParticlesPrefab;
        public GameObject hitParticlesPrefab;

        public AudioClip chargeClip;
        public AudioClip startClip;
        public AudioClip bulletClip;
        public AudioClip hitClip;
    }
}
