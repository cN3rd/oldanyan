using Game.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Core
{
    public class PreferencesApplier : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;

        private void Awake() => ApplyPreferences();

        public void ApplyPreferences()
        {
            var prefs = GamePreferencesManager.Instance.Preferences;
            if (prefs.visualSettings.screenWidth != 0 && prefs.visualSettings.screenHeight != 0)
            {
                Screen.SetResolution(prefs.visualSettings.screenWidth,
                    prefs.visualSettings.screenHeight,
                    prefs.visualSettings.fullScreenMode);
            }

            Screen.fullScreenMode = prefs.visualSettings.fullScreenMode;
            QualitySettings.vSyncCount = prefs.visualSettings.vsync ? 1 : 0;

            mixer.SetFloat("MasterVolume",
                Utils.LinearVolumeToDecibel(prefs.audioSettings.masterVolume));

            mixer.SetFloat("AmbientVolume",
                Utils.LinearVolumeToDecibel(prefs.audioSettings.ambientVolume));

            mixer.SetFloat("MusicVolume",
                Utils.LinearVolumeToDecibel(prefs.audioSettings.musicVolume));

            mixer.SetFloat("SFXVolume",
                Utils.LinearVolumeToDecibel(prefs.audioSettings.sfxVolume));
        }
    }
}
