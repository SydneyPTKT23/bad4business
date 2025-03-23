using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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


        public UnityAction OnShoot;
        public event Action OnShootProcessed;


        private int m_currentAmmo;
        private float m_nextTimeToFire = Mathf.NegativeInfinity;

        public GameObject Owner { get; set; }
        public GameObject SourcePrefab { get; set; }
        public bool IsWeaponActive { get; private set; }

        public Camera m_weaponCamera;


        private AudioSource m_shootAudioSource;
        public bool IsReloading { get; private set; }



        private void Start()
        {
            Owner = GetComponentInParent<MovementController>().gameObject;

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
            /*WeaponManager t_weaponManager = Owner.GetComponent<WeaponManager>();
            if (t_weaponManager)
            {
                m_weaponCamera = t_weaponManager.weaponCamera;
            }
            */

            for (int i = 0; i < bulletsPerShot; i++)
            {
                Vector3 t_direction = GetDirectionWithinSpread(m_weaponCamera.transform);

                Debug.DrawRay(m_weaponCamera.transform.position, t_direction * 100, Color.blue, 1.0f);
                if (Physics.Raycast(m_weaponCamera.transform.position, t_direction, out RaycastHit t_hitInfo, 100))
                {
                    

                    //Debug.Log(t_hitInfo.transform.name);
                    

                    Damageable t_target = t_hitInfo.transform.GetComponent<Damageable>();
                    if (t_target != null)
                    {
                        t_target.InflictDamage(10f, Owner);
                    }
                }
            }

            OnShoot?.Invoke();
            OnShootProcessed?.Invoke();
        }

        public Vector3 GetDirectionWithinSpread(Transform t_shootTransform)
        {
            float t_angleRatio = bulletSpreadAngle / 180.0f;
            Vector3 t_spreadDirection = Vector3.Slerp(t_shootTransform.forward, UnityEngine.Random.insideUnitSphere, t_angleRatio);

            return t_spreadDirection;
        }
    }
}