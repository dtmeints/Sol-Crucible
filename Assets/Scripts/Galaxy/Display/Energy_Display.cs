using UnityEngine;
using TMPro;

public class Energy_Display : MonoBehaviour
{
    public TextMeshProUGUI energyText;

    public Vector3 GalaxyPos;
    public Vector3 GameplayPos;

    private Galaxy_Manager galaxyManager;

    void Start()
    {
        galaxyManager = GameObject.FindWithTag("GalaxyManager").GetComponent<Galaxy_Manager>();
    }

    void Update()
    {
        energyText.text = "Energy: " + galaxyManager.Energy.ToString();
    }

    public void Galaxy()
    {
        transform.localPosition = GalaxyPos;
    }

    public void Gameplay()
    {
        transform.localPosition = GameplayPos;
    }
}
