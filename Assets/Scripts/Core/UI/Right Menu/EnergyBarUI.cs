using System;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    [SerializeField] Slider right;
    [SerializeField] Slider left;
    [SerializeField, Range(0f, 1f)] float energy;

    void OnDrawGizmos() {
        if (Application.isPlaying) return;
        UpdateEnergyUI();
    }

    void Update() => UpdateEnergyUI();

    void UpdateEnergyUI() {
        right.value = energy;
        left.value = energy;
    }
}