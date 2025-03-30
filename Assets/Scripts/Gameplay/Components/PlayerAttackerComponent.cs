using Game.Gameplay.Shooting;
using Game.Input;
using UnityEngine;

namespace Game.Gameplay.Components
{
    [DisallowMultipleComponent]
    public class PlayerAttackerComponent : MonoBehaviour
    {
        // Animator IDs
        private static readonly int _attackId = Animator.StringToHash("Attack");
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private InputState inputState;
        [SerializeField] private Transform playerPivotTransform;
        [SerializeField] private Transform handSocket;
        [SerializeField] private BulletShooter shooter;

        [Header("Combat")] [SerializeField] private int attackDamage = 10;

        private Collider[] _hitColliders;

        private PickableWand _wand;

        private void Awake() => _hitColliders = new Collider[10];

        private void OnEnable()
        {
            if (inputState != null)
            {
                inputState.OnAttack += DoAttack;
            }
        }

        private void OnDisable()
        {
            if (inputState != null)
            {
                inputState.OnAttack -= DoAttack;
            }
        }

        public void OnDrawGizmos()
        {
            if (_wand)
            {
                Gizmos.DrawLine(_wand.ShootPoint.position, playerPivotTransform.forward * 100);
            }
        }

        public void AttachWand(PickableWand pickableWand)
        {
            if (_wand)
            {
                Debug.Log("Cannot take another wand... for now");
                return;
            }

            // Adhere to socket entirely
            pickableWand.transform.SetParent(handSocket);
            pickableWand.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            _wand = pickableWand;
        }

        public void DoAttack()
        {
            if (!_wand)
            {
                Debug.Log("Cannot attack without a wand");
                return;
            }

            Debug.Log("Starting attack...");
            characterAnimator.SetTrigger(_attackId);
        }

        public void EmitAttackParticle()
        {
            Debug.Log("Attacking...");
            shooter.startNodeTrans = _wand.ShootPoint;
            var enemyPosition = playerPivotTransform.forward * 100f;

            var enemyToAttack = LocateClosestEnemy();
            if (enemyToAttack)
            {
                enemyPosition = enemyToAttack.transform.position;
            }

            shooter.Shoot(new ShootingArgs
            {
                targetPos = enemyPosition, originObject = gameObject, damage = attackDamage
            });
        }

        public GameObject LocateClosestEnemy()
        {
            const float DetectionRadius = 5f;
            var lookDirection = playerPivotTransform.forward;

            int numFoundColliders =
                Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, _hitColliders);

            GameObject closestEnemy = null;
            float bestAlignmentScore = -1f;
            for (int index = 0; index < numFoundColliders; index++)
            {
                var potentialCollider = _hitColliders[index];
                if (!potentialCollider.gameObject.CompareTag("Enemy"))
                {
                    continue;
                }

                // Calculate direction to this enemy
                var directionToEnemy =
                    potentialCollider.transform.position - playerPivotTransform.position;

                float alignmentScore =
                    Vector3.Dot(lookDirection.normalized, directionToEnemy.normalized);

                // If this score is better than our current best, update
                if (alignmentScore > bestAlignmentScore)
                {
                    bestAlignmentScore = alignmentScore;
                    closestEnemy = potentialCollider.gameObject;
                }
            }

            return closestEnemy;
        }
    }
}
