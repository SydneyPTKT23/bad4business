using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SLC.Bad4Business.Core
{
    public class DetectionModule : MonoBehaviour
    {
        public float radius = 10.0f;
        [Range(0, 360)] public float angle = 50.0f;

        public List<Transform> visibleTargets = new();

        public LayerMask targetMask;
        public LayerMask obstructionMask;

        public bool isPlayerVisible;

        private void Update()
        {
            FindVisibleTargets();
        }

        private void FindVisibleTargets()
        {
            visibleTargets.Clear();

            Collider[] t_rangeCheck = Physics.OverlapSphere(transform.position, radius, targetMask);

            for (int i = 0; i < t_rangeCheck.Length; i++)
            {
                Transform t_target = t_rangeCheck[i].transform;
                Vector3 t_directionToTarget = (t_target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, t_directionToTarget) < angle / 2)
                {
                    float t_distanceToTarget = Vector3.Distance(transform.position, t_target.position);
                    if (!Physics.Raycast(transform.position, t_directionToTarget, t_distanceToTarget, obstructionMask))
                    {
                        visibleTargets.Add(t_target);
                    }
                }
            }
        }
    }
}
