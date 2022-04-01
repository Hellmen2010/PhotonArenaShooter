using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ScoreBoardItem : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    public void Initialize(Player player)
    {
        usernameText.text = player.NickName;
    }
}
