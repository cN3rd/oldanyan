using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PauseMenuController : PanelBase
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;

        // Events for external systems to subscribe to
        public event Action OnResumeClicked;
        public event Action OnRestartClicked;
        public event Action OnSettingsClicked;
        public event Action OnMainMenuClicked;

        // Keep these methods public for Unity editor integration
        public void OnResumeButtonClicked() => OnResumeClicked?.Invoke();
        public void OnRestartButtonClicked() => OnRestartClicked?.Invoke();
        public void OnSettingsButtonClicked() => OnSettingsClicked?.Invoke();
        public void OnMainMenuButtonClicked() => OnMainMenuClicked?.Invoke();

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
