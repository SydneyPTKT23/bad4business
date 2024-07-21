using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("Interactable Settings")]
        [SerializeField] private bool holdToInteract = false;
        [SerializeField] private float holdDuration = 1.0f;

        [Space]
        [SerializeField] private bool isInteractable = true;

        public bool IsInteractable => isInteractable;

        public virtual void OnInteract()
        {
            Debug.Log("Interacted with: " + gameObject.name);
        }
    }
}
