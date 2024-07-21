using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private float rayDistance = 2.0f;
        [SerializeField] private float raySphereRadius = 0.1f;
        [SerializeField] private LayerMask interactableLayer = ~0;

        private Camera m_camera;

        private bool m_isInteracting;
        private float m_holdingTimer = 0.0f;

        private void Awake()
        {
            m_camera = GetComponentInChildren<Camera>();
        }

        private void Update()
        {
            CheckForInteractables();
        }

        private void CheckForInteractables()
        {
            Ray t_ray = new(m_camera.transform.position, m_camera.transform.forward);
            bool t_hitSomething = Physics.SphereCast(t_ray, raySphereRadius, out RaycastHit t_hitInfo, rayDistance, interactableLayer);

            if (t_hitSomething)
            {
                InteractableBase t_interactable = t_hitInfo.transform.GetComponent<InteractableBase>();

                if (t_interactable != null)
                {
                    
                }
            }
            else
            {
                
            }

            Debug.DrawRay(t_ray.origin, t_ray.direction * rayDistance, t_hitSomething ? Color.green : Color.red);
        }
    }
}
