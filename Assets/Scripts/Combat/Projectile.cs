using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [System.Serializable]
        public class ProjectileEvent : UnityEvent<ProjectilePhase> { }

        [SerializeField] float speed = 1f;
        [SerializeField] float damageBonus = 0f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 8f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;

        [SerializeField] UnityEvent onProjectileLaunch = null;
        [SerializeField] UnityEvent onProjectileHit = null;


        Health target = null;
        GameObject projectileInstegator = null;
        float damage = 0f;

        void Start()
        {
            if (onProjectileLaunch != null) onProjectileLaunch.Invoke();
            OrientProjectile();
        }

        void Update()
        {
            if (target == null) return;

            if (isHoming && target.isAlive)
            {
                OrientProjectile();
            }
            MoveProjectileForward();
        }

        Vector3 GetAimLocation()
        {
            if (target == null) return Vector3.zero;
            CapsuleCollider collider = target.GetComponent<CapsuleCollider>();
            return collider == null
                ? target.transform.position
                : target.transform.position + Vector3.up * collider.height / 2;
        }

        void OrientProjectile()
        {
            transform.LookAt(GetAimLocation());
        }

        void MoveProjectileForward()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        void PlayHitEffect()
        {
            if (hitEffect == null || target == null) return;
            if (onProjectileHit != null) onProjectileHit.Invoke();
            Instantiate(hitEffect, GetAimLocation(), transform.rotation);
        }

        void DestroyObjectsOnHit()
        {
            if (destroyOnHit == null) return;

            foreach (GameObject gameObj in destroyOnHit)
            {
                Destroy(gameObj);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (target != null && other.GetComponent<Health>() == target && target.isAlive)
            {
                speed = 0;
                target.TakeDamage(projectileInstegator, damage);
                PlayHitEffect();
                DestroyObjectsOnHit();
                Destroy(gameObject, lifeAfterImpact);
            }
        }

        public void SetTarget(GameObject instigator, Health projectileTarget, float weaponDamage)
        {
            target = projectileTarget;
            damage = weaponDamage + damageBonus;
            projectileInstegator = instigator;
            Destroy(gameObject, maxLifetime);
        }
    }
}
