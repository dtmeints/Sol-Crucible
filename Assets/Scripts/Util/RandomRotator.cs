using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomRotator : MonoBehaviour
{
    [SerializeField] float minRotationSpeed = -5, maxRotationSpeed = 5;

    float rotationSpeed;
    private void Start()
    {
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }
    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
