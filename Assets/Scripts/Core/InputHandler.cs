using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLC.Bad4Business.Core
{
    public class InputHandler : MonoBehaviour, PlayerControls.IGameActions
    {
        public PlayerControls PlayerControls { get; private set; }
        public PlayerControls.GameActions GameActions { get; private set; }

        public Vector2 MouseDelta { get; private set; }

        public Vector2 InputVector { get; private set; }
        public bool InputDetected => InputVector != Vector2.zero;

        #region Built-In Methods
        private void Awake()
        {
            if (PlayerControls != null)
            {
                return;
            }

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

        }



        public void DisableActionFor(InputAction t_action, float t_seconds)
        {
            StartCoroutine(DisableAction(t_action, t_seconds));
        }

        private IEnumerator DisableAction(InputAction t_action, float t_seconds)
        {
            t_action.Disable();

            yield return new WaitForSeconds(t_seconds);

            t_action.Enable();
        }
    }
}