using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    public Transform Target1, Target2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Target2 != null)
        {
            transform.position = new Vector3((Target1.position.x + Target2.position.x) / 2f, (Target1.position.y + Target2.position.y) / 2f, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(Target1.position.x, Target1.position.y, transform.position.z);
        }
    }
}
