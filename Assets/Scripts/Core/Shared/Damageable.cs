using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class Damageable : MonoBehaviour
    {
        public float damageMultiplier = 1.0f;
        public Health Health { get; private set; }

        private void Awake()
        {
            // find the health component either at the same level, or higher in the hierarchy
            Health = GetComponent<Health>();
            if (!Health)
            {
                Health = GetComponentInParent<Health>();
            }
        }

        public void InflictDamage(float t_damage, GameObject t_damageSource)
        {
            if (Health)
            {
                Health.TakeDamage(t_damage, t_damageSource);
            }
        }
    }
}