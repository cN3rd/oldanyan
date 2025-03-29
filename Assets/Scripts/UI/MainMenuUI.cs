using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] Button newGameButton;
        [SerializeField] Button loadGameButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button exitButton;
        [SerializeField] SettingsUI settingsUI;

        public event Action OnNewGame;
        public event Action<int> OnLoadGame;

        public void OnEnable()
        {
            newGameButton.onClick.AddListener(OnStartGameButtonClicked);
            loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        public void OnDisable()
        {
            newGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
            loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        private void OnLoadGameButtonClicked() => OnLoadGame?.Invoke(0);

        private void OnStartGameButtonClicked() => OnNewGame?.Invoke();

        private void OnExitButtonClicked() => Application.Quit();

        private void OnSettingsButtonClicked() => settingsUI.Show();
    }
}
