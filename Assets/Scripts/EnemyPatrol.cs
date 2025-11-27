using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Invader))]
public class EnemyPatrol : MonoBehaviour
{

    [Header("Locations")]
    public Vector3[] moveSpots;
    public float waitTime;
    public float speed;

    private int nthPoint = 0;
    private Rigidbody2D RB;
    private Vector3 currentPoint;
    private Invader invader;
    private float timer = 0;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        invader = GetComponent<Invader>();

        if (moveSpots.Length > 0)
        {
            currentPoint = moveSpots[0];
        }

        timer = waitTime;
    }
    private void Update()
    {
        if (invader == null || currentPoint == null)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, currentPoint, speed * Time.deltaTime);

        if (!invader.autoRotate)
        {
            invader.RotateTo(currentPoint);
        }

        if (Vector2.Distance(transform.position, currentPoint) < 0.5f)
        {
            invader.autoRotate = true;
            invader.autoShoot = true;
            invader.autoAim = true;

            if (timer <= 0.4f)
            {
                timer = waitTime;

                if (nthPoint < moveSpots.Length - 1)
                {
                    nthPoint++;
                    currentPoint = moveSpots[nthPoint];
                }
                else
                {
                    Destroy(gameObject); 
                }

                invader.autoRotate = false;
                invader.autoShoot = false;
                invader.autoAim = false;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
