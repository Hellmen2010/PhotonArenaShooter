using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
[CreateAssetMenu(menuName = "ScriptableObjects/weaponAudioScriptobject")]
public class WeaponAudioScriptobject : ScriptableObject
{
    public WeaponAudioField[] weaponAudioFields;
    public AudioClip GetAudioClipByType(WeaponSoundType weaponsoundType)
    {
        return weaponAudioFields.First(x => x.weaponsoundType == weaponsoundType).audioType;
    }
}
public enum WeaponSoundType
{
    Automat,
    AWPRifle,
    ShotGun,
    Pistol,
    Knife,
    Grenade,
    Empthy
}
[Serializable]
public class WeaponAudioField
{
    public WeaponSoundType weaponsoundType;
   public  AudioClip audioType;
}
