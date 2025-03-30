using System;
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

        [NonSerialized] public GameObject origin;

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
            if (isTargeting && target)
            {
                cachedTransform.forward = Vector3.RotateTowards(cachedTransform.forward,
                    target.position - cachedTransform.position, rotSpeed * Time.deltaTime, 0.0f);
            }

            cachedTransform.Translate(Vector3.forward * (speed * Time.deltaTime), Space.Self);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy") || !other.CompareTag("Player")) return;
            if (other.gameObject == origin) return;

            Debug.Log("Actual hit!");

            if (useHitPrefab && onHitEffectPrefab != null)
            {
                Instantiate(onHitEffectPrefab, cachedTransform.position, Quaternion.identity);
            }
            else if (onHitEffect != null)
            {
                Instantiate(onHitEffect, cachedTransform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
