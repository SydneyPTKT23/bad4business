using SLC.Bad4Business.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace SLC.Bad4Business.AI
{
    [RequireComponent(typeof(Health), typeof(Actor), typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        public float selfDestructYHeight = -20.0f;
        public float rotationSpeed = 10.0f;
        public float attackRange = 2.0f;

        public float attackCooldown = 1.0f;
        private float m_attackTimer = 0f;

        public UnityAction onDetectedTarget;

        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
        public bool IsTargetVisible => DetectionModule.IsTargetVisible;

        public NavMeshAgent NavMeshAgent { get; private set; }
        public DetectionModule DetectionModule { get; private set; }


        private Health m_health;
        private Actor m_actor;
        [SerializeField] private Collider[] m_selfColliders;
        

        public MeleeAttackModule m_meleeAttack;
        public RangedAttackModule m_rangedAttack;


        private bool m_wasDamagedThisFrame;

        void Start()
        {
            m_health = GetComponent<Health>();
            m_actor = GetComponent<Actor>();

            NavMeshAgent = GetComponent<NavMeshAgent>();
            m_selfColliders = GetComponentsInChildren<Collider>();

            // Subscribe to damage & death events
            m_health.OnDamaged += OnDamaged;
            m_health.OnDie += OnDie;

            // Find and initialize attack modules
            m_meleeAttack = GetComponent<MeleeAttackModule>();


            DetectionModule = GetComponentInChildren<DetectionModule>();

            // Initialize detection module
            DetectionModule.OnDetectedTarget += OnDetectedTarget;
    
        }

        private void Update()
        {
            EnsureIsWithinLevelBounds();

            DetectionModule.HandleTargetDetection(m_actor, m_selfColliders);

            // Update the attack cooldown timer
            if (m_attackTimer > 0f)
            {
                m_attackTimer -= Time.deltaTime;
            }

            m_wasDamagedThisFrame = false;
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


        private void OnDetectedTarget()
        {
            onDetectedTarget.Invoke();
        }

        public void RotateTowards(Vector3 t_targetPosition)
        {
            Vector3 t_direction = Vector3.ProjectOnPlane(t_targetPosition - transform.position, Vector3.up).normalized;
            if (t_direction.sqrMagnitude > 0f)
            {
                Quaternion t_targetRotation = Quaternion.LookRotation(t_direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, t_targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        public void SetNavDestination(Vector3 t_destination)
        {
            if (NavMeshAgent)
            {
                NavMeshAgent.SetDestination(t_destination);
            }
        }


        private void OnDamaged(float t_damage, GameObject t_damageSource)
        {
            // test if the damage source is the player
            if (t_damageSource && !t_damageSource.GetComponent<EnemyController>())
            {
                // pursue the player
                DetectionModule.OnDamaged(t_damageSource);
            }
        }

        private void OnDie()
        {

            // this will call the OnDestroy function
            Destroy(gameObject);
        }

        public bool TryAttack(Vector3 t_targetPosition)
        {
            // Check if enough time has passed since the last attack (based on cooldown)
            if (m_attackTimer <= 0f)
            {
                // Perform the attack (you can use melee or ranged based on your needs)
                if (m_meleeAttack != null && KnownDetectedTarget != null)
                {
                    m_meleeAttack.PerformAttack();
                }

                // Reset the attack cooldown timer
                m_attackTimer = attackCooldown;

                return true;
            }

            return false;
        }

        public bool IsTargetInAttackRange()
        {
            return KnownDetectedTarget != null && Vector3.Distance(transform.position, KnownDetectedTarget.transform.position) <= attackRange;
        }
    }
}
