using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelBase : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public virtual void Show() => canvasGroup.alpha = 1;
        public virtual void Hide() => canvasGroup.alpha = 0;
    }
}
