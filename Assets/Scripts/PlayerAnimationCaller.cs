using UnityEngine;

namespace Game
{
    // Tiny class so we can have proper animations
    public class PlayerAnimationCaller : MonoBehaviour
    {
        [SerializeField] private PlayerController player;

        public void ShootParticle() => player.EmitAttackParticle();
    }
}
