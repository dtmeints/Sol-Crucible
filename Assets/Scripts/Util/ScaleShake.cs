using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScaleShake : MonoBehaviour
{
    [SerializeField] float startDelay;
    [SerializeField] float duration;
    [SerializeField] Vector2 randomStrength = new(0.3f, 0.7f);
    [SerializeField] Vector2Int randomVibrato = new(5, 25);
    [SerializeField] float randomness = 90;
    [SerializeField] bool fadeOut = true;
    [SerializeField] ShakeRandomnessMode shakeRandomnessMode = ShakeRandomnessMode.Full;
    
    IEnumerator Start() {
        yield return new WaitForSeconds(startDelay);
        transform.DOShakeScale(duration, Random.Range(randomStrength.x, randomStrength.y), Random.Range(randomVibrato.x, randomVibrato.y), randomness, fadeOut, shakeRandomnessMode);
    }
}