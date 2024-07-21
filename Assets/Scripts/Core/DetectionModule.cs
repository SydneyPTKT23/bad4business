using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SLC.Bad4Business.Core
{
    public class DetectionModule : MonoBehaviour
    {
        public float viewRadius = 10.0f;
        [Range(0, 360)] public float viewAngle = 50.0f;

        public List<Transform> m_targetsInView = new();

        public LayerMask targetMask;
        public LayerMask obstructionMask;

        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;

        public GameObject KnownDetectedTarget { get; private set; }

        private ActorManager m_actorManager;

        private void Start()
        {
            m_actorManager = FindObjectOfType<ActorManager>();
        }

        public void HandleTargetDetection()
        {

            m_targetsInView.Clear();
            Collider[] t_rangeCheck = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (int i = 0; i < t_rangeCheck.Length; i++)
            {
                Transform t_target = t_rangeCheck[i].transform;
                Vector3 t_directionToTarget = (t_target.position - transform.position).normalized;

                if (Vector3.Angle(t_directionToTarget, transform.forward) < viewAngle / 2)
                {
                    float t_distanceToTarget = Vector3.Distance(transform.position, t_target.position);
                    if (!Physics.Raycast(transform.position, t_directionToTarget, t_distanceToTarget, obstructionMask))
                    {
                        m_targetsInView.Add(t_target);
                    }
                }
            }
        }
    }
}