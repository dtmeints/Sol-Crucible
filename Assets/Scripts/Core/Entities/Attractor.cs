using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Attractor : MonoBehaviour
{
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] float falloff = 3;
    [SerializeField] float pullStrength = 4f;
    [SerializeField] float minPullStrength = 5f, maxPullStrength = 60f;
    float radiusSquared;

    HashSet<Rigidbody2D> InRangeRigidbodies = new();

    private void Awake()
    {
        radiusSquared = circleCollider.radius * circleCollider.radius;
    }

    private void FixedUpdate()
    {
        foreach (var rigidbody in InRangeRigidbodies)
        {
            Vector2 objectToMe = transform.position - rigidbody.transform.position;
            float distance = objectToMe.sqrMagnitude;
            rigidbody.AddForce(Vector2.Lerp(pullStrength * objectToMe.normalized, Vector2.zero, distance / radiusSquared));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            InRangeRigidbodies.Add(collision.attachedRigidbody);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            InRangeRigidbodies.Remove(collision.attachedRigidbody);
        }
    }

    public void SetPullStrength(float percent)
    {
        pullStrength = Mathf.Lerp(minPullStrength, maxPullStrength, percent);
    }

}
