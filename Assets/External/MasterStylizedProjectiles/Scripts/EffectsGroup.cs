using UnityEngine;

namespace MasterStylizedProjectile
{
    [System.Serializable]
    public class EffectsGroup
    {
        public string EffectName;
        public float Speed = 20;
        public ParticleSystem ChargeParticles;
        public float ChargeParticleTime;
        public AudioClip ChargeClip;
        public ParticleSystem StartParticles;
        public ParticleSystem BulletParticles;
        public ParticleSystem HitParticles;
        public AudioClip startClip;
        public AudioClip bulletClip;
        public AudioClip hitClip;
        public bool isTargeting;
        public float RotSpeed;
    }
}
