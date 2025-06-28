using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILevelEndMenu : MonoBehaviour
{
    [SerializeField] GameObject solPurifiedText;
    [SerializeField] Button nextLevelButton;
    [SerializeField] Button replayLevelButton;
    [SerializeField] D_Level freePlayLevel;
 
    public void OnEnable()
    {
        if (!GameManager.Instance.IsInLevelSequence)
            nextLevelButton.gameObject.SetActive(false);
    }

    public void ReplayLevel()
    {
        _ = GameManager.Instance.StartLevel(GameManager.Instance.CurrentLevel);
    }

    public void NextLevel()
    {
        if (GameManager.Instance.CurrentLevelIndex + 1 >= GameManager.Instance.Levels.Length)
        {
            _ = GameManager.Instance.StartLevel(freePlayLevel);
            return;
        }
        _ = GameManager.Instance.StartLevelInSequence(GameManager.Instance.CurrentLevelIndex + 1);
    }

    public void EndLevel()
    {
        solPurifiedText.SetActive(true);

    }
}
