using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.AI
{
    public class MeleeAttackModule : MonoBehaviour
    {
        [SerializeField] private float attackRange = 2.0f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private int attackDamage = 10;
        private bool canAttack = true;

        public void PerformAttack()
        {

        }
    }
}
