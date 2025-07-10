using UnityEngine;

public class Mouse_Follower : MonoBehaviour
{
    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }
}
