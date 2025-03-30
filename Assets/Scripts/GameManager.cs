using System;
using System.Collections;
using Eflatun.SceneReference;
using Game.Core;
using Game.Data;
using Game.Gameplay.Components;
using Game.SceneManagement;
using Game.UI;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SceneManagerComponent sceneManager;
        [SerializeField] private StaticGameData staticGameData;

        public static GameManager Instance { get; private set; }

        private void Awake() => Instance = this;

        private void Start() => ShowMainMenu();

        private void ShowMainMenu() => StartCoroutine(ShowMainMenuCoroutine());

        private IEnumerator ShowMainMenuCoroutine()
        {
            yield return sceneManager.LoadScene(staticGameData.mainMenuScene);

            // Unforgivable, but we only do this once per main menu load
            var mainMenuController = FindFirstObjectByType<MainMenuUI>();

            // Events deregister automatically when the main menu scene is unloaded
            mainMenuController.OnNewGame += OnNewGame;
            mainMenuController.OnLoadGame += OnLoadGame;
        }

        private IEnumerator LoadLevelCoroutine(SceneReference level)
        {
            yield return sceneManager.LoadScene(level);

            // Unforgivable, but we only do this once per level load
            var pauseMenuController = FindFirstObjectByType<PauseMenuUI>();
            var actualPlayerController = FindFirstObjectByType<PlayerController>();

            // Events deregister automatically when the level is unloaded
            pauseMenuController.OnRestartClicked += RestartClicked;
            pauseMenuController.OnMainMenuClicked += ShowMainMenu;
            actualPlayerController.OnPlayerDeath += RestartClicked;
        }

        private void OnLoadGame(int slot)
        {
            Debug.Log($"Loading game from slot {slot}");
            var gameSave = GameSaveWrapper.FromSaveSlot(slot);
            var level = new SceneReference(gameSave.ActualSave.currentCheckpoint.sceneGuid);
            StartCoroutine(LoadLevelCoroutine(level));
        }

        private void OnNewGame()
        {
            Debug.Log("Starting new game");
            var gameSave = GameSaveWrapper.CreateNewGame();
            var level = new SceneReference(staticGameData.levels[0].Guid);
            StartCoroutine(LoadLevelCoroutine(level));
        }

        // just reload the game from the most recently used slot
        private void RestartClicked() =>
            OnLoadGame(GamePreferencesManager.Instance.Preferences.lastUsedSlot);

        public void AllCheckpointsComplete()
        {
            // In a bigger game we'd have a better way to get the current level
            // but we only have, like, 3 levels or so
            var save = GameSaveWrapper.FromSaveSlot(GamePreferencesManager.Instance.Preferences
                .lastUsedSlot);

            string currentLevelGuid = save.ActualSave.currentCheckpoint.sceneGuid;
            int currentLevelIndex =
                Array.IndexOf(staticGameData.levels, new SceneReference(currentLevelGuid));

            if (currentLevelIndex == -1)
            {
                Debug.LogError("Could not find level...");
                return;
            }

            // Cycle through levels
            // TODO: increase game difficulty
            currentLevelIndex = (currentLevelIndex + 1) % staticGameData.levels.Length;
            StartCoroutine(LoadLevelCoroutine(staticGameData.levels[currentLevelIndex]));
        }
    }
}
