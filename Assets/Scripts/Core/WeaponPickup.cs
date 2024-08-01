using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class WeaponPickup : InteractableBase
    {
        private WeaponController m_weaponController;

        private void Start()
        {
            m_weaponController = GetComponent<WeaponController>();
        }

        public override void OnInteract()
        {
            base.OnInteract();

            OnPicked();
        }

        public void OnPicked()
        {
            WeaponManager t_manager = FindObjectOfType<WeaponManager>();

            if (t_manager.AddWeapon(m_weaponController))
            {
                if (t_manager.GetActiveWeapon() == null)
                {
                    t_manager.SwitchWeapon(true);
                }
            }
        }
    }
}