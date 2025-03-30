using System.Collections.Generic;
using Eflatun.SceneReference;
using Game.Core;
using Game.Gameplay.Components;
using Game.Input;
using Game.SceneManagement;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    [DisallowMultipleComponent]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<CheckpointComponent> checkpoints = new();
        [SerializeField] private PlayerController player;
        [SerializeField] private PauseMenuUI pauseMenuUI;
        [SerializeField] private InputState inputState;

        // Reference to the scene guid of the current level, used in the scene-loading mechanism
        // Automatically set by OnValidate
        [SerializeField] private string sceneGuid;
        private readonly Dictionary<CheckpointComponent, int> _checkpointToIndex = new();

        private GameSaveWrapper _gameSaveWrapper;

        private void Awake() =>
            // This will contain the right data always:
            //  - On new games, this will be a brand-new save (checkpoint 0 is always our beginning point)
            //  - On loaded games, this will be the relevant save
            _gameSaveWrapper =
                GameSaveWrapper.FromSaveSlot(GamePreferencesManager.Instance.Preferences.lastUsedSlot);

        private void Start()
        {
            SetupEvents();
            BuildCheckpointIndex();
            LoadLastCheckpoint();
        }

        private void OnDestroy()
        {
            foreach (var checkpoint in checkpoints)
            {
                checkpoint.OnCheckpointPassed -= CheckpointPassed;
            }

            RemoveEvents();
        }

        private void SetupEvents()
        {
            inputState.OnPauseGame += PauseGame;
            pauseMenuUI.OnResumeGame += ResumeGame;
        }

        private void RemoveEvents()
        {
            inputState.OnPauseGame -= PauseGame;
            pauseMenuUI.OnResumeGame -= ResumeGame;
        }

        private void BuildCheckpointIndex()
        {
            int checkpointCount = checkpoints.Count;
            _checkpointToIndex.EnsureCapacity(checkpointCount);
            for (int checkpointIdx = 0; checkpointIdx < checkpointCount; checkpointIdx++)
            {
                var checkpoint = checkpoints[checkpointIdx];
                checkpoint.OnCheckpointPassed += CheckpointPassed;
                _checkpointToIndex.Add(checkpoint, checkpointIdx);
            }
        }

        private void CheckpointPassed(CheckpointComponent checkpoint)
        {
            if (!_checkpointToIndex.TryGetValue(checkpoint, out int checkpointIndex))
            {
                Debug.LogError($"Couldn't find checkpoint: {checkpoint.name}");
                return;
            }

            int savedCheckpointIndex = _gameSaveWrapper.ActualSave.currentCheckpoint.id;
            if (checkpointIndex < savedCheckpointIndex)
            {
                return;
            }

            Debug.Log(sceneGuid);
            _gameSaveWrapper.ActualSave.currentCheckpoint.id = checkpointIndex;
            _gameSaveWrapper.ActualSave.currentCheckpoint.sceneGuid = sceneGuid;
            _gameSaveWrapper.SaveGame();

            if (checkpoints.Count == checkpointIndex + 1)
            {
                GameManager.Instance.AllCheckpointsComplete();
            }
        }

        private void LoadLastCheckpoint()
        {
            var checkpoint = checkpoints[_gameSaveWrapper.ActualSave.currentCheckpoint.id];
            player.transform.position = checkpoint.transform.position;
        }

        private void PauseGame()
        {
            pauseMenuUI.Show();
            inputState.BlockPlayerInput();
        }

        private void ResumeGame() => inputState.UnblockPlayerInput();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (sceneGuid == string.Empty)
            {
                UpdateSceneGuid();
            }

            if (checkpoints.Count == 0)
            {
                RefreshCheckpoints();
            }
        }

        [ContextMenu("Refresh Checkpoints")]
        private void RefreshCheckpoints()
        {
            checkpoints.Clear();
            AddNewCheckpoint();
            Debug.LogWarning("Checkpoints were refreshed, saves might be invalid");
        }

        [ContextMenu("Add new checkpoints")]
        private void AddNewCheckpoint()
        {
            var existingCheckpoints = new HashSet<CheckpointComponent>(checkpoints);

            // Perf: only done while editing the level, at the user's will
            var allCheckpoints = new HashSet<CheckpointComponent>(
                FindObjectsByType<CheckpointComponent>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.InstanceID));

            allCheckpoints.ExceptWith(existingCheckpoints);

            checkpoints.AddRange(allCheckpoints);

            Debug.Log(allCheckpoints.Count > 0
                ? $"{allCheckpoints.Count} new checkpoints added"
                : "No new checkpoints found");
        }

        [ContextMenu("Update scene GUID")]
        private void UpdateSceneGuid()
        {
            sceneGuid = SceneReference.FromScenePath(SceneManager.GetActiveScene().path).Guid;
            Debug.Log($"New GUID set: {sceneGuid}");
        }
#endif
    }
}
