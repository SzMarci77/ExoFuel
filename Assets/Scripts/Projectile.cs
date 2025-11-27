using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    public Vector3 direction = Vector3.up;
    public float speed = 5f;
    public GameObject explosionPrefab;

    private Rigidbody2D rb; 
    private BoxCollider2D boxCollider;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "4Player";

        rb.velocity = direction * speed;
    }

    public void SetDirection(Transform target)
    {
        if (target)
        {
            direction = (target.position - transform.position).normalized;
            
            float offset = 90f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
        }

        else
        {
            direction = transform.up;
        }
    }
    public void setSpeed(float speed)
    {
        this.speed = speed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckCollision(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        CheckCollision(other);
    }
    private void CheckCollision(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Boundary") ||
           other.gameObject.layer == LayerMask.NameToLayer("Invader") ||
           other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
        }
    }
}
