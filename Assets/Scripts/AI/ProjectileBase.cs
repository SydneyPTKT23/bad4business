using SLC.Bad4Business.Core;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Damageable>()?.InflictDamage(damage, gameObject);
            Destroy(gameObject);
        }
    }
}