using System;
using UnityEngine;

namespace Game.Gameplay.Components
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(TargetContainerComponent))]
    [RequireComponent(typeof(EnemyLocomotionComponent))]
    [RequireComponent(typeof(EnemyAttackerComponent))]
    [DisallowMultipleComponent]
    public class EnemyController : MonoBehaviour
    {
        private static readonly int _dieID = Animator.StringToHash("Die");
        [SerializeField] private Animator animator;
        [SerializeField] private HealthComponent health;
        [SerializeField] private TargetContainerComponent targetContainer;
        [SerializeField] private EnemyLocomotionComponent locomotion;
        [SerializeField] private EnemyAttackerComponent attacker;
        [SerializeField] private AnimationClip deathAnimation;

        private void Awake()
        {
            health.OnDeath += HandleEnemyDeath;
            health.OnDamaged += HandleEnemyDamaged;
        }

        private void OnDestroy()
        {
            health.OnDeath -= HandleEnemyDeath;
            health.OnDamaged -= HandleEnemyDamaged;
        }

        public event Action<EnemyController> OnEnemyDeath;

        private void HandleEnemyDeath()
        {
            animator.SetTrigger(_dieID);

            locomotion.StopChasing();
            enabled = false;

            Destroy(gameObject, deathAnimation.length);
            OnEnemyDeath?.Invoke(this);
        }

        private void HandleEnemyDamaged(int damage) => attacker.TriggerHitAnimation();

        public void SetPlayerTarget(Transform target)
        {
            targetContainer.SetTarget(target);

            if (locomotion.HasSeenTarget() && !health.IsDead)
            {
                locomotion.StartChasing();
            }
        }
    }
}
