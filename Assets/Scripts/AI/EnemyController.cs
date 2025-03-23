using SLC.Bad4Business.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SLC.Bad4Business.AI
{
    [RequireComponent(typeof(Health), typeof(Actor), typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        public float selfDestructYHeight = -20.0f;


        [SerializeField] private float attackRange = 2.0f;
        public GameObject KnownDetectedTarget => m_detectionModule.KnownDetectedTarget;
        private DetectionModule m_detectionModule;
        private NavMeshAgent m_agent;

        private Health m_health;
        [SerializeField] private Collider[] m_selfColliders;


        void Start()
        {
            m_health = GetComponent<Health>();
            m_health.OnDamaged += OnDamaged;
            m_health.OnDie += OnDie;

            m_selfColliders = GetComponentsInChildren<Collider>();

            m_agent = GetComponent<NavMeshAgent>();

            m_detectionModule = GetComponentInChildren<DetectionModule>();
            m_detectionModule.OnDetectedTarget += HandleTargetSpotted;
            m_detectionModule.OnLostTarget += HandleTargetLost;
        }

        private void Update()
        {
            EnsureIsWithinLevelBounds();

            m_detectionModule.HandleTargetDetection(m_selfColliders);
        }

        private void EnsureIsWithinLevelBounds()
        {
            // At every frame, this tests for conditions to kill the enemy
            if (transform.position.y < selfDestructYHeight)
            {
                Destroy(gameObject);
                return;
            }
        }


        private void HandleTargetSpotted()
        {
            
        }

        private void HandleTargetLost()
        {
            
        }

        private void HandleForgetTarget()
        {
            
        }

        public void MoveToDestination(Vector3 t_destination)
        {
            if (m_agent)
            {
                m_agent.SetDestination(t_destination);
            }
        }


        private void OnDamaged(float t_damage, GameObject t_damageSource)
        {
            // test if the damage source is the player
            if (t_damageSource && !t_damageSource.GetComponent<EnemyController>())
            {
                // pursue the player
                m_detectionModule.OnDamaged(t_damageSource);

                //onDamaged?.Invoke();

                // play the damage tick sound
                //if (DamageTick && !m_WasDamagedThisFrame)
                //    AudioUtility.CreateSFX(DamageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);
            }
        }

        private void OnDie()
        {
            // spawn a particle system when dying
            //var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
            //Destroy(vfx, 5f);

            // tells the game flow manager to handle the enemy destuction
            //m_enemyManager.UnregisterEnemy(this);

            // loot an object
            

            // this will call the OnDestroy function
            Destroy(gameObject, 0.2f);
        }

        private void TryAttack()
        {
            /*
            if (canAttack)
            {
                OnAttack?.Invoke();
                canAttack = false;
                Invoke(nameof(ResetAttack), attackCooldown);
            }
            */
        }

        public bool IsTargetInAttackRange()
        {
            return KnownDetectedTarget != null && Vector3.Distance(transform.position, KnownDetectedTarget.transform.position) <= attackRange;
        }
    }
}
