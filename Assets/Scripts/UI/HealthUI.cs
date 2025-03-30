using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] public Slider healthUISlider;

        public void SetHealth(int health, int maxHealth) =>
            healthUISlider.value = (float)health / maxHealth;
    }
}
