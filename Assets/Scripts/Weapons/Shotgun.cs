using Cinemachine;
using UnityEngine;

public class Shotgun : RangedWeapon
{
    [Header("ShotGun")]
    [SerializeField] protected bool shotgun;
    [SerializeField] protected int bulletsPerShot;

    protected override void DoDamage()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            if (Physics.Raycast(Raycasting(), out raycastHit, range))
            {
                if (raycastHit.transform.TryGetComponent<Damagable>(out var damagable))
                {
                    damagable.TakeDamage(damage);
                    Instantiate(damageEffect, raycastHit.point, Quaternion.identity);
                }
                else
                    Instantiate(damageEffect, raycastHit.point, Quaternion.identity);
            }
        }
    }
}
