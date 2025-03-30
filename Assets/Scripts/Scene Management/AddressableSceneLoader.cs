using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.SceneManagement
{
    /// <summary>
    /// Helper class for loading/unloading scenes, book-keeping the SceneInstances.
    /// </summary>
    public class AddressableSceneLoader
    {
        // Uses Guid as string key for consistency with SceneReference
        private readonly Dictionary<string, SceneInstance> _sceneInstances = new();

        /// <summary>
        /// Loads a scene asynchronously from an addressable.
        /// </summary>
        /// <param name="sceneRef">Reference to the addressable scene</param>
        /// <param name="loadMode">Scene load mode (optional)</param>
        /// <param name="progress">Progress reporter (optional)</param>
        public async Awaitable LoadAddressableScene(SceneReference sceneRef,
            LoadSceneMode loadMode = LoadSceneMode.Single, IProgress<float> progress = null)
        {
            if (_sceneInstances.ContainsKey(sceneRef.Guid))
            {
                Debug.Log("The scene is already loaded.");
                return;
            }

            // Keep the handle outside to ensure it is freed if needed
            AsyncOperationHandle<SceneInstance> handle = default;
            try
            {
                handle = Addressables.LoadSceneAsync(sceneRef.Address, loadMode);

                if (progress != null)
                {
                    await TrackProgress(handle, progress);
                }

                // ensure the task is done
                var loadedSceneInstance = await handle.Task;
                _sceneInstances.Add(sceneRef.Guid, loadedSceneInstance);

                // set as active scene (for lighting and stuff)
                SceneManager.SetActiveScene(loadedSceneInstance.Scene);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load scene {sceneRef.Address}: {ex}");
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            // No finally needed here as we only release on failure for LoadSceneAsync
        }

        public async Awaitable UnloadAllAddedScenes()
        {
            var sceneGuidsToUnload = new List<string>(_sceneInstances.Keys);
            foreach (string sceneGuid in sceneGuidsToUnload)
            {
                if (_sceneInstances.ContainsKey(sceneGuid))
                {
                     await UnloadAddressableScene(new SceneReference(sceneGuid));
                }
            }
        }

        /// <summary>
        /// Unloads an addressable scene
        /// </summary>
        /// <param name="sceneRef">Reference to the addressable scene</param>
        /// <param name="progress">Progress reporter (optional)</param>
        public async Awaitable UnloadAddressableScene(SceneReference sceneRef,
            IProgress<float> progress = null)
        {
            if (!_sceneInstances.Remove(sceneRef.Guid, out SceneInstance sceneInstance))
            {
                // Changed from Error to Warning/Log as trying to unload an unloaded scene isn't necessarily an error
                Debug.LogWarning($"Cannot unload scene {sceneRef.Guid}: It is not currently tracked as loaded.");
                return;
            }

            try
            {
                var handle = Addressables.UnloadSceneAsync(sceneInstance);

                if (progress == null)
                {
                    await handle.Task;
                }
                else
                {
                    await TrackProgress(handle, progress);
                }

                Debug.Log($"Successfully unloaded scene {sceneRef.Guid}");
            }
            catch (Exception ex)
            {
                 Debug.LogError($"Failed to unload scene {sceneRef.Guid}: {ex}");
            }
        }

        /// <summary>
        /// Shared logic for smart reporting of progress
        /// </summary>
        private async Awaitable TrackProgress(AsyncOperationHandle<SceneInstance> handle,
            [System.Diagnostics.CodeAnalysis.NotNull] IProgress<float> progressReport)
        {
            float lastProgress = -1;
            while (handle.IsValid() && !handle.IsDone)
            {
                // only report on progress changes
                float currentProgress = handle.PercentComplete;
                if (currentProgress > lastProgress)
                {
                    progressReport.Report(currentProgress);
                    lastProgress = currentProgress;
                }

                await Awaitable.EndOfFrameAsync();
            }

            // Ensure the final progress (1.0) is reported
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded && lastProgress < 1f)
            {
                progressReport.Report(1f);
            }
        }
    }
}
