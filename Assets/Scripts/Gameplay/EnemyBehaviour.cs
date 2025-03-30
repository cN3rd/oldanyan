using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.NPCs
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class EnemyBehaviour : MonoBehaviour
    {
        private static readonly int _speedID = Animator.StringToHash("Speed");
        private static readonly int _attackID = Animator.StringToHash("Attack");
        private static readonly int _dieID = Animator.StringToHash("Die");
        private static readonly int _gotHitID = Animator.StringToHash("GotHit");

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;

        [Header("Target")] //
        [SerializeField] public Transform playerTransform;

        [Header("Detection Settings")] //
        [SerializeField] private float detectionRange = 50f;
        [SerializeField] private bool chaseOnSpawn = true;

        [Header("Combat Settings")] //
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private int maxHealth = 100;

        private int _currentHealth;
        private EnemyState _currentState = EnemyState.Idle;
        private bool _hasSeenPlayer;
        private bool _isDead;
        private float _lastAttackTime;

        private void Awake() => _currentHealth = maxHealth;

        private void Start()
        {
            if (chaseOnSpawn)
            {
                _currentState = EnemyState.Chasing;
                _hasSeenPlayer = true;
                ChasePlayer();
            }
        }

        private void Update()
        {
            if (_isDead || !playerTransform)
            {
                return;
            }

            UpdateState();
            UpdateBehavior();
            UpdateAnimator();
        }

        private void OnEnable()
        {
            if (_isDead || !playerTransform || !_hasSeenPlayer && !chaseOnSpawn)
            {
                return;
            }

            _currentState = EnemyState.Chasing;
            ChasePlayer();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }

        public event Action<EnemyBehaviour> OnEnemyDeath;

        public void SetPlayerTarget(Transform target)
        {
            playerTransform = target;

            if (_hasSeenPlayer && !_isDead)
            {
                _currentState = EnemyState.Chasing;
                ChasePlayer();
            }
        }

        private void UpdateState()
        {
            if (_currentHealth <= 0)
            {
                _currentState = EnemyState.Dead;
                return;
            }

            if (!playerTransform)
            {
                _currentState = EnemyState.Idle;
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= detectionRange)
            {
                _hasSeenPlayer = true;
            }

            if (_hasSeenPlayer)
            {
                _currentState = distanceToPlayer <= agent.stoppingDistance
                    ? EnemyState.Attacking
                    : EnemyState.Chasing;
            }
            else
            {
                _currentState = EnemyState.Idle;
            }
        }

        private void UpdateBehavior()
        {
            switch (_currentState)
            {
                case EnemyState.Idle:
                    agent.isStopped = true;
                    break;
                case EnemyState.Chasing:
                    ChasePlayer();
                    break;
                case EnemyState.Attacking:
                    AttackPlayer();
                    break;
                case EnemyState.Dead:
                    Die();
                    break;
            }
        }

        private void ChasePlayer()
        {
            if (!playerTransform)
            {
                return;
            }

            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }

        private void AttackPlayer()
        {
            // Always face the player during attack
            if (playerTransform)
            {
                var direction = playerTransform.position - transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction),
                    Time.deltaTime * 10f);
            }

            agent.isStopped = true;

            if (Time.time >= _lastAttackTime + attackCooldown)
            {
                animator.SetTrigger(_attackID);
                _lastAttackTime = Time.time;
            }
        }

        private void Die()
        {
            if (!_isDead)
            {
                _isDead = true;
                agent.isStopped = true;
                animator.SetTrigger(_dieID);

                OnEnemyDeath?.Invoke(this);
                enabled = false;
                Destroy(gameObject, 5f);
            }
        }

        private void UpdateAnimator()
        {
            float currentSpeed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat(_speedID, currentSpeed);
        }

        public void TakeDamage(int damage)
        {
            if (_isDead)
            {
                return;
            }

            _currentHealth -= damage;
            _hasSeenPlayer = true;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _currentState = EnemyState.Dead;
            }
            else
            {
                animator.SetTrigger(_gotHitID);
            }
        }

        public void EmitAttackParticle()
        {
            Debug.Log("Player is attacked");
        }

        private enum EnemyState
        {
            Idle,
            Chasing,
            Attacking,
            Dead
        }
    }
}
