using UnityEngine;
using UnityEngine.AI;

namespace Game.Gameplay.Components
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class EnemyLocomotionComponent : MonoBehaviour
    {
        private static readonly int _speedID = Animator.StringToHash("Speed");

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private TargetContainerComponent targetContainer;

        [Header("Detection Settings")] [SerializeField]
        private float detectionRange = 50f;

        [SerializeField] private bool chaseOnSpawn = true;

        private bool _hasSeenTarget;
        private bool _isChasing;

        private void Start()
        {
            if (chaseOnSpawn && targetContainer.Target != null)
            {
                _hasSeenTarget = true;
                StartChasing();
            }
        }

        private void Update()
        {
            if (!targetContainer.Target)
            {
                StopChasing();
                return;
            }

            float distanceToTarget =
                Vector3.Distance(transform.position, targetContainer.Target.position);

            if (distanceToTarget <= detectionRange)
            {
                _hasSeenTarget = true;
            }

            if (_hasSeenTarget && !_isChasing && distanceToTarget > agent.stoppingDistance)
            {
                StartChasing();
            }
            else if (_isChasing && distanceToTarget <= agent.stoppingDistance)
            {
                StopChasing();
            }

            UpdateAnimator();
        }

        private void OnEnable()
        {
            if (_hasSeenTarget && targetContainer.Target != null)
            {
                StartChasing();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }

        public void StartChasing()
        {
            if (!targetContainer.Target)
            {
                return;
            }

            _isChasing = true;
            agent.isStopped = false;
            agent.SetDestination(targetContainer.Target.position);
        }

        public void StopChasing()
        {
            _isChasing = false;
            agent.isStopped = true;
        }

        public void FaceTarget()
        {
            if (!targetContainer.Target)
            {
                return;
            }

            var direction = targetContainer.Target.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 10f);
        }

        private void UpdateAnimator()
        {
            float currentSpeed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat(_speedID, currentSpeed);
        }

        // This should be called from Update loop when checking if target is in attacking range
        public bool IsInAttackRange()
        {
            if (!targetContainer.Target)
            {
                return false;
            }

            float distanceToTarget =
                Vector3.Distance(transform.position, targetContainer.Target.position);

            return distanceToTarget <= agent.stoppingDistance;
        }

        public bool HasSeenTarget() => _hasSeenTarget;

        public void SetHasSeenTarget(bool value) => _hasSeenTarget = value;
    }
}
