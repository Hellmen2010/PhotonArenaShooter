using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeWeapon : Weapon
{
    [SerializeField] protected float attackRate;
    protected RaycastHit raycastHit;
    protected float nextAttack;
    private void OnEnable()
    {
        currentAmmoCanvas.enabled = false;
    }
    private void OnDisable()
    {
        currentAmmoCanvas.enabled = enabled;
    }
    public override void Attack()
    {
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + attackRate;
            DoDamage();
            base.Attack();
        }
    }
    protected virtual void DoDamage()
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
