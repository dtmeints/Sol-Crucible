using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class AnimationVolumeWeigth : MonoBehaviour
{
    [SerializeField] Volume volume;
    [SerializeField] float from;
    [SerializeField] float to;
    [SerializeField] float duration;
    [SerializeField] Ease ease;
    
    void Reset() => volume = GetComponentInChildren<Volume>();
    void Awake() => volume ??= GetComponentInChildren<Volume>();
    public void Play() {
        volume.weight = from;
        DOTween.To(() => volume.weight, x => volume.weight = x, to, duration).SetEase(ease);
    }
}