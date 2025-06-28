using System;
using TMPro;
using UnityEngine;

public class UIStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI volitionsText, durationText, collisionsText, 
        airCreatedText, fireCreatedText, earthCreatedText, waterCreatedText, totalCreatedText,
        airLostText, fireLostText, earthLostText, waterLostText, totalLostText, volitionPowerText, decisivenessText, efficiencyText, wasteText;

    private void OnEnable()
    {
        Stats stats = GameManager.Instance.Stats;
        volitionsText.text = stats.volitionsCast.ToString();
        TimeSpan time = TimeSpan.FromSeconds(stats.duration);
        durationText.text = string.Format("{0}:{1:D2}", time.Minutes, time.Seconds);
        collisionsText.text = stats.collisionsMade.ToString();
        airCreatedText.text = "AIR — " + stats.airCreated.ToString();
        fireCreatedText.text = "FIRE — " + stats.fireCreated.ToString();
        earthCreatedText.text = "EARTH — " + stats.earthCreated.ToString();
        waterCreatedText.text = "WATER — " + stats.waterCreated.ToString();
        airLostText.text = "AIR — " + stats.airLost.ToString();
        fireLostText.text = "FIRE — " + stats.fireLost.ToString();
        earthLostText.text = "EARTH — " + stats.earthLost.ToString();
        waterLostText.text = "WATER — " + stats.waterLost.ToString();
        totalLostText.text = "TOTAL — " + stats.TotalLost.ToString();
        totalCreatedText.text = "TOTAL — " + stats.TotalCreated.ToString();

        float volitionPower = (float)stats.TotalCreated / (float)stats.volitionsCast;
        volitionPower *= 100f;
        volitionPowerText.text = Mathf.RoundToInt(volitionPower).ToString() + "%";

        float decisiveness = (float)stats.volitionsCast / stats.duration;
        decisiveness *= 100f;
        decisivenessText.text = Mathf.RoundToInt(decisiveness).ToString() + "%";

        float efficiency =  (float)stats.requirements.OriginalTotal / (float)stats.TotalCreated;
        efficiency *= 100f;
        efficiencyText.text = Mathf.RoundToInt(efficiency).ToString() + "%";

        float waste = (float)stats.TotalLost / (float)stats.TotalCreated;
        waste *= 100f;
        wasteText.text = Mathf.RoundToInt(waste).ToString() + "%";
    }
}
