using System;
using UnityEngine;

namespace Game.Gameplay.Components
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(PlayerLocomotionComponent))]
    [RequireComponent(typeof(PlayerAttackerComponent))]
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;
        [SerializeField] private PlayerLocomotionComponent locomotion;
        [SerializeField] private PlayerAttackerComponent attacker;

        private void Awake() => health.OnDeath += HandlePlayerDeath;

        private void OnDestroy() => health.OnDeath -= HandlePlayerDeath;

        public event Action OnPlayerDeath;

        private void HandlePlayerDeath()
        {
            Debug.Log("Player died");
            OnPlayerDeath?.Invoke();
        }

        public void AttachWand(PickableWand pickableWand) => attacker.AttachWand(pickableWand);

        public void EmitAttackParticle() => attacker.EmitAttackParticle();
    }
}
