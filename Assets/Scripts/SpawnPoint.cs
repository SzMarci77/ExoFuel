using UnityEngine;
public class SpawnPoint : MonoBehaviour
{
    [Header("Invaders")]
    public Invader invaderPrefab;
    public int numberOf;
    public float speed;

    [Header("RotateAndShoot")]
    public bool autoRotate;
    public bool autoShoot;
    public bool autoAim;
    public bool shootingEnabled = true;

    [Header("Locations")]
    public Vector3[] moveSpots;
    public float startSpawningTime;
    public float waitTime;
    private Vector3 initialPosition;
    private void Start()
    {
        initialPosition = transform.position;
        InvokeRepeating("SpawnInvaders", startSpawningTime, waitTime);
    }
    private void SpawnInvaders()
    {
        if (numberOf <= 0)
        {
            turnOff();
            return;
        }
        numberOf--;
        CreateInvader();
    }
    public void turnOff()
    {
        this.gameObject.SetActive(false);
        CancelInvoke("SpawnInvaders");
    }
    private void CreateInvader()
    {
        Invader invader = Instantiate(invaderPrefab, transform.position, Quaternion.identity);
        invader.autoAim = autoAim;
        invader.autoShoot = autoShoot;
        invader.shootingEnabled = shootingEnabled;
        invader.autoRotate = autoRotate;

        EnemyPatrol patrol = invader.gameObject.AddComponent<EnemyPatrol>();
        patrol.moveSpots = moveSpots;
        patrol.speed = speed;
        patrol.waitTime = waitTime;
    }
    private void OnDrawGizmos()
    {
        foreach (var point in moveSpots)
        {
            Gizmos.DrawWireSphere(point, 0.5f);
        }
    }
}
