using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pickable : MonoBehaviour, IPickable
    {
        public GameObject player;
        public Transform holdParent;

        private void Awake()
        {
            m_rigidBody = GetComponent<Rigidbody>();
        }

        private Rigidbody m_rigidBody;

        public Rigidbody Rigid
        {
            get => m_rigidBody;
            set => m_rigidBody = value;
        }

        public void OnPickup()
        {
            m_rigidBody.isKinematic = true;

            transform.parent = holdParent.transform;


            Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }

        public void OnHold()
        {
            
        }

        public void OnRelease()
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>(), false);

            m_rigidBody.isKinematic = false;
            transform.parent = null;
        }
    }
}