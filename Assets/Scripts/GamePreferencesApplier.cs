using Game.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Game
{
    public class PreferencesApplier : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;

        void Awake() => ApplyPreferences();

        public void ApplyPreferences()
        {
            var prefs = GamePreferencesManager.Instance.Preferences;
            Screen.SetResolution(prefs.visualSettings.screenWidth, prefs.visualSettings.screenHeight,
                prefs.visualSettings.fullScreenMode);

            QualitySettings.vSyncCount = prefs.visualSettings.vsync ? 1 : 0;

            mixer.SetFloat("MasterVolume",
                Game.Utils.LinearVolumeToDecibel(prefs.audioSettings.masterVolume));

            mixer.SetFloat("AmbientVolume",
                Game.Utils.LinearVolumeToDecibel(prefs.audioSettings.ambientVolume));

            mixer.SetFloat("MusicVolume",
                Game.Utils.LinearVolumeToDecibel(prefs.audioSettings.musicVolume));

            mixer.SetFloat("SFXVolume",
                Game.Utils.LinearVolumeToDecibel(prefs.audioSettings.sfxVolume));
        }
    }
}
