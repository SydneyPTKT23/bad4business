using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SLC.Bad4Business.Core
{
    public class EnemyController : MonoBehaviour
    {
        private Health m_health;
        private Actor m_actor;


        private void Start()
        {
            m_health = GetComponent<Health>();

            m_actor = GetComponent<Actor>();

            m_health.OnDie += OnDie;
            m_health.OnDamaged += OnDamaged;
        }

        private void Update()
        {
            
        }

        private void OnDamaged(int t_damage, GameObject t_damageSource)
        {
            if (t_damageSource && !t_damageSource.GetComponent<EnemyController>())
            {

            }
        }

        private void OnDie()
        {

            Destroy(gameObject);
        }
    }
}