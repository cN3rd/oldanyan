using System;
using Game.NPCs;
using UnityEngine;

namespace Game.Gameplay.Shooting
{
    [RequireComponent(typeof(Rigidbody))]
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
        [HideInInspector] public AudioSource audioSource;

        private Transform _cachedTransform;
        private Rigidbody _rb;
        private bool _hasHit;

        private void Awake()
        {
            _cachedTransform = transform;
            _rb = GetComponent<Rigidbody>();

            // Configure rigidbody for optimal bullet physics
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;

            // Set initial velocity
            _rb.linearVelocity = _cachedTransform.forward * speed;

            // Auto-destroy after 5 seconds
            Destroy(gameObject, 5f);

            // Ignore collision with origin
            if (origin != null)
            {
                Collider bulletCollider = GetComponent<Collider>();
                Collider originCollider = origin.GetComponent<Collider>();
                if (bulletCollider != null && originCollider != null)
                {
                    Physics.IgnoreCollision(bulletCollider, originCollider, true);
                }
            }
        }

        private void FixedUpdate()
        {
            // Only handle targeting in FixedUpdate
            if (_hasHit || !isTargeting || !target) return;

            // Calculate direction to target
            Vector3 dirToTarget = (target.position - _cachedTransform.position).normalized;

            // Gradually rotate toward target
            Quaternion targetRotation = Quaternion.LookRotation(dirToTarget);
            _cachedTransform.rotation = Quaternion.Slerp(
                _cachedTransform.rotation,
                targetRotation,
                rotSpeed * Time.fixedDeltaTime
            );

            // Update linearVelocity based on new forward direction
            _rb.linearVelocity = _cachedTransform.forward * speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasHit) return;

            // Skip collision with origin (redundant safety check)
            if (collision.gameObject == origin) return;

            // Only process collisions with valid targets
            if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Player")) return;

            _hasHit = true;

            // Get the contact point
            Vector3 hitPoint = collision.contacts[0].point;

            // Process damage
            if (collision.gameObject.TryGetComponent(out PlayerController playerController))
                playerController.TakeDamage(10);
            else if (collision.gameObject.TryGetComponent(out EnemyBehaviour enemyBehaviour))
                enemyBehaviour.TakeDamage(100);

            // Spawn effects
            if (useHitPrefab && onHitEffectPrefab)
                Instantiate(onHitEffectPrefab, hitPoint, Quaternion.identity);
            else if (onHitEffect)
                Instantiate(onHitEffect, hitPoint, Quaternion.identity);

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}
