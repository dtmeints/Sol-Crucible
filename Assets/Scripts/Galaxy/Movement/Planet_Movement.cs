using UnityEngine;

public class Planet_Movement : MonoBehaviour
{
    public float RotateSpeed;
    public float MoveSpeed;
    public float Radius;
    public Transform Target;

    [Space(10)]
    public Transform Shadow;

    [HideInInspector] public Vector2 moveDir;

    private Vector3 startOrbitPos;

    private float angle = 4.3f;

    private bool inOrbit;

    void Start()
    {
        if (Target != null)
        {
            inOrbit = true;
        }
    }


    void FixedUpdate()
    {
        if (Target != null && inOrbit)
        {
            MoveAround();
        }

        if (!inOrbit && Target == null)
        {
            Vector3 step = moveDir.normalized * MoveSpeed;
            transform.position += step;
        }

        if (!inOrbit && Target != null)
        {
            transform.position = Vector3.Lerp(transform.position, startOrbitPos, 0.05f);

            if (Vector3.Distance(transform.position, startOrbitPos) <= 0.01f)
            {
                inOrbit = true;
            }
        }
    }


    void MoveAround()
    {
        float x = Target.position.x + Mathf.Cos(angle) * Radius;
        float y = Target.position.y + Mathf.Sin(angle) * Radius;

        transform.position = new Vector3(x, y);

        angle += RotateSpeed * Time.deltaTime;


        // Planet Shadow rotation (removed because it didn't look good)

        // Vector3 pos = transform.position;
        // Vector3 targetPos = Target.position;

        // targetPos.x = targetPos.x - pos.x;
        // targetPos.y = targetPos.y - pos.y;

        // float shadowAngle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        // Shadow.localRotation = Quaternion.Euler(new Vector3(0, 0, shadowAngle - 90f));
    }


    public void GenerateOrbitStart()
    {
        Radius = Random.Range(3f, 9f);
        RotateSpeed = Random.Range(0.05f, 0.2f) * (Random.Range(0, 2) * 2 - 1);
        angle = Random.Range(0, Mathf.PI * 2f);
        float x = Target.position.x + Mathf.Cos(angle) * Radius;
        float y = Target.position.y + Mathf.Sin(angle) * Radius;

        startOrbitPos = new Vector3(x, y);
    }
}
