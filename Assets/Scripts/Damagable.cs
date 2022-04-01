using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Rendering.PostProcessing;

public abstract class Damagable : MonoBehaviourPunCallbacks
{
    [SerializeField] protected ScoreBoard scoreBoard;
    [SerializeField] protected float maxHealth;
    [SerializeField] private Image healthbarImage;
    protected float currentHealth;
    PlayerManager playerManager;
    PhotonView PV;
    Vignette vignette;
    protected int kills, deaths;
    
    private void Awake()
    {
        currentHealth = maxHealth;
        scoreBoard = GetComponent<ScoreBoard>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    private void Start()
    {
        transform.name = PV.Owner.NickName;
    }
    private void Update()
    {
        if (transform.position.y < -10f)
        {
            Die();
        }
    }
    public void TakeDamage(float damage)
    {
        //StartCoroutine(DamageEffects());
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    [PunRPC]
    protected void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
        {
            return;
        }

        currentHealth -= damage;
        healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    //protected void Kill()
    //{
    //    kills = (int)PhotonNetwork.LocalPlayer.CustomProperties["Kills"];
    //    kills++;
    //    Hashtable hash = new Hashtable();
    //    hash.Add("Kills", kills);
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    //}
    protected virtual void Die()
    {
        //deaths = (int)PhotonNetwork.LocalPlayer.CustomProperties["Deaths"];
        //deaths++;
        //Hashtable hash = new Hashtable();
        //hash.Add("Deaths", deaths);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        playerManager.Die();
    }
    public IEnumerator DamageEffects()
    {
        vignette.color.Override(Color.red);
        yield return new WaitForSeconds(1);
        vignette.color.Override(Color.white);
    }
}