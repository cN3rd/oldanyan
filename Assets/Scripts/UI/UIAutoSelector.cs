using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIAutoSelector : MonoBehaviour
    {
        [SerializeField] private Selectable elementToSelect;

        void Start() => SelectElement();

        private void SelectElement() => elementToSelect.Select();
    }
}
