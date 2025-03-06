using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class CameraController : MonoBehaviour
    {
        public InputHandler handler;

        public Transform pivot;

        public float sensitivity = 10;

        private float m_mouseX;
        private float m_mouseY;

        private float m_desiredPitch;
        private float m_desiredYaw;

        public Transform m_player;

        private void Awake()
        {
            ChangeCursorState();
        }

        private void LateUpdate()
        {
            m_mouseX = handler.MouseDelta.x * sensitivity * Time.deltaTime;
            m_mouseY = handler.MouseDelta.y * sensitivity * Time.deltaTime;

            m_desiredYaw += m_mouseX;

            m_desiredPitch -= m_mouseY;
            m_desiredPitch = Mathf.Clamp(m_desiredPitch, -89.9f, 89.9f);

            transform.localRotation = Quaternion.Euler(m_desiredPitch, m_desiredYaw, 0);

            transform.position = pivot.position;
        }

        private void FixedUpdate()
        {
            m_player.localRotation = Quaternion.Euler(0, m_desiredYaw, 0);
        }

        private void ChangeCursorState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}