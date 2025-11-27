using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeed = 2f;

    private float height;
    private float offset;
    private Vector3 startPosition;
    public bool beginScroll { get; set; } = true;
    void Start()
    {
        startPosition = transform.position;
        offset = transform.position.y;
        height = GetComponent<SpriteRenderer>().size.y;
    }
    void Update()
    {
        if (!beginScroll) return;

        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);
    }
}
