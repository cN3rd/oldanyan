using Game.Gameplay.Components;
using UnityEngine;

namespace Game
{
    public class PickableWand : MonoBehaviour
    {
        [SerializeField] private Collider triggerCollider;
        [SerializeField] private Transform shootPoint;

        public Transform ShootPoint => shootPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            var playerController = other.GetComponent<PlayerController>();
            playerController.AttachWand(this);
            triggerCollider.enabled = false;
        }
    }
}
