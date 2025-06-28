using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] D_Level FreePlayLevel;
    public void EnterGame()
    {
        GameManager.Instance.StartLevelInSequence(0);
    }

    public void StartFreePlay()
    {
        GameManager.Instance.StartLevel(FreePlayLevel);
    }
}
