using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PauseMenuUI : PanelBase
    {
        [Header("Pause-menu Specific")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private SettingsUI settingsUI;

        private void OnEnable()
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

            settingsUI.OnBackButtonClicked += OnSettingsHidden;
        }

        private void OnDisable()
        {
            resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);

            settingsUI.OnBackButtonClicked -= OnSettingsHidden;
        }

        public event Action OnResumeGame;
        public event Action OnRestartClicked;
        public event Action OnMainMenuClicked;

        private void OnResumeButtonClicked()
        {
            Hide();
            OnResumeGame?.Invoke();
        }

        private void OnRestartButtonClicked() => OnRestartClicked?.Invoke();

        private void OnSettingsButtonClicked()
        {
            settingsUI.Show();
            DoPanelStacking();
        }

        private void OnMainMenuButtonClicked() => OnMainMenuClicked?.Invoke();

        private void OnSettingsHidden() => DoPanelUnstacking();
    }
}
