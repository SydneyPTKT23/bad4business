using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SLC.Bad4Business.Core
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private Transform throwingParent;
        public List<WeaponController> startingWeapons = new();

        public Camera weaponCamera;

        public Transform weaponParentSocket;

        public int ActiveWeaponIndex { get; private set; }

        public LayerMask weaponLayer;

        public UnityAction<WeaponController> OnSwitchdToWeapon;
        public UnityAction<WeaponController, int> OnAddedWeapon;
        public UnityAction<WeaponController, int> OnRemovedWeapon;

        public WeaponController[] m_weaponSlots = new WeaponController[2];
        private InputHandler m_inputHandler;

        int m_weaponSwitchIndex;

        private void Start()
        {
            ActiveWeaponIndex = -1;

            m_inputHandler = GetComponent<InputHandler>();


            OnSwitchdToWeapon += OnWeaponSwitched;

            foreach (WeaponController t_weapon in startingWeapons)
            {
                _ = AddWeapon(t_weapon);
            }


        }

        private void Update()
        {
            WeaponController t_activeWeapon = GetActiveWeapon();

            UpdateWeaponSwitching();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchWeapon(true);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                RemoveWeapon(t_activeWeapon);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchToWeaponIndex(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchToWeaponIndex(1);
            }
        }

        // Find the next valid weapon slot to switch to.
        public void SwitchWeapon(bool t_order)
        {
            int t_newWeaponIndex = -1;
            int t_closestSlotDistance = m_weaponSlots.Length;

            for (int i = 0; i < m_weaponSlots.Length; i++)
            {
                if (i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
                {
                    int t_distanceToActiveIndex = GetDistanceBetweenSlots(ActiveWeaponIndex, i, t_order);

                    if (t_distanceToActiveIndex < t_closestSlotDistance)
                    {
                        t_closestSlotDistance = t_distanceToActiveIndex;
                        t_newWeaponIndex = i;
                    }
                }
            }

            SwitchToWeaponIndex(t_newWeaponIndex);
        }

        public void SwitchToWeaponIndex(int t_newWeaponIndex, bool t_force = false)
        {
            if (t_force || (t_newWeaponIndex != ActiveWeaponIndex && t_newWeaponIndex >= 0))
            {
                m_weaponSwitchIndex = t_newWeaponIndex;

                if (GetActiveWeapon() == null)
                {
                    ActiveWeaponIndex = m_weaponSwitchIndex;

                    WeaponController t_newWeapon = GetWeaponAtSlotIndex(m_weaponSwitchIndex);
                    if (OnSwitchdToWeapon != null)
                    {
                        OnSwitchdToWeapon.Invoke(t_newWeapon);
                    }
                }
            }
        }

        private void UpdateWeaponSwitching()
        {
            WeaponController t_oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
            if (t_oldWeapon != null)
            {
                t_oldWeapon.ShowWeapon(false);
            }

            ActiveWeaponIndex = m_weaponSwitchIndex;

            WeaponController t_newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
            if (OnSwitchdToWeapon != null)
            {
                OnSwitchdToWeapon.Invoke(t_newWeapon);
            }
        }

        public bool AddWeapon(WeaponController t_prefab)
        {
            // Search through weapon slots for an empty one.
            for (int i = 0; i < m_weaponSlots.Length; i++)
            {
                // Only add the weapon if there is a free slot.
                if (m_weaponSlots[i] == null)
                {
                    WeaponController t_weaponInstance = Instantiate(t_prefab, weaponParentSocket);
                    t_weaponInstance.transform.localPosition = Vector3.zero;
                    t_weaponInstance.transform.localRotation = Quaternion.identity;

                    // Set weapon owner to this object to handle weapon logic.
                    t_weaponInstance.Owner = gameObject;
                    t_weaponInstance.SourcePrefab = t_weaponInstance.gameObject;
                    t_weaponInstance.ShowWeapon(false);

                    t_weaponInstance.m_weaponCamera = weaponCamera;

                    // Assign the weapon to the viewmodel layer.
                    foreach (Transform t_transform in t_weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        t_transform.gameObject.layer = 10;
                    }

                    m_weaponSlots[i] = t_weaponInstance;
                    t_weaponInstance.enabled = true;

                    if (OnAddedWeapon != null)
                    {
                        OnAddedWeapon.Invoke(t_weaponInstance, i);
                    }

                    return true;
                }
            }

            // Switch weapon automatically if no weapons are equipped.
            if (GetActiveWeapon() == null)
            {
                SwitchWeapon(true);
            }

            return false;
        }

        public bool RemoveWeapon(WeaponController t_weaponInstance)
        {
            for (int i = 0; i < m_weaponSlots.Length; i++)
            {
                if (m_weaponSlots[i] == t_weaponInstance)
                {
                    m_weaponSlots[i] = null;
                    t_weaponInstance.enabled = false;

                    if (OnRemovedWeapon != null)
                    {
                        OnRemovedWeapon.Invoke(t_weaponInstance, i);
                    }

                    Rigidbody t_rigidBody = t_weaponInstance.GetComponent<Rigidbody>();
                    t_rigidBody.isKinematic = false;

                    t_weaponInstance.transform.SetParent(null);
                    t_weaponInstance.transform.position = throwingParent.position;
                    t_weaponInstance.transform.rotation = Quaternion.identity;

                    t_weaponInstance.Owner = null;
                    t_rigidBody.AddForce(weaponCamera.transform.forward * 4f, ForceMode.Impulse);
                    t_rigidBody.AddForce(weaponCamera.transform.up * 4f, ForceMode.Impulse);

                    float t_random = Random.Range(-1.0f, 1.0f);
                    t_rigidBody.AddTorque(new Vector3(t_random, t_random, t_random) * 10f);

                    foreach (Transform t_transform in t_weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        t_transform.gameObject.layer = 0;
                    }

                    // Switch to next weapon when active weapon removed.
                    if (i == ActiveWeaponIndex)
                    {
                        SwitchWeapon(true);
                    }

                    return true;
                }
            }

            return false;
        }

        public WeaponController GetActiveWeapon()
        {
            return GetWeaponAtSlotIndex(ActiveWeaponIndex);
        }

        public WeaponController GetWeaponAtSlotIndex(int t_index)
        {
            if (t_index >= 0 && t_index < m_weaponSlots.Length)
            {
                return m_weaponSlots[t_index];
            }

            return null;
        }

        private int GetDistanceBetweenSlots(int t_fromSlotIndex, int t_toSlotIndex, bool t_order)
        {
            int t_distanceBetweenSlots;

            if (t_order)
            {
                t_distanceBetweenSlots = t_toSlotIndex - t_fromSlotIndex;
            }
            else
            {
                t_distanceBetweenSlots = -1 * (t_toSlotIndex - t_fromSlotIndex);
            }

            if (t_distanceBetweenSlots < 0)
            {
                t_distanceBetweenSlots = m_weaponSlots.Length + t_distanceBetweenSlots;
            }

            return t_distanceBetweenSlots;
        }

        private void OnWeaponSwitched(WeaponController t_newWeapon)
        {
            if (t_newWeapon != null)
            {
                t_newWeapon.ShowWeapon(true);
            }
        }
    }
}