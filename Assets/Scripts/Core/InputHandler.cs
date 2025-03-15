using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLC.Bad4Business.Core
{
    public class InputHandler : MonoBehaviour, PlayerControls.IGameActions
    {
        public PlayerControls PlayerControls { get; private set; }
        public PlayerControls.GameActions GameActions { get; private set; }


        public event Action OnJumpPressed;
        public event Action OnDashPressed;


        public Vector2 MouseDelta { get; private set; }
        public Vector2 InputVector { get; private set; }
        public bool InputDetected => InputVector != Vector2.zero;


        private readonly Coroutine m_disableActionCoroutine;

        #region Built-In Methods
        private void Awake()
        {
            PlayerControls = new PlayerControls();
            GameActions = PlayerControls.Game;
        }

        private void OnEnable()
        {
            GameActions.SetCallbacks(this);
            PlayerControls.Enable();
        }

        private void OnDisable()
        {
            GameActions.RemoveCallbacks(this);
            PlayerControls.Disable();
        }
        #endregion

        public void OnJump(InputAction.CallbackContext t_context)
        {
            if (t_context.started)
            {
                OnJumpPressed?.Invoke();
            }
        }

        public void OnMove(InputAction.CallbackContext t_context)
        {
            InputVector = t_context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext t_context)
        {
            MouseDelta = t_context.ReadValue<Vector2>();
        }

        public void OnShoot(InputAction.CallbackContext t_context)
        {
            if (t_context.performed)
            {

            }
        }

        public void OnDash(InputAction.CallbackContext t_context)
        {
            if (t_context.started)
            {
                OnDashPressed?.Invoke();
            }
        }


        public Vector3 GetMoveDirection()
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;

            forward.y = 0; // Flatten to prevent unwanted vertical movement
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            return ((forward * InputVector.y) + (right * InputVector.x)).normalized;
        }




        #region Utilities
        public void DisableActionFor(InputAction t_action, float t_seconds)
        {
            if (m_disableActionCoroutine != null)
            {
                StopCoroutine(m_disableActionCoroutine);
            }

            StartCoroutine(DisableAction(t_action, t_seconds));
        }

        private IEnumerator DisableAction(InputAction t_action, float t_seconds)
        {
            if (t_action == null)
            {
                yield break;
            }

            t_action.Disable();
            yield return new WaitForSeconds(t_seconds);
            t_action.Enable();
        }
        #endregion
    }
}