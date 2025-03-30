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

        public bool IsTargetInAttackRange(Vector3 targetPosition)
        {
            return Vector3.Distance(transform.position, targetPosition) <= attackRange;
        }

        public void PerformAttack(Vector3 targetPosition)
        {
            if (Time.time < lastAttackTime + attackCooldown) return;
            if (projectilePrefab == null || firePoint == null) return;

            // Calculate direction to target
            Vector3 direction = (targetPosition - firePoint.position).normalized;

            // Instantiate and shoot the projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }

            lastAttackTime = Time.time;
        }

        private void OnDrawGizmos()
        {
            if (firePoint == null) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(firePoint.position, firePoint.forward * attackRange);
        }
    }
}
