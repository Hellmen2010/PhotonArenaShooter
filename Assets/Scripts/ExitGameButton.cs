using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameButton : MonoBehaviour
{
    private Canvas exitGameCanvac;
    private void Start()
    {
        exitGameCanvac = GetComponent<Canvas>();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        exitGameCanvac.enabled = false;
    }
}
