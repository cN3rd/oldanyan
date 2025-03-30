using System;
using Game.UI;
using UnityEngine;

namespace Game.Gameplay.Components
{
    [DisallowMultipleComponent]
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHP = 100;
        [SerializeField] private HealthUI healthUI;

        public int CurrentHP { get; private set; }

        public int MaxHP => maxHP;
        public bool IsDead { get; private set; }

        private void Awake() => ResetHealth();

        public event Action OnDeath;
        public event Action<int> OnDamaged;

        public void TakeDamage(int damage)
        {
            if (IsDead)
            {
                return;
            }

            CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, maxHP);
            healthUI?.SetHealth(CurrentHP, maxHP);
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
            healthUI.SetHealth(CurrentHP, maxHP);
            IsDead = false;
        }
    }
}
