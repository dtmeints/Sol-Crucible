using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.GameEnded)
            return;

        if (collision.gameObject.TryGetComponent<Orb>(out var ball))
        {
            GameManager.Instance.Stats.AddLostElement(ball.Rank - 1, ball.Element);
            ball.SetRank(1);
            ball.RB.linearVelocity *= -1;
            if (ball.RB.linearVelocity.magnitude < 10)
                ball.RB.linearVelocity *= 1.5f;
        }
    }
}
