using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PauseMenuUI : PanelBase
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;

        public event Action OnResumeClicked;
        public event Action OnRestartClicked;
        public event Action OnSettingsClicked;
        public event Action OnMainMenuClicked;

        private void OnResumeButtonClicked() => OnResumeClicked?.Invoke();
        private void OnRestartButtonClicked() => OnRestartClicked?.Invoke();
        private void OnSettingsButtonClicked() => OnSettingsClicked?.Invoke();
        private void OnMainMenuButtonClicked() => OnMainMenuClicked?.Invoke();

        private void OnEnable()
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        private void OnDisable()
        {
            resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }
    }
}
