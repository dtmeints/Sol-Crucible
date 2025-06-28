using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIOptionsMenu : MonoBehaviour
{
    public static bool iHateRomanNumeralsMode = false;
    public TextMeshProUGUI romanNumeralsText;


    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        GameManager.Instance.StartLevel(GameManager.Instance.CurrentLevel);
    }

    public void SetIHateRomanNumeralsMode(float setting)
    {
        int settingInt = (int)setting;
        if (settingInt == 1)
        {
            iHateRomanNumeralsMode = true;
            romanNumeralsText.text = "Arabic Numerals (1, 2, 3...)";
        }
        else
        {
            iHateRomanNumeralsMode = false;
            romanNumeralsText.text = "Roman Numerals (I, II, III, IV...)";
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
