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
        public Transform startNodeTrans;
        public float shootInterval = 0.2f;

        private float _lastShootTime;
        private BulletEffectPrefabs CurrentEffect => effectPrefabs.GetEffectByIndex(index);

        public void Shoot(Vector3 targetPos)
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

            StartCoroutine(ShootIE(targetPos));
        }

        public IEnumerator ShootIE(Vector3 targetPos)
        {
            if (!startNodeTrans)
            {
                Debug.LogError("StartShootPosition (startNodeTrans) is not assigned!");
                yield break;
            }

            _lastShootTime = Time.time;
            yield return Charge();

            DoShoot(targetPos);
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

        public void DoShoot(Vector3 targetPos)
        {
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

                var bulletData = bulletInstance.GetComponent<MyBullet>();
                bulletData.origin = this.gameObject;
            }
        }
    }
}
