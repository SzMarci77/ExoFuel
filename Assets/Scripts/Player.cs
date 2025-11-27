using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Player : MonoBehaviour
{
    [Header("Player")]
    public float speed = 5f;
    public int health = 3;

    private SpriteRenderer spriteRenderer;
    private Vector3 currentPos;

    [Header("Weapon Templates")]
    public int currentTemplate = -1;
    public List<Player> upgradeTemplates = new List<Player>();

    [Header("Guns")]
    public List<Gun> guns = new List<Gun>();
    private float shieldDuration = 0f;
    private float shieldTimer = 0f;

    public GameObject shieldBubblePrefab;

    private GameObject activeShieldBubble;
    private AudioSource audioSource;
    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            currentPos.x -= speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            currentPos.x += speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            currentPos.y += speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            currentPos.y -= speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E) && shieldTimer <= 0)
        {
            shieldDuration = 2f; 
            
            shieldTimer = 30f;

            ActivateShieldBubble();
        }
        else
        {
            shieldTimer -= Time.deltaTime;
        }

        if (shieldDuration > 0)
        {
            shieldDuration -= Time.deltaTime;
        }
        else
        {
            DectivateShieldBubble();
        }
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        currentPos.x = Mathf.Clamp(currentPos.x, leftEdge.x + 1, rightEdge.x - 1);
        Vector3 upperEdge = Camera.main.ViewportToWorldPoint(Vector3.up);
        Vector3 bottomEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        currentPos.y = Mathf.Clamp(currentPos.y, bottomEdge.y + 1, upperEdge.y - 1);


        transform.position = currentPos;

        if (Input.GetKey(KeyCode.Space))
        {
            foreach (Gun gun in guns)
            {
                gun.Shoot();
            }
        }
    }

    public float getShieldDuration()
    {
        return this.shieldDuration;
    }

    public void acquireSpriteRenderer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        currentPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager.Instance.UpgradeWeapons();
        audioSource = GetComponent<AudioSource>();
    }
    public void beUnkillable(float toSeconds)
    {
        shieldDuration = toSeconds;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (shieldDuration > 0)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("InvaderMissile") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            GameManager.Instance.OnPlayerKilled();
        }
    }
    public void ActivateShieldBubble()
    {
        if (activeShieldBubble == null)
        {
            activeShieldBubble = Instantiate(shieldBubblePrefab, transform.position, Quaternion.identity);
            activeShieldBubble.transform.SetParent(transform);
            activeShieldBubble.transform.localPosition = Vector3.zero;
        }
    }
    public void DectivateShieldBubble()
    {
        if (activeShieldBubble != null)
        {
            Destroy(activeShieldBubble);
            activeShieldBubble = null;
        }
    }
}
