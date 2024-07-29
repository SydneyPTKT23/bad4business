using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class LightSwitch : InteractableBase
    {
        [SerializeField] private Light m_light;

        public override void OnInteract()
        {
            base.OnInteract();
            m_light.enabled = !m_light.enabled;
        }
    }
}