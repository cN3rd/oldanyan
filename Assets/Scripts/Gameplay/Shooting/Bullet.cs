using System;
using Game.NPCs;
using UnityEngine;

namespace Game.Gameplay.Shooting
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
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

        public int bulletDamage;

        [NonSerialized] public GameObject origin;
        [HideInInspector] public AudioSource audioSource;

        private Transform _cachedTransform;
        private Rigidbody _rb;
        private bool _hasHit;

        private void Awake()
        {
            _cachedTransform = transform;
            _rb = GetComponent<Rigidbody>();

            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.linearVelocity = _cachedTransform.forward * speed;

            Destroy(gameObject, 5f);

            // Ignore collision with origin
            if (origin != null)
            {
                // TODO: get rid of
                Collider bulletCollider = GetComponent<Collider>();
                Collider originCollider = origin.GetComponent<Collider>();
                if (bulletCollider && originCollider)
                {
                    Physics.IgnoreCollision(bulletCollider, originCollider, true);
                }
            }
        }

        private void FixedUpdate()
        {
            // Only handle targeting in FixedUpdate
            if (_hasHit || !isTargeting || !target) return;

            Vector3 dirToTarget = (target.position - _cachedTransform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dirToTarget);
            _cachedTransform.rotation = Quaternion.Slerp(
                _cachedTransform.rotation,
                targetRotation,
                rotSpeed * Time.fixedDeltaTime
            );

            _rb.linearVelocity = _cachedTransform.forward * speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasHit) return;
            if (collision.gameObject == origin) return;
            if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Player")) return;

            _hasHit = true;

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
