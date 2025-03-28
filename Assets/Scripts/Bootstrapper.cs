using System;
using Eflatun.SceneReference;
using Game.SceneManagement;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    internal class UIReporter : IProgress<float>
    {
        private readonly LoadingScreenUI _ui;

        public UIReporter(LoadingScreenUI ui) => _ui = ui;
        public void Report(float value) => _ui.UpdateProgress(value);
    }

    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private LoadingScreenUI loadingScreenUI;
        [SerializeField] private SceneReference initialScene;

        [Tooltip("Disable the initial scene loading during edit mode (for ease of debugging)")]
        [SerializeField]
        private bool dontLoadSceneOnEditMode;

        private readonly AddressableSceneLoader _sceneLoader = new();
        private UIReporter _reporter;

        private async void Start()
        {
            _reporter = new UIReporter(loadingScreenUI);
#if UNITY_EDITOR
            if (ShouldSkipSceneLoading())
            {
                return;
            }
#endif
            await LoadScene(initialScene);
        }

        private async void LoadLevel()
        {

        }

        private async Awaitable LoadScene(SceneReference scene)
        {
            loadingScreenUI.Show();
            await _sceneLoader.UnloadAllAddedScenes();
            await _sceneLoader.LoadAddressableScene(scene, LoadSceneMode.Additive, _reporter);
            loadingScreenUI.Hide();
        }

#if UNITY_EDITOR
        private bool ShouldSkipSceneLoading()
        {
            if (dontLoadSceneOnEditMode)
            {
                return true;
            }

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (SceneReference.FromScenePath(scene.path) == initialScene)
                {
                    Debug.Log("Scene already loaded");
                    return true;
                }
            }

            return false;
        }
#endif
    }
}
