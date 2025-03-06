using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour
    {
        [System.Serializable]
        public struct MovementSettings
        {
            [Tooltip("The speed the motor will reach through it's own acceleration")]
            public float TopSpeed;
            [Tooltip("How fast the motor will reach top speed")]
            public float Acceleration;
            [Tooltip("How fast the motor will stop when no input is given")]
            public float Deceleration;
        }

        public bool readyToJump;
        public float groundDrag;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7.0f;
        [SerializeField] private float jumpForce = 10.0f;

        [Header("Gravity Settings")]
        [SerializeField] private LayerMask groundLayer = ~0;
        [SerializeField] private float rayLength = 0.1f;
        [SerializeField] private float raySphereRadius = 0.1f;



        private Rigidbody m_rigidBody;
        private InputHandler m_inputHandler;
        private Health m_health;

        private CapsuleCollider m_collider;

        private RaycastHit m_hitInfo;

        [Space, Header("DEBUG")]

        public Vector3 movementDirection;

        public bool m_isGrounded;
        [SerializeField] private float m_finalRayLength;

        public float killHeight = -50.0f;
        public bool IsDead { get; private set; }

        private void Start()
        {
            m_rigidBody = GetComponent<Rigidbody>();
            m_inputHandler = GetComponent<InputHandler>();

            m_health = GetComponent<Health>();
            m_health.OnDie += OnDie;

            m_collider = GetComponent<CapsuleCollider>();
            m_finalRayLength = rayLength + m_collider.center.y;

            m_isGrounded = true;
        }

        private void Update()
        {
            // Autokill player if they manage to fall out of the map to prevent softlocking.
            if (!IsDead && transform.position.y < killHeight)
            {
                m_health.Kill();
            }

            if (m_rigidBody)
            {
                CheckIfGrounded();
                SpeedControl();

                if (Input.GetKeyDown(KeyCode.Space) && readyToJump && m_isGrounded)
                {
                    readyToJump = false;

                    Jump();

                    ResetJump();
                }

                m_rigidBody.drag = m_isGrounded ? groundDrag : 0f;
            }
        }

        private void FixedUpdate()
        {
            Movement();
        }

        private void OnDie()
        {
            IsDead = true;
        }

        private void CheckIfGrounded()
        {
            // Manually check for grounded because the CharacterController default is less reliable.
            Vector3 t_origin = transform.position + m_collider.center;
            bool t_hitGround = Physics.SphereCast(t_origin, raySphereRadius, Vector3.down, out m_hitInfo, m_finalRayLength, groundLayer);

            // Draw the groundcheck for convenience.
            Debug.DrawRay(t_origin, Vector3.down * rayLength, Color.red);
            m_isGrounded = t_hitGround;
        }

        private void Jump()
        {
            m_rigidBody.velocity = new Vector3(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);

            m_rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            readyToJump = true;
        }

        private void SpeedControl()
        {
            Vector3 t_flatVelocity = new(m_rigidBody.velocity.x, 0f, m_rigidBody.velocity.z);

            // Manually limit velocity to not go beyond the movement speed maximum.
            if(t_flatVelocity.magnitude > moveSpeed)
            {
                Vector3 t_desiredVelocity = t_flatVelocity.normalized * moveSpeed;
                m_rigidBody.velocity = new Vector3(t_desiredVelocity.x, m_rigidBody.velocity.y, t_desiredVelocity.z);
            }
        }

        private void Movement()
        {
            movementDirection = transform.forward * m_inputHandler.InputVector.y + transform.right * m_inputHandler.InputVector.x;

            m_rigidBody.AddForce(10f * moveSpeed * movementDirection, ForceMode.Force);
        }
    }
}