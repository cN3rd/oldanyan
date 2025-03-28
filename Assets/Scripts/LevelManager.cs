using System.Collections.Generic;
using Eflatun.SceneReference;
using Game.Data;
using Game.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    [DisallowMultipleComponent]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<CheckpointComponent> checkpoints = new();
        [SerializeField] private PlayerController player;

        // Reference to the scene guid of the current level, used in the scene-loading mechanism
        [HideInInspector] [SerializeField] private string sceneGuid;

        private GameSaveManager _gameSaveManager;
        private readonly Dictionary<CheckpointComponent, int> _checkpointToIndex = new();

        private void Awake() =>
            _gameSaveManager = GamePreferencesManager.Instance.GetManagerForCurrentSave();

        private void Start()
        {
            BuildCheckpointIndex();
            LoadLastCheckpoint();
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

        private void OnDestroy()
        {
            foreach (var checkpoint in checkpoints)
            {
                checkpoint.OnCheckpointPassed -= CheckpointPassed;
            }
        }

        private void CheckpointPassed(CheckpointComponent checkpoint)
        {
            if (!_checkpointToIndex.TryGetValue(checkpoint, out int checkpointIndex))
            {
                Debug.LogError($"Couldn't find checkpoint: {checkpoint.name}");
                return;
            }

            int savedCheckpointIndex = _gameSaveManager.ActualSave.currentCheckpoint?.id ?? 0;
            if (checkpointIndex < savedCheckpointIndex)
            {
                return;
            }

            _gameSaveManager.ActualSave.currentCheckpoint = new()
            {
                id = checkpointIndex, sceneGuid = sceneGuid
            };

            _gameSaveManager.SaveGame();
        }

        private void LoadLastCheckpoint()
        {
            var checkpoint = checkpoints[_gameSaveManager.ActualSave.currentCheckpoint?.id ?? 0];
            player.transform.position = checkpoint.transform.position;
        }

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

            Debug.Log(allCheckpoints.Count > 0 ? $"{allCheckpoints.Count} new checkpoints added" : "No new checkpoints found");
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
