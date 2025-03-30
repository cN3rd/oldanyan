using System;
using UnityEngine;

namespace Game.Gameplay.Components
{
    [DisallowMultipleComponent]
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHP = 100;

        public int CurrentHP { get; private set; }

        public int MaxHP => maxHP;
        public bool IsDead { get; private set; }

        private void Awake() => CurrentHP = maxHP;

        public event Action OnDeath;
        public event Action<int> OnDamaged;

        public void TakeDamage(int damage)
        {
            if (IsDead)
            {
                return;
            }

            CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, maxHP);
            OnDamaged?.Invoke(damage);

            if (CurrentHP <= 0)
            {
                IsDead = true;
                OnDeath?.Invoke();
            }
        }

        public void ResetHealth()
        {
            CurrentHP = maxHP;
            IsDead = false;
        }
    }
}
