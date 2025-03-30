using SLC.Bad4Business.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.AI
{
    public class MeleeAttackModule : MonoBehaviour
    {
        [SerializeField] private float attackWidth = 1.5f;
        [SerializeField] private float attackHeight = 1.5f;
        [SerializeField] private float attackRange = 2.0f;
        [SerializeField] private int attackDamage = 10;

        [SerializeField] private LayerMask playerLayer;

        public void PerformAttack()
        {
            Vector3 boxCenter = transform.position + transform.forward * (attackRange / 2) + Vector3.up * (attackHeight / 2);
            Vector3 boxSize = new Vector3(attackWidth, attackHeight, attackRange);

            Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2, transform.rotation, playerLayer);

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<Damageable>()?.InflictDamage(attackDamage, gameObject);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 boxCenter = transform.position + transform.forward * (attackRange / 2) + Vector3.up * (attackHeight / 2);
            Vector3 boxSize = new Vector3(attackWidth, attackHeight, attackRange);
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
}
