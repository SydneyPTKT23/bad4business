using UnityEngine;

namespace SLC.Bad4Business.AI
{
    public class RangedAttackModule : MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;

        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float attackRange = 15f;
        [SerializeField] private LayerMask targetLayer;

        private float lastAttackTime = 0f;

        public GameObject Owner;
        public Vector3 MuzzleVelocity;

        public void PerformAttack(Vector3 targetPosition)
        {
            if (Time.time < lastAttackTime + attackCooldown) return;
            if (projectilePrefab == null || firePoint == null) return;

            GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            ProjectileBase projectile = projectileInstance.GetComponent<ProjectileBase>();
            if (projectile == null)
                return;

            // Set projectile direction towards the target
            Vector3 direction = (targetPosition - firePoint.position).normalized;
            projectileInstance.transform.forward = direction;

            // Initialize the projectile
            projectile.Shoot(this);
        }

        private void OnDrawGizmos()
        {
            if (firePoint == null) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(firePoint.position, firePoint.forward * attackRange);
        }
    }
}
