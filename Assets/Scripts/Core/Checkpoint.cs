using System;
using UnityEngine;

namespace Game.SceneManagement
{
    [RequireComponent(typeof(Collider))]
    public class CheckpointComponent : MonoBehaviour
    {
        public event Action<CheckpointComponent> OnCheckpointPassed;

        public void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                Debug.Log("This is not the player");
                return;
            }

            OnCheckpointPassed?.Invoke(this);
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            var colliderComponent = GetComponent<Collider>();
            colliderComponent.isTrigger = true;
        }

        public void OnDrawGizmos()
        {
            var colliderBounds = GetComponent<Collider>().bounds;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(colliderBounds.center, colliderBounds.size);
        }
#endif
    }
}
