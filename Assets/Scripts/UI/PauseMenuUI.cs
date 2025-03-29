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

        [Header("UI Elements")]
        [SerializeField] private SettingsUI settingsUI;

        public event Action OnResumeGame;
        public event Action OnRestartClicked;
        public event Action OnMainMenuClicked;

        public override void Show() => base.Show();
        public override void Hide() => base.Hide();

        private void OnResumeButtonClicked()
        {
            Hide();
            OnResumeGame?.Invoke();
        }

        private void OnRestartButtonClicked() => OnRestartClicked?.Invoke();
        private void OnSettingsButtonClicked() => settingsUI.Show();
        private void OnMainMenuButtonClicked() => OnMainMenuClicked?.Invoke();

        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }
    }
}
