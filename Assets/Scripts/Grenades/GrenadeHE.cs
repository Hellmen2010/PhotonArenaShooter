using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeHE : Grenades
{
    [SerializeField] float explosionForce;

    public override void Explode()
    {
        base.Explode();
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider near in colliders)
        {
            Rigidbody rb = near.GetComponent<Rigidbody>();
            if(rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, radius, 1f, ForceMode.Impulse);

            if(near.TryGetComponent<Damagable>(out var damagable))
                damagable.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
