using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelBase : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Selectable initialSelectable;

        private GameObject _lastFocusedSelectable;

        public virtual void Show()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // truly select the object
            EventSystem.current?.SetSelectedGameObject(initialSelectable?.gameObject);
        }

        public virtual void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        protected void DoPanelStacking()
        {
            _lastFocusedSelectable = EventSystem.current?.currentSelectedGameObject ?? initialSelectable?.gameObject;
            canvasGroup.interactable = false;
        }

        protected void DoPanelUnstacking()
        {
            canvasGroup.interactable = true;
            EventSystem.current?.SetSelectedGameObject(_lastFocusedSelectable);
        }
    }
}
