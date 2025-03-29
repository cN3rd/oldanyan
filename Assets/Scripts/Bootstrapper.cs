using Game.SceneManagement;
using UnityEngine;

namespace Game
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private SceneManagerComponent sceneManager;
        private async void Start()
        {
            await sceneManager.LoadScene(sceneManager.GetInitialScene());
        }

    }
}
