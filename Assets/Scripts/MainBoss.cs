using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainBoss : MonoBehaviour
{
    [Header("Boss stats")]
    public float speed = 4f;             
    public float cycleTime = 5f;          
    public float startSpawningTime = 5f;  
    public bool secondPhase = false;
    public GameObject healthBar;

    [SerializeField] private Text BossWarning;

    private Vector2 centerRightDestination;
    private Vector2 centerLeftDestination;
    private Vector2 centerScreen;          
    private bool movingRight = true;      

    private Invader invaderComponent;
    private int healthStart;
    private bool isInSecondPhase = false;
    private Slider healthSlider;

    private void Start()
    {
        invaderComponent = gameObject.GetComponent<Invader>();
        invaderComponent.autoAim = false;
        invaderComponent.autoShoot = false;
        invaderComponent.GetComponent<BoxCollider2D>().enabled = false;
        healthSlider = healthBar.GetComponent<Slider>();
        healthStart = invaderComponent.health;
        healthSlider.maxValue = healthStart;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        Vector3 centerScreenPoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0));

        centerScreen = new Vector2(centerScreenPoint.x, centerScreenPoint.y);
        centerRightDestination = new Vector2((centerScreenPoint.x + rightEdge.x) / 2, centerScreenPoint.y);
        centerLeftDestination = new Vector2((centerScreenPoint.x + leftEdge.x) / 2, centerScreenPoint.y);

        StartCoroutine(StartSpawnCountdown());
    }
    private IEnumerator StartSpawnCountdown()
    {
        float firstphase = startSpawningTime - 10;
        yield return new WaitForSeconds(firstphase);

        BossWarning.gameObject.SetActive(true);


        yield return new WaitForSeconds(startSpawningTime - firstphase);


        while (Vector2.Distance(transform.position, centerScreen) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, centerScreen, speed * Time.deltaTime);
            yield return null;
        }

        BossWarning.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);

        invaderComponent.GetComponent<BoxCollider2D>().enabled = true;

        StartCoroutine(CheckHealthForSecondPhase());
        StartCoroutine(AlternateMovement());
    }

    private void Update()
    {
        healthSlider.value = invaderComponent.health;

    }

    private IEnumerator AlternateMovement()
    {
        invaderComponent.autoAim = true;
        invaderComponent.autoShoot = true;

        while (true)
        {

            Vector2 target = movingRight ? centerRightDestination : centerLeftDestination;


            while (Vector2.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }

            movingRight = !movingRight;

            yield return new WaitForSeconds(cycleTime);
        }
    }
    private IEnumerator CheckHealthForSecondPhase()
    {
        while (secondPhase && !isInSecondPhase)
        {
            if (invaderComponent.health <= healthStart * 0.5f)
            {
                ActivateSecondPhase();
            }
            yield return null;
        }
    }
    private void ActivateSecondPhase()
    {
        isInSecondPhase = true;
        speed *= 1.5f; 
        cycleTime /= 1.5f;

        Debug.Log("Second phase activated!");
    }
}



