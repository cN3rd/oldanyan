using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MainMenuUI : PanelBase
    {
        [Header("Main-Menu specifics")] //
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private SettingsUI settingsUI;

        private void Start()
        {
            Show();
            if (GamePreferencesManager.Instance.Preferences.lastUsedSlot == -1)
            {
                loadGameButton.interactable = false;
            }
        }

        private void OnEnable()
        {
            newGameButton.onClick.AddListener(OnStartGameButtonClicked);
            loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);

            settingsUI.OnBackButtonClicked += SettingsUIOnBackButtonClicked;
        }

        private void OnDisable()
        {
            newGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
            loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            exitButton.onClick.RemoveListener(OnExitButtonClicked);

            settingsUI.OnBackButtonClicked -= SettingsUIOnBackButtonClicked;
        }

        public event Action OnNewGame;
        public event Action<int> OnLoadGame;

        private void OnLoadGameButtonClicked() => OnLoadGame?.Invoke(0);

        private void OnStartGameButtonClicked() => OnNewGame?.Invoke();

        private void OnExitButtonClicked() => Application.Quit();

        private void OnSettingsButtonClicked()
        {
            DoPanelStacking();
            settingsUI.Show();
        }

        private void SettingsUIOnBackButtonClicked() => DoPanelUnstacking();
    }
}
