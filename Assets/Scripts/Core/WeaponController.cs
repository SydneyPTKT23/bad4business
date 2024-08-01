using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class WeaponController : MonoBehaviour
    {
        public string weaponName;

        public GameObject weaponRoot;
        public Transform weaponMuzzle;

        public float fireRate = 0.15f;
        public float bulletSpreadAngle = 0.0f;

        public int bulletsPerShot = 1;
        public int bulletsPerClip = 1;

        public int maxAmmo;
        
        public GameObject shellCasing;
        public Transform ejectionPoint;

        private int m_currentAmmo;
        private float m_nextTimeToFire = Mathf.NegativeInfinity;

        public GameObject Owner { get; set; }
        public GameObject SourcePrefab { get; set; }
        public bool IsWeaponActive { get; private set; }

        public Camera m_weaponCamera;

        private void Start()
        {
            m_currentAmmo = maxAmmo;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleShoot();
            }
        }

        public void ShowWeapon(bool t_state)
        {
            weaponRoot.SetActive(t_state);

            IsWeaponActive = t_state;
        }
        
        private void HandleShoot()
        {
            if (m_currentAmmo >= 1.0f && Time.time >= m_nextTimeToFire)
            {
                Shoot();
                m_nextTimeToFire = Time.time + fireRate;

                m_currentAmmo -= 1;
            }
        }

        private void Shoot()
        {
            WeaponManager t_weaponManager = Owner.GetComponent<WeaponManager>();
            if (t_weaponManager)
            {
                m_weaponCamera = t_weaponManager.weaponCamera;
            }

            for (int i = 0; i < bulletsPerShot; i++)
            {
                float t_angle = Random.Range(-bulletSpreadAngle, bulletSpreadAngle);
                Vector3 t_direction = m_weaponCamera.transform.forward + new Vector3(t_angle, t_angle, 0);

                if (Physics.Raycast(m_weaponCamera.transform.position, t_direction, out RaycastHit t_hitInfo, 100))
                {
                    Debug.Log(t_hitInfo.transform.name);

                    Health t_target = t_hitInfo.transform.GetComponent<Health>();
                    if (t_target != null)
                    {
                        t_target.DealDamage(10);
                    }
                }
            }
        }
    }
}