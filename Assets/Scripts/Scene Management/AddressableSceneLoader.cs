using System;
using System.Collections;
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

            var handle = Addressables.LoadSceneAsync(sceneRef.Address, loadMode);

            // If you're not reporting progress, just await the task itself
            if (progress == null)
            {
                await handle.Task;
            }
            else
            {
                await TrackProgress(handle, progress);
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to load scene {sceneRef.Address}: {handle.OperationException}");
                Addressables.Release(handle);
                return;
            }

            _sceneInstances.Add(sceneRef.Guid, handle.Result);
        }

        public async Awaitable UnloadAllAddedScenes()
        {
            foreach (var sceneGuid in _sceneInstances.Keys)
            {
                await UnloadAddressableScene(new SceneReference(sceneGuid));
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
            if (!_sceneInstances.TryGetValue(sceneRef.Guid, out SceneInstance sceneInstance))
            {
                Debug.LogError("Cannot unload an unloaded scene.");
                return;
            }

            var handle = Addressables.UnloadSceneAsync(sceneInstance);

            // If you're not reporting progress, just await the task itself
            if (progress == null)
            {
                await handle.Task;
            }
            else
            {
                await TrackProgress(handle, progress);
            }

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to unload scene {sceneRef.Guid}: {handle.OperationException}");
                return;
            }

            _sceneInstances.Remove(sceneRef.Guid);
        }

        /// <summary>
        /// Shared logic for smart reporting of progress
        /// </summary>
        private async Awaitable TrackProgress(AsyncOperationHandle<SceneInstance> handle,
            [System.Diagnostics.CodeAnalysis.NotNull] IProgress<float> progressReport)
        {
            float lastProgress = -1;
            while (!handle.IsDone)
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
            if (lastProgress < 1f)
            {
                progressReport.Report(1f);
            }
        }
    }
}
