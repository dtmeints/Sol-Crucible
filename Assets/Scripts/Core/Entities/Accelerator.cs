using UnityEngine;

public class Accelerator : MonoBehaviour
{
    [SerializeField] float velocityMultiplier;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            collision.attachedRigidbody.linearVelocity *= velocityMultiplier;
        }
    }
}
