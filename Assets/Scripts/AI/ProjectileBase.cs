using SLC.Bad4Business.AI;
using UnityEngine;
using System;

/// <summary>
/// Base class for projectiles, handling owner assignment and initial trajectory.
/// </summary>
public abstract class ProjectileBase : MonoBehaviour
{
    // The entity that fired the projectile
    public GameObject Owner { get; private set; }
    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDirection { get; private set; }

    // Velocity inherited from the weapon (e.g., for moving platforms or projectiles with momentum)
    public Vector3 InheritedMuzzleVelocity { get; private set; }

    public event Action OnShoot;

    // Initializes the projectile with data from the weapon that fired it.
    public virtual void Shoot(RangedAttackModule t_controller)
    {
        Owner = t_controller.Owner;
        InitialPosition = transform.position;
        InitialDirection = transform.forward;
        InheritedMuzzleVelocity = t_controller.MuzzleVelocity;

        OnShoot?.Invoke();
    }
}