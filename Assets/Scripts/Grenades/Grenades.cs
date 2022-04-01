using UnityEngine;

public abstract class Grenades : Weapon
{
    [SerializeField] protected Grenades grenadePrefab;
    [SerializeField] protected Transform grenadePosition;
    [SerializeField] protected float delay;
    [SerializeField] protected float radius;
    [SerializeField] protected float throwSpeed;

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
        if(/*Time.time > nextFire*/gameObject.active)
        {
            nextFire = Time.time + nextFire;
            base.Attack();
            var currentGrenade = Instantiate(grenadePrefab, grenadePosition.position, grenadePosition.rotation);
            gameObject.SetActive(false);
            currentGrenade.Invoke("Explode", delay);
            currentGrenade.GetComponent<Rigidbody>().isKinematic = false;
            currentGrenade.transform.SetParent(null);
            currentGrenade.GetComponent<Rigidbody>().AddForce(Raycasting().direction * throwSpeed, ForceMode.Impulse);
        }  
    }
    public virtual void Explode()
    {
        Instantiate(damageEffect, gameObject.transform.position, Quaternion.identity);
    }
}
