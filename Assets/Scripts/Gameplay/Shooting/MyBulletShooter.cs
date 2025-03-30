using System.Collections;
using System.Linq;
using Game.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay.Shooting
{
    public class BulletShooter : MonoBehaviour
    {
        public BulletEffectPrefabsCollection effectPrefabs;
        public int index;
        public Transform startNodeTrans;    // This is your "StartShootPosition"
        public float shootInterval = 0.2f;

        private bool _isFiring;
        private float _lastShootTime;
        private BulletEffectPrefabs CurrentEffect => effectPrefabs.GetEffectByIndex(index);

        private void Update()
        {
            // Check for left mouse button
            if (Mouse.current == null)
            {
                return;
            }

            // Handle initial click
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _isFiring = true;
                Shoot();
            }

            // Handle button release
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _isFiring = false;
            }

            // Handle continuous firing
            if (_isFiring && Time.time - _lastShootTime > shootInterval)
            {
                Shoot();
            }
        }

        public void Shoot()
        {
            // Ensure we have a valid effect and start position
            if (!CurrentEffect)
            {
                Debug.LogError(
                    "No bullet effect selected! Please assign effectPrefabs and check the Index.");
                return;
            }

            if (startNodeTrans == null)
            {
                Debug.LogError("StartShootPosition (startNodeTrans) is not assigned!");
                return;
            }

            StartCoroutine(ShootIE());
        }

        public IEnumerator ShootIE()
        {
            _lastShootTime = Time.time;
            yield return Charge();

            DoShoot();
        }

        public IEnumerator Charge()
        {
            // Ensure we have a valid start position
            if (startNodeTrans == null)
            {
                Debug.LogError("StartShootPosition (startNodeTrans) is not assigned!");
                yield break;
            }

            if (CurrentEffect.chargeParticlesPrefab)
            {
                // Instantiate the charge effect at the exact position of the StartShootPosition
                var chargeInstance = Instantiate(CurrentEffect.chargeParticlesPrefab,
                    startNodeTrans.position,
                    Quaternion.identity);

                yield return new WaitForSeconds(CurrentEffect.chargeParticleTime);

                Destroy(chargeInstance);
            }
        }

        public void DoShoot()
        {
            // Ensure we have a valid start position
            if (startNodeTrans == null)
            {
                Debug.LogError("StartShootPosition (startNodeTrans) is not assigned!");
                return;
            }

            var targetPos = GetMouseTargetPos();
            var targetDir = targetPos - startNodeTrans.position;
            targetDir = targetDir.normalized;

            // Calculate the rotation once
            var targetRotation = Quaternion.LookRotation(targetDir);

            // Instantiate start particles prefab exactly at the StartShootPosition
            if (CurrentEffect.startParticlesPrefab)
            {
                var startInstance = Instantiate(
                    CurrentEffect.startParticlesPrefab,
                    startNodeTrans.position,
                    targetRotation);

                Destroy(startInstance, 5f);
            }

            // Instantiate bullet particles prefab exactly at the StartShootPosition
            if (CurrentEffect.bulletParticlesPrefab)
            {
                var bulletInstance = Instantiate(
                    CurrentEffect.bulletParticlesPrefab,
                    startNodeTrans.position,
                    targetRotation);

                // Only set dynamic properties that can't be pre-configured
                var bullet = bulletInstance.GetComponent<MyBullet>();
                if (bullet && CurrentEffect.isTargeting)
                {
                    var target = FindNearestTarget("Respawn");
                    if (target)
                    {
                        bullet.target = target.transform;
                    }
                }
            }
        }

        public Vector3 GetMouseTargetPos()
        {
            if (Mouse.current == null)
            {
                return Vector3.zero;
            }

            var mousePosition = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(mousePosition);
            return Physics.Raycast(ray, out var hit, 100) ? hit.point : Vector3.zero;
        }

        public GameObject FindNearestTarget(string tag)
        {
            var gameObjects = GameObject.FindGameObjectsWithTag(tag).ToList().OrderBy(
                x => Vector3.Distance(transform.position, x.transform.position));

            return gameObjects.FirstOrDefault();
        }
    }
}
