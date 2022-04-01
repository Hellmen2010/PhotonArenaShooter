using TMPro;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


public abstract class Weapon : MonoBehaviour
{

    [Header("General stats")]
    [SerializeField] protected Transform damageEffect;
    [SerializeField] protected float damage;
    [SerializeField, Min(0f)] protected float inaccuracyDistance;
    [SerializeField, Min(0f)] protected float range;
    [SerializeField] protected WeaponSoundType weaponSoundType;
    [SerializeField] protected WeaponAudioScriptobject weaponAudioScriptobject;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected TMP_Text currentAmmoCanvas;



    protected Vector2 windowCenter => new Vector2(Screen.width / 2f, Screen.height / 2f);
    protected float nextFire = 0.0f;
    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public virtual void Attack()
    {
        audioSource.PlayOneShot(weaponAudioScriptobject.GetAudioClipByType(weaponSoundType));
    }
    protected virtual Ray Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(
            windowCenter.x + Random.Range(-inaccuracyDistance, inaccuracyDistance),
            windowCenter.y + Random.Range(-inaccuracyDistance, inaccuracyDistance)));
        Debug.DrawRay(ray.origin, ray.direction * 2200f, Color.red);
        return ray;
    }

}
