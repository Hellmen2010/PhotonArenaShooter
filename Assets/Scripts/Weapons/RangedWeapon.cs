using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using TMPro;

public abstract class RangedWeapon : Weapon
{
    [SerializeField, Min(0)] protected int maxAmmo;
    [SerializeField, Min(0)] protected float fireRate;
    [SerializeField, Min(0)] protected float reloadTime;
    [SerializeField, Range(10f, 180f)] protected float zoomRange;

    /// <summary>
    /// Delay between fires
    /// </summary>
    protected Coroutine ReloadingRoutine;
    
    protected int currentAmmo;
    protected bool isReloading => ReloadingRoutine != null;
    protected RaycastHit raycastHit;

    protected override void Awake()
    {
        base.Awake();
        currentAmmo = maxAmmo;
    }
    private void OnEnable()
    {
        currentAmmoCanvas.text = currentAmmo.ToString();
    }

    public override void Attack()
    {
        if (CanShoot)
        {
            DoDamage();
            currentAmmo--;
            AmmoCanvasValueChange();
            nextFire = Time.time + fireRate;
            base.Attack();
        }
    }

    private void AmmoCanvasValueChange()
    {
        int canvasAmmoValue = Convert.ToInt32(currentAmmoCanvas.text);
        canvasAmmoValue--;
        currentAmmoCanvas.text = canvasAmmoValue.ToString();
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
    public void Reload()
    {
        if (currentAmmo == maxAmmo) return;
        if (ReloadingRoutine != null) StopCoroutine(ReloadingRoutine);
        ReloadingRoutine = StartCoroutine(nameof(ReloadRoutine));
    }

    public virtual void Aim(CinemachineVirtualCamera zoomCamera) => zoomCamera.m_Lens.FieldOfView = zoomRange;
    protected IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        currentAmmoCanvas.text = currentAmmo.ToString();
        ReloadingRoutine = null;
    }
    public bool CanShoot => currentAmmo > 0 && !isReloading && Time.time > nextFire;
}