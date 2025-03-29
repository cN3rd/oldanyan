using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    internal struct Resolution : IEquatable<Resolution>
    {
        public int width;
        public int height;

        public bool Equals(Resolution other) => width == other.width && height == other.height;
        public override bool Equals(object obj) => obj is Resolution other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(width, height);
        public static bool operator ==(Resolution left, Resolution right) => left.Equals(right);

        public static bool operator !=(Resolution left, Resolution right) =>
            !left.Equals(right);
    }

    public class SettingsUI : PanelBase
    {
        [SerializeField] private PreferencesApplier preferencesApplier;
        [SerializeField] private Button backButton;

        [Header("Visual Settings")] //
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown windowingModeDropdown;
        [SerializeField] private Toggle vsyncToggle;

        [Header("Audio Settings")] //
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider ambientVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        private Dictionary<string, FullScreenMode> _fullscreenModes;
        private Resolution[] _resolutions;

        private void Awake()
        {
            InitializeResolutionsDropdown();
            InitializeWindowingModeDropdown();

            vsyncToggle.isOn = GamePreferencesManager.Instance.Preferences.visualSettings.vsync;
            masterVolumeSlider.value =
                GamePreferencesManager.Instance.Preferences.audioSettings.masterVolume;

            ambientVolumeSlider.value =
                GamePreferencesManager.Instance.Preferences.audioSettings.ambientVolume;

            musicVolumeSlider.value =
                GamePreferencesManager.Instance.Preferences.audioSettings.musicVolume;

            sfxVolumeSlider.value =
                GamePreferencesManager.Instance.Preferences.audioSettings.sfxVolume;

            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            windowingModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);

            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            ambientVolumeSlider.onValueChanged.AddListener(OnAmbientVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

            backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnDestroy()
        {
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            windowingModeDropdown.onValueChanged.RemoveListener(OnScreenModeChanged);
            vsyncToggle.onValueChanged.RemoveListener(OnVSyncChanged);

            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            ambientVolumeSlider.onValueChanged.RemoveListener(OnAmbientVolumeChanged);
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);

            backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnBackClicked() => Hide();

        private void InitializeResolutionsDropdown()
        {
            resolutionDropdown.ClearOptions();
            _resolutions = Screen.resolutions
                .Select(r => new Resolution { width = r.width, height = r.height }).Distinct()
                .ToArray();

            // calculate desired width/height from settings
            int visualSettingsScreenWidth = GamePreferencesManager.Instance.Preferences
                .visualSettings.screenWidth;

            int visualSettingsScreenHeight = GamePreferencesManager.Instance.Preferences
                .visualSettings.screenHeight;

            // fallback for current resolution if nothing was ever selected
            int desiredWidth = visualSettingsScreenWidth == 0
                ? Screen.currentResolution.width
                : visualSettingsScreenWidth;

            int desiredHeight = visualSettingsScreenHeight == 0
                ? Screen.currentResolution.height
                : visualSettingsScreenHeight;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string resolutionText = $"{_resolutions[i].width} x {_resolutions[i].height}";
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutionText));

                // select option based on desired resolution
                if (_resolutions[i].width == desiredWidth &&
                    _resolutions[i].height == desiredHeight)
                {
                    resolutionDropdown.SetValueWithoutNotify(i);
                }
            }

            resolutionDropdown.RefreshShownValue();
        }

        private void InitializeWindowingModeDropdown()
        {
            _fullscreenModes = new Dictionary<string, FullScreenMode>
            {
                { "Windowed", FullScreenMode.Windowed },
                { "Borderless", FullScreenMode.FullScreenWindow },
                { "Fullscreen", FullScreenMode.ExclusiveFullScreen },
                { "Maximized Window", FullScreenMode.MaximizedWindow }
            };

            windowingModeDropdown.ClearOptions();
            foreach (var option in _fullscreenModes)
            {
                windowingModeDropdown.options.Add(new TMP_Dropdown.OptionData(option.Key));
            }

            windowingModeDropdown.SetValueWithoutNotify((int)GamePreferencesManager.Instance.Preferences
                .visualSettings.fullScreenMode);
        }

        private void PropagateSettings()
        {
            preferencesApplier.ApplyPreferences();
            GamePreferencesManager.Instance.UpdateGamePreferences();
        }

        private void OnResolutionChanged(int newSelection)
        {
            GamePreferencesManager.Instance.Preferences.visualSettings.screenWidth =
                _resolutions[newSelection].width;

            GamePreferencesManager.Instance.Preferences.visualSettings.screenHeight =
                _resolutions[newSelection].height;

            PropagateSettings();
        }

        private void OnScreenModeChanged(int newSelection)
        {
            GamePreferencesManager.Instance.Preferences.visualSettings.fullScreenMode =
                Screen.fullScreenMode;

            PropagateSettings();
        }

        private void OnVSyncChanged(bool newValue)
        {
            GamePreferencesManager.Instance.Preferences.visualSettings.vsync = newValue;
            PropagateSettings();
        }

        private void OnSfxVolumeChanged(float newVolume)
        {
            GamePreferencesManager.Instance.Preferences.audioSettings.sfxVolume = newVolume;
            PropagateSettings();
        }

        private void OnMusicVolumeChanged(float newVolume)
        {
            GamePreferencesManager.Instance.Preferences.audioSettings.musicVolume = newVolume;
            PropagateSettings();
        }

        private void OnAmbientVolumeChanged(float newVolume)
        {
            GamePreferencesManager.Instance.Preferences.audioSettings.ambientVolume = newVolume;
            PropagateSettings();
        }

        private void OnMasterVolumeChanged(float newVolume)
        {
            GamePreferencesManager.Instance.Preferences.audioSettings.masterVolume = newVolume;
            PropagateSettings();
        }
    }
}
