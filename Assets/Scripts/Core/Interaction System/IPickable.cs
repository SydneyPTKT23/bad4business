using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public interface IPickable
    {
        Rigidbody Rigid { get; set; }

        void OnPickup();
        void OnHold();
        void OnRelease();
    }
}