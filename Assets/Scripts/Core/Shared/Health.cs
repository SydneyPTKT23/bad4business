using UnityEngine;
using UnityEngine.Events;

namespace SLC.Bad4Business.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maximumHealth = 100;
        [SerializeField] private float criticalHealthRatio = 0.3f;

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float> OnHealed;
        public UnityAction OnDie;

        public float CurrentHealth { get; set; }
        public bool Invincible { get; set; }

        public float GetHealthRatio() => CurrentHealth / maximumHealth;
        public bool IsCritical() => GetHealthRatio() <= criticalHealthRatio;

        private bool m_isDead;

        private void Start()
        {
            CurrentHealth = maximumHealth;
        }

        // Call before healing to prevent using healing items at full health.
        public bool CanAddHealth() => CurrentHealth < maximumHealth;
        public void AddHealth(float t_amount)
        {
            CurrentHealth += t_amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maximumHealth);

            // When adding health, call the OnHealed action.
            if (t_amount > 0)
            {
                OnHealed?.Invoke(t_amount);
            }
        }

        public void TakeDamage(float t_amount, GameObject t_damageSource)
        {
            if (Invincible) { return; }

            CurrentHealth -= t_amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maximumHealth);

            // Call the OnDamaged action for damage SFX or animations in other scripts.
            if (t_amount > 0)
            {
                OnDamaged?.Invoke(t_amount, t_damageSource);
            }

            HandleDeath();
        }

        // Call this function from other scripts to instant kill the character if needed.
        public void Kill()
        {
            CurrentHealth = 0;
            OnDamaged?.Invoke(maximumHealth, null);

            HandleDeath();
        }

        private void HandleDeath()
        {
            if (m_isDead) { return; }

            if (CurrentHealth <= 0f)
            {
                // Same thing with the actions as healing and damage.
                m_isDead = true;
                OnDie?.Invoke();
            }
        }
    }
}