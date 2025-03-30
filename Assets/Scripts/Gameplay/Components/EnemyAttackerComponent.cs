using Game.Gameplay.Shooting;
using UnityEngine;

namespace Game.Gameplay.Components
{
    [DisallowMultipleComponent]
    public class EnemyAttackerComponent : MonoBehaviour
    {
        // Animator IDs
        private static readonly int _attackID = Animator.StringToHash("Attack");
        private static readonly int _gotHitID = Animator.StringToHash("GotHit");
        [SerializeField] private Animator animator;
        [SerializeField] private BulletShooter shooter;
        [SerializeField] private TargetContainerComponent targetContainer;
        [SerializeField] private EnemyLocomotionComponent locomotion;

        [Header("Combat Settings")] [SerializeField]
        private int attackDamage = 10;

        [SerializeField] private float attackCooldown = 2f;

        private float _lastAttackTime;

        private void Update()
        {
            if (!targetContainer.Target)
            {
                return;
            }

            if (locomotion.IsInAttackRange() && Time.time >= _lastAttackTime + attackCooldown)
            {
                AttackTarget();
            }
        }

        public void AttackTarget()
        {
            if (!targetContainer.Target)
            {
                return;
            }

            // Always face the target during attack
            locomotion.FaceTarget();

            animator.SetTrigger(_attackID);
            _lastAttackTime = Time.time;
        }

        public void TriggerHitAnimation()
        {
            animator.SetTrigger(_gotHitID);
            locomotion.SetHasSeenTarget(true);
        }

        // this is called by the trigger, so don't omit ;)
        public void EmitAttackParticle()
        {
            if (!targetContainer.Target)
            {
                return;
            }

            shooter.Shoot(new ShootingArgs
            {
                targetPos = targetContainer.Target.position,
                originObject = gameObject,
                damage = attackDamage
            });
        }
    }
}
