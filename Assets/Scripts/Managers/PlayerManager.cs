using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public string playerClass = "Soldier";
    PhotonView PV;
    GameObject controller;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }
    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        playerClass = (string)PhotonNetwork.LocalPlayer.CustomProperties["PlayerClass"];
        Debug.Log("Chosen class is: " + playerClass);
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"{playerClass}"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {PV.ViewID });
    }
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
