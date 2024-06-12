using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class SpriteBillboard : MonoBehaviour
    {
        [SerializeField] private bool alignToCamera = true;

        private Transform m_cameraTransform;

        #region Built-In Methods
        private void Start()
        {
            m_cameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (alignToCamera)
            {
                transform.rotation = m_cameraTransform.rotation;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, m_cameraTransform.rotation.eulerAngles.y, 0f);
            }            
        }
        #endregion
    }
}