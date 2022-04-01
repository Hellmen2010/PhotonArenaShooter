using UnityEngine;

public class ChooseMapButton : MonoBehaviour
{
    public void ChoseMap(int value)
    {
        switch (value)
        {
            case 0:
                Launcher.Instance.levelNumber = 1;
                break;
            case 1:
                Launcher.Instance.levelNumber = 2;
                break;
            case 2:
                Launcher.Instance.levelNumber = 3;
                break;
        }

    }
}
