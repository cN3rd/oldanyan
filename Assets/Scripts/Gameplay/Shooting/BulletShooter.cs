using System.Collections;
using System.Linq;
using Game.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay.Shooting
{
    public struct ShootingArgs
    {
        public Vector3 targetPos;
        public GameObject originObject;
        public int damage;
    }

    public class BulletShooter : MonoBehaviour
    {
        public BulletEffectPrefabsCollection effectPrefabs;
        public int index;
        public Transform startNodeTrans;
        public float shootInterval = 0.2f;

        private float _lastShootTime;
        private BulletEffectPrefabs CurrentEffect => effectPrefabs.GetEffectByIndex(index);

        public void Shoot(ShootingArgs shootingArgs)
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

            StartCoroutine(ShootIE(shootingArgs));
        }

        public IEnumerator ShootIE(ShootingArgs shootingArgs)
        {
            if (!startNodeTrans)
            {
                Debug.LogError("StartShootPosition (startNodeTrans) is not assigned!");
                yield break;
            }

            _lastShootTime = Time.time;
            yield return Charge();

            DoShoot(shootingArgs);
        }

        public IEnumerator Charge()
        {
            if (!CurrentEffect.chargeParticlesPrefab)
            {
                yield break;
            }

            // Instantiate the charge effect at the exact position of the StartShootPosition
            var chargeInstance = Instantiate(CurrentEffect.chargeParticlesPrefab,
                startNodeTrans.position,
                Quaternion.identity);

            yield return new WaitForSeconds(CurrentEffect.chargeParticleTime);

            Destroy(chargeInstance);
        }

        public void DoShoot(ShootingArgs shootingArgs)
        {
            var targetDir = shootingArgs.targetPos - startNodeTrans.position;
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

                var bulletData = bulletInstance.GetComponent<Bullet>();
                var originCollider = shootingArgs.originObject.GetComponent<Collider>();

                bulletData.origin = shootingArgs.originObject;
                bulletData.bulletDamage = shootingArgs.damage;
                Physics.IgnoreCollision(bulletData.bulletCollider, originCollider, true);
            }
        }
    }
}
