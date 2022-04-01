using UnityEngine;
using Photon.Pun;

public class ClassChooseButton : MonoBehaviour
{
    private void Awake()
    {
        var prop = PhotonNetwork.LocalPlayer.CustomProperties;
        string key = "PlayerClass";
        prop[key] = "Soldier";
        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
    }
    public void ChoseClass(int value)
    {
        var prop = PhotonNetwork.LocalPlayer.CustomProperties;
        string key = "PlayerClass";
        if (!prop.ContainsKey(key))
            prop.Add(key, "Soldier");
        switch (value)
        {
            case 0:
                prop[key] = "Soldier";
                break;
            case 1:
                prop[key] = "Tank";
                break;
            case 2:
                prop[key] = "Sniper";
                break;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
    }
}
