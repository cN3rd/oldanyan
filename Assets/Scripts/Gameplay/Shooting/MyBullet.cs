using UnityEngine;

namespace Game.Gameplay.Shooting
{
    public class MyBullet : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float speed = 5;
        public bool isTargeting;
        public Transform target;
        public float rotSpeed;

        [Header("Effect References")]
        public ParticleSystem onHitEffect;
        public GameObject onHitEffectPrefab;
        public bool useHitPrefab = false;

        [Header("Audio Settings")]
        public AudioClip bulletClip;
        public AudioClip onHitClip;

        // Reference to the audio source (assigned in prefab)
        [HideInInspector] public AudioSource audioSource;

        // Cache transform for better performance
        private Transform cachedTransform;

        private void Awake()
        {
            // Cache transform reference
            cachedTransform = transform;
        }

        private void Update()
        {
            if (isTargeting && target != null)
            {
                cachedTransform.forward = Vector3.RotateTowards(cachedTransform.forward,
                    target.position - cachedTransform.position, rotSpeed * Time.deltaTime, 0.0f);
            }

            cachedTransform.Translate(Vector3.forward * (speed * Time.deltaTime), Space.Self);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Use the prefab-based approach
            if (useHitPrefab && onHitEffectPrefab != null)
            {
                Instantiate(onHitEffectPrefab, cachedTransform.position, Quaternion.identity);
            }
            // Fall back to original approach if prefab reference is missing
            else if (onHitEffect != null)
            {
                Instantiate(onHitEffect, cachedTransform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
