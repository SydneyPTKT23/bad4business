using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.AI
{
    public class EnemyRusher : MonoBehaviour
    {
        public enum AIState
        {
            Idle,
            Chase,
            Attack
        }

        public AIState CurrentState { get; private set; }
        private EnemyController m_enemyController;

        private void Awake()
        {
            m_enemyController = GetComponent<EnemyController>();

            
        }

        private void Start()
        {
            ChangeState(AIState.Idle);
        }

        private void Update()
        {
            UpdateCurrentState();
        }

        private void UpdateCurrentState()
        {
            switch (CurrentState)
            {
                case AIState.Idle:
                    if (m_enemyController.KnownDetectedTarget != null)
                        ChangeState(AIState.Chase);
                    break;
                case AIState.Chase:
                    m_enemyController.MoveToDestination(m_enemyController.KnownDetectedTarget.transform.position);
                    if (m_enemyController.IsTargetInAttackRange())
                        ChangeState(AIState.Attack);
                    break;
                case AIState.Attack:
                    if (!m_enemyController.IsTargetInAttackRange())
                        ChangeState(AIState.Chase);
                    else
                        Debug.Log("attack");
                    break;
            }
        }

        private void ChangeState(AIState t_newState)
        {
            CurrentState = t_newState;
        }
    }
}
