using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class WeaponController : MonoBehaviour
    {
        public int damage = 10;
        public float range = 100f;
        public float fireRate = 0.15f;

        public int bulletsPerShot = 1;
        public float bulletSpread = 0f;

        private float m_nextTimeToFire = 0f;

        public Camera m_cam;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleShoot();
            }
        }
        
        private void HandleShoot()
        {
            if (Time.time >= m_nextTimeToFire)
            {
                m_nextTimeToFire = Time.time + fireRate;
                Shoot();
            }
        }

        private void Shoot()
        {
            for (int i = 0; i < bulletsPerShot; i++)
            {

                RaycastHit t_hitInfo = new();
                if (Physics.Raycast(m_cam.transform.position, m_cam.transform.forward, out t_hitInfo, range))
                {
                    Debug.Log(t_hitInfo.transform.name);

                    Health t_target = t_hitInfo.transform.GetComponent<Health>();
                    if (t_target != null)
                    {
                        t_target.DealDamage(damage);
                    }
                }
            }
        }
    }
}