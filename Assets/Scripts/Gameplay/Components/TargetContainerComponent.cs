using UnityEngine;

namespace Game.Gameplay.Components
{
    [DisallowMultipleComponent]
    public class TargetContainerComponent : MonoBehaviour
    {
        [SerializeField] private Transform target;

        public Transform Target => target;

        public void SetTarget(Transform newTarget) => target = newTarget;
    }
}
