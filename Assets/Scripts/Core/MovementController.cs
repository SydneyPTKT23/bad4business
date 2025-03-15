using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class MovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7.0f;
        [SerializeField] private float jumpForce = 10.0f;

        [SerializeField] private float dashSpeed = 15.0f;
        [SerializeField] private float dashDuration = 0.3f;
        [SerializeField] private int maxDashes = 2;
        [SerializeField] private float dashRechargeTime = 2.0f;

        [Space, Header("Ground Settings")]
        [SerializeField] private float gravityMultiplier = 2.5f;
        [SerializeField] private float stickToGroundForce = 5.0f;
        [Space]
        [SerializeField] private LayerMask groundLayer = ~0;
        [SerializeField] private float rayLength = 0.1f;
        [SerializeField] private float raySphereRadius = 0.1f;

        [Space, Header("Wallrun Settings")]
        [SerializeField] private float wallRunGravity = 1.0f;
        [SerializeField] private float wallRunSpeed = 6.0f;
        [SerializeField] private float wallJumpForce = 12.0f;
        [SerializeField] private float maxWallRunTime = 2.0f;
        [SerializeField] private float wallRunRayLength = 0.7f;
        [SerializeField] private LayerMask wallLayer;

        private bool isWallRunning;
        private float wallRunTimer;
        private Vector3 wallNormal;

        [Space, Header("Smooth")]
        [SerializeField] private float smoothInputSpeed = 10.0f;
        [SerializeField] private float smoothVelocitySpeed = 10.0f;
        [SerializeField] private float smoothFinalDirectionSpeed = 10.0f;

        private CharacterController m_characterController;
        private InputHandler m_inputHandler;
        private Health m_health;

        private RaycastHit m_hitInfo;

        [Space, Header("DEBUG")]
        [SerializeField] private Vector2 m_inputVector;
        [SerializeField] private Vector2 m_smoothInputVector;

        [Space]
        [SerializeField] private Vector3 m_smoothFinalMoveDir;
        [SerializeField] private Vector3 m_finalMoveVector;

        [Space]
        [SerializeField] private float m_currentSpeed;
        [SerializeField] private float m_smoothCurrentSpeed;

        [Space]
        [SerializeField] private float m_finalRayLength;
        [SerializeField] private bool m_isGrounded;
        [SerializeField] private bool m_previouslyGrounded;
        [SerializeField] private bool m_isDashing;
        [SerializeField] private int m_dashCount;

        [SerializeField] private bool isRegeneratingDash = false;

        [SerializeField] private float m_inAirTimer;

        private readonly float killHeight = -50.0f;
        public bool IsDead { get; private set; }


        private void Start()
        {
            m_characterController = GetComponent<CharacterController>();
            m_inputHandler = GetComponent<InputHandler>();

            m_health = GetComponent<Health>();
            m_health.OnDie += OnDie;

            //m_inputHandler.OnJumpPressed += HandleJump;
            //m_inputHandler.OnDashPressed += HandleDash;

            m_finalRayLength = rayLength + m_characterController.center.y;
            m_dashCount = maxDashes;
        }

        private void Update()
        {
            if (IsDead) return;

            // Autokill player if they manage to fall out of the map to prevent softlocking.
            if (!IsDead && transform.position.y < killHeight)
            {
                m_health.Kill();
            }

            SmoothDirection();
            SmoothInput();
            SmoothSpeed();

            CheckIfGrounded();
            CheckWallRun();

            if (!m_isDashing)
            {
                if (isWallRunning)
                {
                    HandleWallRun();
                }
                else
                {
                    HandleMovement();
                    HandleJump();
                }              
            }

            HandleDash();

            if (!isWallRunning)
                ApplyGravity();

            ApplyMovement();

            if (m_isGrounded && !IsInvoking(nameof(RegenerateDash)))
            {
                InvokeRepeating(nameof(RegenerateDash), dashRechargeTime, dashRechargeTime);
            }
        }

        private void OnDie()
        {
            IsDead = true;
        }

        private void CheckIfGrounded()
        {
            // Manually check for grounded because the CharacterController default is less reliable.
            Vector3 t_origin = transform.position + m_characterController.center;
            m_isGrounded = Physics.SphereCast(t_origin, raySphereRadius, Vector3.down, out m_hitInfo, m_finalRayLength, groundLayer);

            #if UNITY_EDITOR
            Debug.DrawRay(t_origin, Vector3.down * rayLength, Color.red);
            #endif
        }

        private void CheckWallRun()
        {
            if (m_isGrounded || m_finalMoveVector.y > 0)
            {
                isWallRunning = false;
                return;
            }

            RaycastHit leftHit, rightHit;
            bool leftWall = Physics.Raycast(transform.position, -transform.right, out leftHit, wallRunRayLength, wallLayer);
            bool rightWall = Physics.Raycast(transform.position, transform.right, out rightHit, wallRunRayLength, wallLayer);

            if (leftWall || rightWall && m_inputVector != Vector2.zero)
            {
                wallNormal = leftWall ? leftHit.normal : rightHit.normal;
                isWallRunning = true;
                wallRunTimer = maxWallRunTime;
            }
            else
            {
                isWallRunning = false;
            }
        }

        private void SmoothInput()
        {
            m_inputVector = m_inputHandler.InputVector.normalized;
            m_smoothInputVector = Vector2.Lerp(m_smoothInputVector, m_inputVector, Time.deltaTime * smoothInputSpeed);
        }

        private void SmoothSpeed()
        {
            m_smoothCurrentSpeed = Mathf.Lerp(m_smoothCurrentSpeed, m_currentSpeed, Time.deltaTime * smoothVelocitySpeed);
        }

        private void SmoothDirection()
        {
            m_smoothFinalMoveDir = Vector3.Lerp(m_smoothFinalMoveDir, m_finalMoveVector, Time.deltaTime * smoothFinalDirectionSpeed);
        }

        private void HandleMovement()
        {
            Vector3 t_movementDirection = (transform.forward * m_smoothInputVector.y) + (transform.right * m_smoothInputVector.x);
            
            if (m_isGrounded)
                t_movementDirection = Vector3.ProjectOnPlane(t_movementDirection, m_hitInfo.normal);

            t_movementDirection *= m_smoothCurrentSpeed;
            m_finalMoveVector = new Vector3(t_movementDirection.x, m_finalMoveVector.y, t_movementDirection.z);

            if (m_isGrounded)
            {
                m_inAirTimer = 0.0f;
                m_finalMoveVector.y = Mathf.Max(m_finalMoveVector.y, -stickToGroundForce);
            }
        }

        private void HandleWallRun()
        {
            if (wallRunTimer <= 0)
            {
                isWallRunning = false;
                return;
            }
            wallRunTimer -= Time.deltaTime;
            Vector3 moveAlongWall = (Vector3.Cross(wallNormal, Vector3.up) * (Vector3.Dot(transform.right, wallNormal) > 0 ? 1 : -1)).normalized * wallRunSpeed;
            m_finalMoveVector = new Vector3(moveAlongWall.x, -wallRunGravity, moveAlongWall.z);
        }

        private void HandleDash()
        {
            if (m_dashCount > 0 && Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartCoroutine(DashRoutine());
            }
        }

        private IEnumerator DashRoutine()
        {
            m_isDashing = true;
            m_dashCount--;

            Vector3 t_dashDirection = (m_inputVector.sqrMagnitude > 0)
                ? ((transform.forward * m_smoothInputVector.y) + (transform.right * m_smoothInputVector.x)).normalized
                : transform.forward;

            m_finalMoveVector = t_dashDirection * dashSpeed;

            yield return new WaitForSeconds(dashDuration);

            m_isDashing = false;
        }

        private void RegenerateDash()
        {
            if (m_dashCount < maxDashes)
            {
                m_dashCount++;
            }
            else
            {
                CancelInvoke(nameof(RegenerateDash));
            }
        }

        private void HandleJump()
        {
            if (m_isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                m_finalMoveVector.y = jumpForce;
                m_isGrounded = false;
            }
        }

        private void ApplyGravity()
        {
            if (!m_isGrounded && m_finalMoveVector.y > Physics.gravity.y)
            {
                m_inAirTimer += Time.deltaTime;
                m_finalMoveVector += gravityMultiplier * Time.deltaTime * Physics.gravity;
            }
        }

        private void ApplyMovement()
        {
            m_characterController.Move(m_finalMoveVector * Time.deltaTime);
        }
    }
}