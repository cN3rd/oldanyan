using Eflatun.SceneReference;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SceneManagement
{
    public class SceneManagerComponent : MonoBehaviour
    {
        [SerializeField] private LoadingScreenUI loadingScreenUI;
        [SerializeField] private SceneReference initialScene;

        [Tooltip("Disable the initial scene loading during edit mode (for ease of debugging)")]
        [SerializeField] private bool dontLoadSceneOnEditMode;

        private readonly AddressableSceneLoader _sceneLoader = new();
        private LoadingUIReporter _reporter;

        public async Awaitable LoadScene(SceneReference scene)
        {
            loadingScreenUI.Show();
            await _sceneLoader.UnloadAllAddedScenes();
            await _sceneLoader.LoadAddressableScene(scene, LoadSceneMode.Additive, _reporter);
            loadingScreenUI.Hide();
        }

        void Awake() => _reporter = new LoadingUIReporter(loadingScreenUI);

        public SceneReference GetInitialScene()
        {
#if UNITY_EDITOR
            if (dontLoadSceneOnEditMode)
            {
                return null;
            }

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (SceneReference.FromScenePath(scene.path) == initialScene)
                {
                    Debug.Log("Scene already loaded");
                    return null;
                }
            }
#endif
            return initialScene;
        }
    }
}
