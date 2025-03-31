using SLC.Bad4Business.Core;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.AI
{
    public class ProjectileStandard : ProjectileBase
    {
        [Header("General Settings")]
        public float radius = 0.01f;
        public Transform root;
        public Transform tip;
        public float maxLifeTime = 5.0f;
        public LayerMask hittableLayers = -1;

        [Header("VFX & SFX")]
        public GameObject impactVfx;
        public float impactVfxLifetime = 5.0f;
        public float impactVfxSpawnOffset = 0.1f;
        public AudioClip impactSfxClip;

        [Header("Movement")]
        public float speed = 20.0f;
        public float gravityDownAcceleration = 0f;
        public bool inheritWeaponVelocity = false;

        [Header("Damage")]
        public float damage = 40.0f;

        private Vector3 m_velocity;
        private Vector3 m_lastRootPosition;
        private HashSet<Collider> m_ignoredColliders;
        private ProjectileBase m_projectileBase;

        private const QueryTriggerInteraction k_triggerInteraction = QueryTriggerInteraction.Collide;

        private void Awake()
        {
            m_ignoredColliders = new HashSet<Collider>();
        }

        private void OnEnable()
        {
            m_projectileBase = GetComponent<ProjectileBase>();
            m_projectileBase.OnShoot += OnShoot;

            Destroy(gameObject, maxLifeTime);
        }

        private void OnDisable()
        {
            if (m_projectileBase != null)
                m_projectileBase.OnShoot -= OnShoot;
        }

        private new void OnShoot()
        {
            m_velocity = transform.forward * speed;
            m_lastRootPosition = root.position;

            // Ignore owner's colliders
            m_ignoredColliders.Clear();
            Collider[] t_ownerColliders = m_projectileBase.Owner.GetComponentsInChildren<Collider>();
            foreach (Collider t_collider in t_ownerColliders)
            {
                m_ignoredColliders.Add(t_collider);
            }

            // Apply initial velocity from weapon if enabled
            if (inheritWeaponVelocity)
            {
                m_velocity += m_projectileBase.InheritedMuzzleVelocity;
            }
        }

        private void FixedUpdate()
        {
            MoveProjectile();
            DetectCollisions();
        }

        private void MoveProjectile()
        {
            Vector3 t_displacement = m_velocity * Time.fixedDeltaTime;
            transform.position += t_displacement;

            if (m_velocity.sqrMagnitude > 0.01f)
            {
                transform.forward = m_velocity.normalized;
            }

            // Apply gravity if enabled
            if (gravityDownAcceleration > 0)
            {
                m_velocity += gravityDownAcceleration * Time.fixedDeltaTime * Vector3.down;
            }
        }

        private void DetectCollisions()
        {
            Vector3 t_displacement = tip.position - m_lastRootPosition;
            float t_distance = t_displacement.magnitude;
            Vector3 t_direction = t_displacement / t_distance; // Normalize once manually

            if (Physics.SphereCast(m_lastRootPosition, radius, t_direction, out RaycastHit t_hit, t_distance, 
                hittableLayers, k_triggerInteraction))
            {
                if (IsHitValid(t_hit))
                {
                    OnHit(t_hit.point, t_hit.normal, t_hit.collider);
                }
            }

            m_lastRootPosition = root.position;
        }

        private bool IsHitValid(RaycastHit t_hit)
        {
            return !(t_hit.collider.isTrigger && t_hit.collider.GetComponent<Damageable>() == null) 
                && !m_ignoredColliders.Contains(t_hit.collider);
        }

        private void OnHit(Vector3 t_point, Vector3 t_normal, Collider t_collider)
        {
            Damageable t_damageable = t_collider.GetComponent<Damageable>();
            if (t_damageable)
            {
                t_damageable.InflictDamage(damage, m_projectileBase.Owner);
            }

            if (impactVfx)
            {
                GameObject t_impactVfxInstance = Instantiate(impactVfx, t_point + t_normal * impactVfxSpawnOffset, Quaternion.LookRotation(t_normal));
                if (impactVfxLifetime > 0)
                {
                    Destroy(t_impactVfxInstance, impactVfxLifetime);
                }
            }

            // Play impact sound (if added)
            if (impactSfxClip)
            {
                AudioSource.PlayClipAtPoint(impactSfxClip, t_point);
            }

            // Destroy projectile (or use object pooling instead)
            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan * 0.2f;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}