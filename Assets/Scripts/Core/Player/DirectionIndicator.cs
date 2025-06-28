using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    [SerializeField] SpriteRenderer tip;
    [SerializeField] LineRenderer line;
    [SerializeField] float lengthScaling = .1f;

    public void UpdateLine(Vector3 startPoint, Vector3 velocity)
    {
        Vector3 endPoint = startPoint + (velocity * lengthScaling);
        line.SetPositions(new Vector3[]{startPoint, endPoint});
        tip.transform.position = endPoint;
        tip.transform.rotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(endPoint - startPoint, Vector2.up));
    }
}
