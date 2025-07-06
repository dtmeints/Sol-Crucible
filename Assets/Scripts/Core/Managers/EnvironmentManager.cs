using System;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] AnimationVolumeWeigth darkFog1;
    void Awake() {
        ItemPlacer.OnFirstSol += RemoveDarkFog1;
    }

    void RemoveDarkFog1(Transform newSol) {
        ItemPlacer.OnFirstSol -= RemoveDarkFog1;
        darkFog1.Play();
    }
    
    void OnDisable() {
        ItemPlacer.OnFirstSol -= RemoveDarkFog1;
    }
}