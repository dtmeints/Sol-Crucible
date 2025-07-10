using UnityEngine;
using UnityEngine.SceneManagement;

public class Galaxy_Starter : MonoBehaviour
{
    public void OpenGalaxy()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("Galaxy");
    }
}
