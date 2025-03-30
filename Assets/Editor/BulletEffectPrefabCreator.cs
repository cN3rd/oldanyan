using System.Collections.Generic;
using System.IO;
using Game.Data;
using MasterStylizedProjectile;
using UnityEditor;
using UnityEngine;

namespace Game.Gameplay.Shooting
{
    public class BulletEffectPrefabCreator : EditorWindow
    {
        private BulletDatas bulletDatas;
        private string prefabSavePath = "Assets/Prefabs/BulletEffects";

        private void OnGUI()
        {
            GUILayout.Label("Bullet Effect Prefab Creator", EditorStyles.boldLabel);

            bulletDatas =
                EditorGUILayout.ObjectField("Bullet Datas", bulletDatas, typeof(BulletDatas), false) as
                    BulletDatas;

            prefabSavePath = EditorGUILayout.TextField("Save Path", prefabSavePath);

            if (GUILayout.Button("Create Directory"))
            {
                if (!Directory.Exists(prefabSavePath))
                {
                    Directory.CreateDirectory(prefabSavePath);
                    AssetDatabase.Refresh();
                }
            }

            EditorGUI.BeginDisabledGroup(bulletDatas == null);
            if (GUILayout.Button("Create Prefabs"))
            {
                CreatePrefabs();
            }

            EditorGUI.EndDisabledGroup();
        }

        [MenuItem("Tools/Bullet Effects/Create Prefabs")]
        public static void ShowWindow() =>
            GetWindow<BulletEffectPrefabCreator>("Bullet Effect Prefab Creator");

        private void CreatePrefabs()
        {
            if (!Directory.Exists(prefabSavePath))
            {
                Directory.CreateDirectory(prefabSavePath);
            }

            // Dictionary to store effect names and their corresponding prefab groups
            var prefabsByEffect = new Dictionary<string, BulletEffectPrefabs>();

            // First pass: Create all prefabs
            for (int i = 0; i < bulletDatas.Effects.Count; i++)
            {
                var effect = bulletDatas.Effects[i];
                string effectName = string.IsNullOrEmpty(effect.EffectName)
                    ? $"Effect_{i}"
                    : effect.EffectName;

                // Create prefab group scriptable object
                var prefabs = CreateInstance<BulletEffectPrefabs>();
                prefabs.effectName = effectName;
                prefabs.speed = effect.Speed;
                prefabs.isTargeting = effect.isTargeting;
                prefabs.rotSpeed = effect.RotSpeed;
                prefabs.chargeClip = effect.ChargeClip;
                prefabs.startClip = effect.startClip;
                prefabs.bulletClip = effect.bulletClip;
                prefabs.hitClip = effect.hitClip;
                prefabs.chargeParticleTime = effect.ChargeParticleTime;

                // Create ChargeParticles prefab
                if (effect.ChargeParticles != null)
                {
                    // Create a temporary object with the particle system centered
                    var tempObj = new GameObject($"{effectName}_Charge_Temp");
                    tempObj.transform.position = Vector3.zero;

                    // Add particle system as child but reset its position
                    var chargeParticles = Instantiate(effect.ChargeParticles);
                    chargeParticles.transform.SetParent(tempObj.transform, false);
                    chargeParticles.transform.localPosition =
                        Vector3.zero; // Critical: Position at origin

                    // Add audio source to the root
                    if (effect.ChargeClip != null)
                    {
                        var audioSource = tempObj.AddComponent<AudioSource>();
                        audioSource.clip = effect.ChargeClip;
                        audioSource.playOnAwake = true;
                    }

                    // Save as prefab
                    var chargePrefab = PrefabUtility.SaveAsPrefabAsset(tempObj,
                        $"{prefabSavePath}/{effectName}_Charge.prefab");

                    DestroyImmediate(tempObj);
                    prefabs.chargeParticlesPrefab = chargePrefab;
                }

                // Create StartParticles prefab
                if (effect.StartParticles != null)
                {
                    // Create a temporary object with the particle system centered
                    var tempObj = new GameObject($"{effectName}_Start_Temp");
                    tempObj.transform.position = Vector3.zero;

                    // Add particle system as child but reset its position
                    var startParticles = Instantiate(effect.StartParticles);
                    startParticles.transform.SetParent(tempObj.transform, false);
                    startParticles.transform.localPosition =
                        Vector3.zero; // Critical: Position at origin

                    // Add audio trigger to the root
                    if (effect.startClip != null)
                    {
                        var audioTrigger = tempObj.AddComponent<AudioTrigger>();
                        audioTrigger.onClip = effect.startClip;
                    }

                    // Save as prefab
                    var startPrefab = PrefabUtility.SaveAsPrefabAsset(tempObj,
                        $"{prefabSavePath}/{effectName}_Start.prefab");

                    DestroyImmediate(tempObj);
                    prefabs.startParticlesPrefab = startPrefab;
                }

                // Create BulletParticles prefab
                if (effect.BulletParticles != null)
                {
                    // Check and record the original position
                    var originalPosition = effect.BulletParticles.transform.position;

                    // Create a temporary object with the particle system
                    var tempObj = new GameObject($"{effectName}_Bullet_Temp");
                    tempObj.transform.position = Vector3.zero; // Reset position

                    // Add particle system as child but preserve its local configuration
                    var bulletParticles = Instantiate(effect.BulletParticles);
                    bulletParticles.transform.SetParent(tempObj.transform, false);
                    bulletParticles.transform.localPosition =
                        Vector3.zero; // Critical: Position at origin

                    // Add the MyBullet component to the root
                    var bullet = tempObj.AddComponent<MyBullet>();
                    bullet.speed = effect.Speed;
                    bullet.isTargeting = effect.isTargeting;
                    bullet.rotSpeed = effect.RotSpeed;

                    // Add audio source and configure it
                    var audioSource = tempObj.AddComponent<AudioSource>();
                    bullet.audioSource = audioSource;
                    if (effect.bulletClip != null)
                    {
                        audioSource.clip = effect.bulletClip;
                        audioSource.playOnAwake = true;
                        bullet.bulletClip = effect.bulletClip;
                    }

                    // Add hit clip reference
                    if (effect.hitClip != null)
                    {
                        bullet.onHitClip = effect.hitClip;
                    }

                    // Add collider - important to add to root
                    var collider = tempObj.AddComponent<SphereCollider>();
                    collider.isTrigger = true;
                    collider.radius = 0.6f;

                    // Save the final bullet prefab
                    var bulletPrefab = PrefabUtility.SaveAsPrefabAsset(tempObj,
                        $"{prefabSavePath}/{effectName}_Bullet.prefab");

                    DestroyImmediate(tempObj);

                    prefabs.bulletParticlesPrefab = bulletPrefab;
                }

                // Create HitParticles prefab
                if (effect.HitParticles != null)
                {
                    // Create a temporary object with the particle system centered
                    var tempObj = new GameObject($"{effectName}_Hit_Temp");
                    tempObj.transform.position = Vector3.zero;

                    // Add particle system as child but reset its position
                    var hitParticles = Instantiate(effect.HitParticles);
                    hitParticles.transform.SetParent(tempObj.transform, false);
                    hitParticles.transform.localPosition = Vector3.zero; // Critical: Position at origin

                    // Add audio trigger to the root
                    if (effect.hitClip != null)
                    {
                        var audioTrigger = tempObj.AddComponent<AudioTrigger>();
                        audioTrigger.onClip = effect.hitClip;
                    }

                    // Save as prefab
                    var hitPrefab = PrefabUtility.SaveAsPrefabAsset(tempObj,
                        $"{prefabSavePath}/{effectName}_Hit.prefab");

                    DestroyImmediate(tempObj);
                    prefabs.hitParticlesPrefab = hitPrefab;
                }

                // Save the ScriptableObject
                string prefabsPath = $"{prefabSavePath}/{effectName}_Prefabs.asset";
                AssetDatabase.CreateAsset(prefabs, prefabsPath);

                prefabsByEffect[effectName] = prefabs;
            }

            // Second pass: Update bullet prefabs with hit effect references
            foreach (var effect in prefabsByEffect.Values)
            {
                if (effect.bulletParticlesPrefab != null && effect.hitParticlesPrefab != null)
                {
                    // Open the prefab for editing
                    var bulletPrefab =
                        PrefabUtility.InstantiatePrefab(effect.bulletParticlesPrefab) as GameObject;

                    var bullet = bulletPrefab.GetComponent<MyBullet>();

                    if (bullet != null)
                    {
                        // Set the hit effect prefab reference
                        bullet.onHitEffectPrefab = effect.hitParticlesPrefab;
                        bullet.useHitPrefab = true;

                        // Save the changes
                        PrefabUtility.SaveAsPrefabAsset(bulletPrefab,
                            AssetDatabase.GetAssetPath(effect.bulletParticlesPrefab));

                        DestroyImmediate(bulletPrefab);
                    }
                }
            }

            // Create a master collection of all effect prefabs
            var collection = CreateInstance<BulletEffectPrefabsCollection>();
            collection.effectPrefabs = new List<BulletEffectPrefabs>(prefabsByEffect.Values);

            string collectionPath = $"{prefabSavePath}/BulletEffectPrefabsCollection.asset";
            AssetDatabase.CreateAsset(collection, collectionPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Created prefabs for {prefabsByEffect.Count} effects in {prefabSavePath}");
        }
    }
}
